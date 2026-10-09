#!/usr/bin/env python3
"""
Batch-generate 3D models for Rebirth: Eclipse System via the Tripo3D API.

Reads Assets/Game/Art/model_manifest.json, submits a text-to-3D task per model,
polls each task to completion, and downloads the resulting GLB to the model's
targetPath.

STATUS: This script has NOT been run end to end from the authoring environment
(no Tripo API key and no network access to the Tripo API were available here).
The request/response shape is based on Tripo's public API docs (v3 text-to-model +
task polling, Bearer auth, async task with the model URL under data.output). The
exact base URL, endpoint version, and output field name can differ by plan/version,
so they are CONFIGURABLE CONSTANTS at the top — verify them against your current
Tripo docs (https://developers.tripo3d.com) before a real run.

Usage:
    export TRIPO_API_KEY=tsk_xxx           # your key from the Tripo console; never commit it
    python3 Tools/tripo_generate.py --dry-run          # print what would be generated
    python3 Tools/tripo_generate.py --only char_arden_eclipse_sovereign weapon_voidrend
    python3 Tools/tripo_generate.py --category crystal  # just the 7 crystals
    python3 Tools/tripo_generate.py                     # generate everything missing

Design notes:
- Idempotent: skips any model whose targetPath already exists unless --force.
- Never hardcodes a key; reads TRIPO_API_KEY from the environment and errors clearly
  if it is missing (except in --dry-run).
- Writes a sidecar <target>.tripo.json next to each downloaded model recording the
  task id and prompt, for provenance / regeneration.
- Characters flagged "rig": true still download as a static mesh here; rigging is a
  separate Tripo step or external (see Docs/Build/03_ModelPipeline_Tripo3D.md). This
  script intentionally does not assume the rig endpoint shape.
"""

import argparse
import json
import os
import sys
import time
import urllib.error
import urllib.request
from pathlib import Path

# --- Configurable API shape (verify against current Tripo docs) ---------------
API_BASE = os.environ.get("TRIPO_API_BASE", "https://api.tripo3d.ai/v2/openapi")
# Endpoint + payload for creating a text-to-3D task. Tripo's v2 openapi uses a single
# /task endpoint with a "type" discriminator; the newer v3 docs describe
# /v3/generation/text-to-model. Override via env if your plan differs.
CREATE_PATH = os.environ.get("TRIPO_CREATE_PATH", "/task")
TASK_PATH_TEMPLATE = os.environ.get("TRIPO_TASK_PATH", "/task/{task_id}")
MODEL_VERSION = os.environ.get("TRIPO_MODEL_VERSION", "v2.5-20250123")
# Dotted path into the successful task JSON where the downloadable model URL lives.
# v2 openapi: data.output.pbr_model (or data.output.model). Adjust if needed.
OUTPUT_URL_PATHS = [
    "data.output.pbr_model",
    "data.output.model",
    "data.result.pbr_model.url",
    "data.output.model_mesh.url",
]
# ------------------------------------------------------------------------------

POLL_INTERVAL_SEC = 5
POLL_TIMEOUT_SEC = 600
REPO_ROOT = Path(__file__).resolve().parent.parent
MANIFEST = REPO_ROOT / "Assets" / "Game" / "Art" / "model_manifest.json"


def _dig(obj, dotted):
    cur = obj
    for part in dotted.split("."):
        if isinstance(cur, dict) and part in cur:
            cur = cur[part]
        else:
            return None
    return cur


def _api_key_or_die(dry_run):
    key = os.environ.get("TRIPO_API_KEY")
    if not key and not dry_run:
        sys.exit("ERROR: TRIPO_API_KEY is not set. Get one from the Tripo console and "
                 "`export TRIPO_API_KEY=...` (never commit it). Use --dry-run to preview without a key.")
    return key


def _post_json(url, payload, key):
    body = json.dumps(payload).encode("utf-8")
    req = urllib.request.Request(url, data=body, method="POST")
    req.add_header("Content-Type", "application/json")
    req.add_header("Authorization", f"Bearer {key}")
    with urllib.request.urlopen(req, timeout=60) as resp:
        return json.loads(resp.read().decode("utf-8"))


def _get_json(url, key):
    req = urllib.request.Request(url, method="GET")
    req.add_header("Authorization", f"Bearer {key}")
    with urllib.request.urlopen(req, timeout=60) as resp:
        return json.loads(resp.read().decode("utf-8"))


