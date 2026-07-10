using UnityEngine;

namespace RenkaiMobile.Combat
{
    public sealed class WeaponSway : MonoBehaviour
    {
        [SerializeField] private float positionAmount = 0.012f;
        [SerializeField] private float rotationAmount = 1.6f;
        [SerializeField] private float smooth = 10f;

        private Vector3 basePosition;
        private Quaternion baseRotation;

        private void Awake()
        {
            basePosition = transform.localPosition;
            baseRotation = transform.localRotation;
        }

        private void LateUpdate()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            float x = UnityEngine.Input.GetAxisRaw("Mouse X");
            float y = UnityEngine.Input.GetAxisRaw("Mouse Y");
#else
            float x = 0f;
            float y = 0f;
#endif
            Vector3 targetPos = basePosition + new Vector3(-x, -y, 0f) * positionAmount;
            Quaternion targetRot = baseRotation * Quaternion.Euler(y * rotationAmount, -x * rotationAmount, -x * rotationAmount * 0.4f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, smooth * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, smooth * Time.deltaTime);
        }
    }
}
