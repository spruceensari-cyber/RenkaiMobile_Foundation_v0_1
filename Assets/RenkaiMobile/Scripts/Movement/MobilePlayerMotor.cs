using UnityEngine;

namespace RenkaiMobile.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class MobilePlayerMotor : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4.6f;
        [SerializeField] private float gravity = -24f;
        [SerializeField] private float acceleration = 14f;
        [SerializeField] private float airControl = 0.45f;

        private CharacterController controller;
        private Vector2 touchMove;
        private Vector3 planarVelocity;
        private float verticalVelocity;

        public float PlanarSpeed => planarVelocity.magnitude;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Vector2 input = touchMove;
#if UNITY_EDITOR || UNITY_STANDALONE
            input += new Vector2(UnityEngine.Input.GetAxisRaw("Horizontal"), UnityEngine.Input.GetAxisRaw("Vertical"));
#endif
            input = Vector2.ClampMagnitude(input, 1f);

            Vector3 desired = (transform.right * input.x + transform.forward * input.y) * moveSpeed;
            float control = controller.isGrounded ? 1f : airControl;
            planarVelocity = Vector3.Lerp(planarVelocity, desired, 1f - Mathf.Exp(-acceleration * control * Time.deltaTime));

            if (controller.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -2f;
            verticalVelocity += gravity * Time.deltaTime;

            controller.Move((planarVelocity + Vector3.up * verticalVelocity) * Time.deltaTime);
        }

        public void SetMoveInput(Vector2 value)
        {
            touchMove = Vector2.ClampMagnitude(value, 1f);
        }
    }
}