def create_task(model, defaults, key):
    payload = {
        "type": "text_to_model",
        "model_version": MODEL_VERSION,
        "prompt": model["prompt"],
        "face_limit": model.get("faceLimit", defaults.get("faceLimit", 40000)),
        "texture": True,
        "pbr": defaults.get("pbr", True),
    }
    resp = _post_json(API_BASE + CREATE_PATH, payload, key)
    if resp.get("code", 0) != 0:
        raise RuntimeError(f"create failed: {resp}")
    task_id = _dig(resp, "data.task_id") or _dig(resp, "data.id")
    if not task_id:
        raise RuntimeError(f"no task_id in create response: {resp}")
    return task_id


def poll_task(task_id, key):
    url = API_BASE + TASK_PATH_TEMPLATE.format(task_id=task_id)
    deadline = time.time() + POLL_TIMEOUT_SEC
    while time.time() < deadline:
        resp = _get_json(url, key)
        status = (_dig(resp, "data.status") or "").lower()
        if status in ("success", "succeeded", "completed"):
            for p in OUTPUT_URL_PATHS:
                u = _dig(resp, p)
                if u:
                    return u
            raise RuntimeError(f"task done but no model URL found; inspect: {resp}")
        if status in ("failed", "cancelled", "canceled", "error", "ban"):
            raise RuntimeError(f"task {task_id} ended as {status}: {resp}")
        time.sleep(POLL_INTERVAL_SEC)
    raise TimeoutError(f"task {task_id} did not finish within {POLL_TIMEOUT_SEC}s")


def download(url, target: Path):
    target.parent.mkdir(parents=True, exist_ok=True)
    with urllib.request.urlopen(url, timeout=120) as resp:
        data = resp.read()
    target.write_bytes(data)
    return len(data)


def select_models(manifest, args):
    models = manifest["models"]
    if args.only:
        wanted = set(args.only)
        models = [m for m in models if m["key"] in wanted]
    if args.category:
        models = [m for m in models if m["category"] == args.category]
    return models


def main():
    ap = argparse.ArgumentParser(description="Generate Rebirth: Eclipse System models via Tripo3D.")
    ap.add_argument("--dry-run", action="store_true", help="print planned generations, call no API")
    ap.add_argument("--force", action="store_true", help="regenerate even if the target file exists")
    ap.add_argument("--only", nargs="*", help="only these model keys")
    ap.add_argument("--category", help="only this category (character/weapon/armor_set/helmet/crystal/enemy/boss/prop)")
    args = ap.parse_args()

    if not MANIFEST.exists():
        sys.exit(f"ERROR: manifest not found at {MANIFEST}")
    manifest = json.loads(MANIFEST.read_text(encoding="utf-8"))
    defaults = manifest.get("defaults", {})
    models = select_models(manifest, args)
    key = _api_key_or_die(args.dry_run)

    print(f"Manifest: {len(manifest['models'])} models; selected {len(models)}.")
    done = skipped = failed = 0
    for m in models:
        target = REPO_ROOT / m["targetPath"]
        if target.exists() and not args.force:
            print(f"  skip (exists): {m['key']} -> {m['targetPath']}")
            skipped += 1
            continue
        if args.dry_run:
            print(f"  would generate: {m['key']} -> {m['targetPath']}")
            print(f"      prompt: {m['prompt']}")
            continue
        try:
            print(f"  generating: {m['key']} ...", flush=True)
            task_id = create_task(m, defaults, key)
            url = poll_task(task_id, key)
            size = download(url, target)
            # Provenance sidecar. Deliberately omits the signed download URL (it can carry a
            # short-lived token) so nothing secret ends up committed.
            sidecar = target.with_suffix(target.suffix + ".tripo.json")
            sidecar.write_text(json.dumps(
                {"key": m["key"], "task_id": task_id, "prompt": m["prompt"]},
                indent=2), encoding="utf-8")
            print(f"    OK: {m['key']} ({size} bytes) -> {m['targetPath']}")
            done += 1
        except (urllib.error.URLError, RuntimeError, TimeoutError, ValueError) as e:
            print(f"    FAIL: {m['key']}: {e}", file=sys.stderr)
            failed += 1

    print(f"\nDone. generated={done} skipped={skipped} failed={failed}")
    if failed:
        sys.exit(1)


if __name__ == "__main__":
    main()
