using UnityEngine;
using UnityEngine.InputSystem;

namespace RebirthEclipse.Character
{
    /// <summary>
    /// Third-person movement + camera-relative locomotion driven by Unity's Input System.
    /// Reads from a generated "PlayerInputActions" asset (virtual joystick on mobile maps
    /// to the same Move/Look actions as keyboard+mouse in editor, so this script never
    /// branches on platform). Combat actions (attack/dodge/block/skills) are read by
    /// CombatController, not here — this class owns locomotion only.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(CharacterStats))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float rotationSpeedDegPerSec = 540f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float jumpHeight = 1.2f;

        [Header("References")]
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Animator animator;

        private CharacterController controller;
        private CharacterStats stats;
        private Vector2 moveInput;
        private Vector3 verticalVelocity;
        private bool jumpPressed;
        private bool isLocomotionLocked; // set true during cutscenes/dialogue/hitstun

        private static readonly int AnimSpeed = Animator.StringToHash("Speed");
        private static readonly int AnimGrounded = Animator.StringToHash("Grounded");

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            stats = GetComponent<CharacterStats>();
        }

        public void OnMove(InputAction.CallbackContext ctx) => moveInput = ctx.ReadValue<Vector2>();
        public void OnJump(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) jumpPressed = true;
        }

        public void SetLocomotionLocked(bool locked) => isLocomotionLocked = locked;

        private void Update()
        {
            if (stats.IsDead)
            {
                moveInput = Vector2.zero;
            }

            HandleGravityAndGrounding();

            if (isLocomotionLocked || stats.IsDead)
            {
                UpdateAnimator(0f);
                return;
            }

            Vector3 moveDir = CameraRelativeDirection(moveInput);
            float speed = stats.Derived.moveSpeed;
            Vector3 horizontalMotion = moveDir * speed;

            if (jumpPressed && controller.isGrounded)
            {
                verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            jumpPressed = false;

            Vector3 motion = (horizontalMotion + verticalVelocity) * Time.deltaTime;
            controller.Move(motion);

            if (moveDir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeedDegPerSec * Time.deltaTime);
            }

            UpdateAnimator(horizontalMotion.magnitude / Mathf.Max(0.01f, speed));
        }

        private void HandleGravityAndGrounding()
        {
            if (controller.isGrounded && verticalVelocity.y < 0f)
            {
                verticalVelocity.y = -1f; // small downward stick, keeps isGrounded stable
            }
            verticalVelocity.y += gravity * Time.deltaTime;
        }

        private Vector3 CameraRelativeDirection(Vector2 input)
        {
            if (input.sqrMagnitude < 0.0001f) return Vector3.zero;
            Vector3 fwd = cameraPivot != null ? cameraPivot.forward : Vector3.forward;
            Vector3 right = cameraPivot != null ? cameraPivot.right : Vector3.right;
            fwd.y = 0f; right.y = 0f;
            fwd.Normalize(); right.Normalize();
            Vector3 dir = fwd * input.y + right * input.x;
            return dir.sqrMagnitude > 1f ? dir.normalized : dir;
        }

        private void UpdateAnimator(float normalizedSpeed)
        {
            if (animator == null) return;
            animator.SetFloat(AnimSpeed, normalizedSpeed);
            animator.SetBool(AnimGrounded, controller.isGrounded);
        }
    }
}
