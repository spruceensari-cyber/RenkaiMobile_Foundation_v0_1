using UnityEngine;

namespace RenkaiMobile.Movement
{
    public sealed class MobileLookController : MonoBehaviour
    {
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private float editorSensitivity = 2.2f;
        [SerializeField] private float touchSensitivity = 120f;
        [SerializeField] private float maxPitch = 84f;

        private Vector2 touchLook;
        private float pitch;

        private void Awake()
        {
            if (cameraRoot == null)
            {
                Camera camera = GetComponentInChildren<Camera>(true);
                if (camera != null) cameraRoot = camera.transform;
            }
        }

        private void Update()
        {
            Vector2 look = touchLook * touchSensitivity * Time.deltaTime;
#if UNITY_EDITOR || UNITY_STANDALONE
            look += new Vector2(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y")) * editorSensitivity;
#endif
            touchLook = Vector2.zero;

            transform.Rotate(0f, look.x, 0f, Space.Self);
            pitch = Mathf.Clamp(pitch - look.y, -maxPitch, maxPitch);
            if (cameraRoot != null)
            {
                Vector3 euler = cameraRoot.localEulerAngles;
                cameraRoot.localRotation = Quaternion.Euler(pitch, euler.y, euler.z);
            }
        }

        public void AddLookInput(Vector2 delta)
        {
            touchLook += delta;
        }
    }
}
