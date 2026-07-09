using UnityEngine;

namespace Renkai.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class MobileFpsMotor : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float walkSpeed = 4.8f;
        [SerializeField, Min(0.1f)] private float crouchSpeed = 2.8f;
        [SerializeField, Min(0.1f)] private float jumpHeight = 1.15f;
        [SerializeField] private float gravity = -24f;

        private CharacterController controller;
        private Vector2 moveInput;
        private float verticalVelocity;

        public bool IsCrouching { get; private set; }
        public float PlanarSpeed { get; private set; }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        public void SetMoveInput(Vector2 value)
        {
            moveInput = Vector2.ClampMagnitude(value, 1f);
        }

        public void SetCrouching(bool value)
        {
            IsCrouching = value;
        }

        public void TryJump()
        {
            if (controller.isGrounded && !IsCrouching)
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        private void Update()
        {
            if (controller.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -2f;

            float speed = IsCrouching ? crouchSpeed : walkSpeed;
            Vector3 wish = transform.right * moveInput.x + transform.forward * moveInput.y;
            Vector3 planar = wish * speed;
            PlanarSpeed = new Vector2(planar.x, planar.z).magnitude;

            verticalVelocity += gravity * Time.deltaTime;
            Vector3 velocity = planar + Vector3.up * verticalVelocity;
            controller.Move(velocity * Time.deltaTime);
        }
    }
}
