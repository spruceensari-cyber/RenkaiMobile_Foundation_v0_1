using UnityEngine;

namespace RenkaiMobile.Presentation
{
    public sealed class MainMenuWorldSceneController : MonoBehaviour
    {
        [SerializeField] private Transform cameraRig;
        [SerializeField] private Transform focusTarget;
        [SerializeField] private Transform[] hologramElements;
        [SerializeField] private float orbitRadius = 5.5f;
        [SerializeField] private float orbitHeight = 1.8f;
        [SerializeField] private float orbitSpeed = 4f;
        [SerializeField] private float hologramSpinSpeed = 10f;
        [SerializeField] private float breatheAmount = 0.06f;
        [SerializeField] private float breatheSpeed = 1.4f;

        private void Update()
        {
            if (cameraRig != null && focusTarget != null)
            {
                float angle = Time.unscaledTime * orbitSpeed * Mathf.Deg2Rad;
                Vector3 local = new Vector3(Mathf.Sin(angle) * orbitRadius, orbitHeight, Mathf.Cos(angle) * orbitRadius);
                cameraRig.position = focusTarget.position + local;
                Vector3 look = focusTarget.position + Vector3.up * 1.2f;
                cameraRig.rotation = Quaternion.LookRotation(look - cameraRig.position, Vector3.up);
            }

            if (hologramElements == null) return;
            float scale = 1f + Mathf.Sin(Time.unscaledTime * breatheSpeed) * breatheAmount;
            foreach (Transform element in hologramElements)
            {
                if (element == null) continue;
                element.Rotate(0f, hologramSpinSpeed * Time.unscaledDeltaTime, 0f, Space.Self);
                element.localScale = Vector3.one * scale;
            }
        }
    }
}
