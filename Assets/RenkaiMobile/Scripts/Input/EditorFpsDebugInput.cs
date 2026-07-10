using UnityEngine;
using Renkai.Combat;
using Renkai.Movement;
using RenkaiMobile.Combat;

namespace RenkaiMobile.Input
{
    public sealed class EditorFpsDebugInput : MonoBehaviour
    {
        [SerializeField] private MobileFpsMotor motor;
        [SerializeField] private Transform yawRoot;
        [SerializeField] private Transform pitchRoot;
        [SerializeField] private HitscanWeapon weapon;
        [SerializeField] private AdsController ads;
        [SerializeField] private float lookSensitivity = 2.2f;
        [SerializeField, Range(30f, 89f)] private float pitchLimit = 85f;

        private float pitch;

        private void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (motor != null)
            {
                Vector2 move = new Vector2(UnityEngine.Input.GetAxisRaw("Horizontal"), UnityEngine.Input.GetAxisRaw("Vertical"));
                motor.SetMoveInput(Vector2.ClampMagnitude(move, 1f));
                motor.SetCrouching(UnityEngine.Input.GetKey(KeyCode.LeftControl));
                if (UnityEngine.Input.GetKeyDown(KeyCode.Space)) motor.TryJump();
            }

            if (yawRoot != null && pitchRoot != null)
            {
                float mouseX = UnityEngine.Input.GetAxis("Mouse X") * lookSensitivity;
                float mouseY = UnityEngine.Input.GetAxis("Mouse Y") * lookSensitivity;
                yawRoot.Rotate(0f, mouseX, 0f, Space.Self);
                pitch = Mathf.Clamp(pitch - mouseY, -pitchLimit, pitchLimit);
                pitchRoot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            }

            if (weapon != null && UnityEngine.Input.GetMouseButton(0)) weapon.TryFire();
            if (weapon != null && UnityEngine.Input.GetKeyDown(KeyCode.R)) weapon.TryReload();
            if (ads != null) ads.SetAds(UnityEngine.Input.GetMouseButton(1));
#endif
        }
    }
}
