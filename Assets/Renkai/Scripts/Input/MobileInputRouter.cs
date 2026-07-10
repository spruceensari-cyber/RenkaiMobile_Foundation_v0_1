using UnityEngine;
using Renkai.Combat;
using Renkai.Movement;

namespace Renkai.Input
{
    public sealed class MobileInputRouter : MonoBehaviour
    {
        [SerializeField] private MobileFpsMotor motor;
        [SerializeField] private VirtualJoystick movementJoystick;
        [SerializeField] private TouchLookArea lookArea;
        [SerializeField] private Transform yawRoot;
        [SerializeField] private Transform pitchRoot;
        [SerializeField] private HitscanWeapon weapon;
        [SerializeField, Range(30f, 89f)] private float pitchLimit = 85f;
        [SerializeField] private float desktopLookSensitivity = 2.2f;

        private float pitch;
        private bool fireHeld;

        private void Update()
        {
            UpdateMovement();
            UpdateLook();
            UpdateCombat();
        }

        private void UpdateMovement()
        {
            if (motor == null) return;

#if UNITY_EDITOR || UNITY_STANDALONE
            Vector2 keyboard = new Vector2(
                UnityEngine.Input.GetAxisRaw("Horizontal"),
                UnityEngine.Input.GetAxisRaw("Vertical"));

            if (keyboard.sqrMagnitude > 0.001f)
                motor.SetMoveInput(Vector2.ClampMagnitude(keyboard, 1f));
            else if (movementJoystick != null)
                motor.SetMoveInput(movementJoystick.Value);
            else
                motor.SetMoveInput(Vector2.zero);

            motor.SetCrouching(UnityEngine.Input.GetKey(KeyCode.LeftControl));
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
                motor.TryJump();
#else
            if (movementJoystick != null)
                motor.SetMoveInput(movementJoystick.Value);
#endif
        }

        private void UpdateLook()
        {
            if (yawRoot == null || pitchRoot == null) return;

            Vector2 look = Vector2.zero;

            if (lookArea != null)
                look += lookArea.ConsumeDelta();

#if UNITY_EDITOR || UNITY_STANDALONE
            look.x += UnityEngine.Input.GetAxis("Mouse X") * desktopLookSensitivity;
            look.y += UnityEngine.Input.GetAxis("Mouse Y") * desktopLookSensitivity;
#endif

            yawRoot.Rotate(0f, look.x, 0f, Space.Self);
            pitch = Mathf.Clamp(pitch - look.y, -pitchLimit, pitchLimit);
            pitchRoot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        private void UpdateCombat()
        {
            bool shouldFire = fireHeld;

#if UNITY_EDITOR || UNITY_STANDALONE
            shouldFire |= UnityEngine.Input.GetMouseButton(0);
            if (UnityEngine.Input.GetKeyDown(KeyCode.R))
                weapon?.TryReload();
#endif

            if (shouldFire && weapon != null)
                weapon.TryFire();
        }

        public void SetFireHeld(bool value) => fireHeld = value;
        public void Jump() => motor?.TryJump();
        public void Reload() => weapon?.TryReload();
        public void SetCrouch(bool value) => motor?.SetCrouching(value);
    }
}
