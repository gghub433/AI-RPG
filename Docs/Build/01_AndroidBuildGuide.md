# Android Build Guide

This project targets Unity 6000.0.32f1 LTS (URP) for Android ARM64. **No Unity
Editor was available in the environment this project was authored in** — this
guide is the reproducible procedure to follow once the project is opened in a real
Unity Editor install; it has not been executed end-to-end, and no APK has been
produced by this session. See `Docs/Build/00_Status.md` for the honest
done/not-done breakdown.

## 1. Prerequisites

- Unity 6000.0.32f1 (or the closest available 6000 LTS point release) with the
  **Android Build Support** module (including OpenJDK and Android SDK & NDK Tools)
  installed via Unity Hub.
- If using an external Android SDK/NDK instead of Unity's bundled ones, configure
  their paths under **Preferences → External Tools**.

## 2. First Open

1. Open Unity Hub → **Add** → select the repository root (`AI-RPG/`).
2. Let the Editor import; it will generate `Library/`, regenerate `.sln`/`.csproj`
   files, and populate any `ProjectSettings/*.asset` files this repo does not
   already carry a hand-authored version of (most are Editor-generated and are
   intentionally not hand-written here — see `Docs/Build/00_Status.md`).
3. Confirm the active Render Pipeline Asset is a URP asset (**Edit → Project
   Settings → Graphics**). If empty, create one via **Assets → Create → Rendering →
   URP Asset (with Universal Renderer)** and assign it — this project ships the
   `QualitySettingsManager` script expecting four URP assets (Low/Medium/High/Ultra)
   to exist as real assets, which must be created in-editor since URP assets are
   binary-ish ScriptableObjects not practical to hand-author reliably.

## 3. Player Settings (Android)

Under **Edit → Project Settings → Player → Android tab**:

| Setting | Value |
|---|---|
| Company Name | (your studio/publisher name) |
| Product Name | Rebirth: Eclipse System |
| Package Name (application identifier) | `com.rebirtheclipse.game` (placeholder — change before any real release) |
| Version | 0.1.0 |
| Bundle Version Code | 1 |
| Default Orientation | Landscape Left (or Auto Landscape if dynamic rotation is desired) |
| Target API Level | Highest installed (Android 14 / API 34 recommended) |
| Minimum API Level | Android 8.0 (API 26) — balances URP mobile feature support against device reach |
| Scripting Backend | IL2CPP (required for ARM64) |
| Target Architectures | ARM64 only (uncheck ARMv7 and x86) |
| Install Location | Automatic |
| Internet Access | Require (if any online features are added later; otherwise Auto) |

### Icons & Splash
Assign adaptive icon layers and a splash image once real art exists
(`Assets/Game/Art/UI/`). Placeholder-free final presentation per the design brief
means these should not ship as Unity's default icon in a real release build.

### Permissions
No special permissions are required for the current offline, local-save-only
feature set. Do not add Internet/Storage permissions speculatively.

### Compression / Quality
- **Texture Compression:** ASTC (recommended default for modern Android GPUs).
- **Compression Method (Build Settings):** LZ4HC for release builds.

## 4. Quality Presets (Low / Medium / High / Ultra)

`QualitySettingsManager` (`Assets/Game/Scripts/Core/QualitySettingsManager.cs`)
expects four `UniversalRenderPipelineAsset` references wired in its Inspector.
Author them as:

- **Low:** no shadows, render scale 0.75, no MSAA, simplified post-processing.
- **Medium:** hard shadows only, render scale 0.9, 2x MSAA.
- **High:** soft shadows, render scale 1.0, 4x MSAA, bloom + color grading on.
- **Ultra:** soft shadows + higher shadow distance, render scale 1.0 (or
  device-native), 4x MSAA, full post-processing stack including SSAO where the
  target device profile supports it.

Also set each tier as a row in **Project Settings → Quality**, matching
`QualityPreset` enum order (Low=0, Medium=1, High=2, Ultra=3) since
`QualitySettingsManager.ApplyPreset` calls `QualitySettings.SetQualityLevel`
by that same index.

## 5. Building the APK

1. **File → Build Settings**.
2. Switch platform to **Android** if not already active.
3. Add the five scenes under `Assets/Game/Scenes/*/*.unity` to the **Scenes In
   Build** list, in order: Boot, MainMenu, Prologue, FirstRegion, FirstDungeon.
   (These scenes currently ship as minimal empty containers — see
   `Docs/Build/00_Status.md` — so a build will run but show empty scenes until
   gameplay prefabs/hierarchy are authored in-editor per
   `Docs/Build/02_SceneAuthoringChecklist.md`.)
4. Click **Build** (or **Build And Run** with a connected/ADB-visible device),
   choose an output path, e.g. `Builds/Android/RebirthEclipse.apk`.
5. After the build completes, verify:
   - The `.apk` file exists at the chosen path and is a plausible size (a few
     hundred MB once real art/audio are included; likely under 50MB with current
     placeholder-only content).
   - `adb install -r Builds/Android/RebirthEclipse.apk` succeeds on a connected
     device/emulator, and `adb shell am start -n com.rebirtheclipse.game/com.unity3d.player.UnityPlayerActivity`
     launches it.
   - Do not report the game as "running on device" without having actually run
     these two adb commands (or the Build And Run equivalent) — per the project's
     own rule against claiming unverified success.

## 6. AAB (Play Store) Follow-up

Once the APK path is validated: enable **Build App Bundle (Google Play)** in Build
Settings, enable **Player Settings → Publishing Settings → Custom Keystore** with a
real release keystore (never commit a keystore or its passwords to this
repository), and repeat the build step to produce a `.aab` instead.
