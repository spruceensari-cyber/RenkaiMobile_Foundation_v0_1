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

        private float pitch;
        private bool fireHeld;

        private void Update()
        {
            if (motor != null && movementJoystick != null)
                motor.SetMoveInput(movementJoystick.Value);

            if (lookArea != null && yawRoot != null && pitchRoot != null)
            {
                Vector2 look = lookArea.ConsumeDelta();
                yawRoot.Rotate(0f, look.x, 0f, Space.Self);
                pitch = Mathf.Clamp(pitch - look.y, -pitchLimit, pitchLimit);
                pitchRoot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            }

            if (fireHeld && weapon != null)
                weapon.TryFire();
        }

        public void SetFireHeld(bool value) => fireHeld = value;
        public void Jump() => motor?.TryJump();
        public void Reload() => weapon?.TryReload();
        public void SetCrouch(bool value) => motor?.SetCrouching(value);
    }
}
