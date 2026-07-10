using UnityEngine;

namespace RenkaiMobile.Combat
{
    public sealed class AdsController : MonoBehaviour
    {
        [SerializeField] private Camera gameplayCamera;
        [SerializeField] private float hipFov = 86f;
        [SerializeField] private float adsFov = 68f;
        [SerializeField] private float speed = 14f;
        private bool ads;

        public bool IsAds => ads;

        public void SetAds(bool value) => ads = value;

        private void Update()
        {
            if (gameplayCamera == null) return;
            float target = ads ? adsFov : hipFov;
            gameplayCamera.fieldOfView = Mathf.Lerp(gameplayCamera.fieldOfView, target, speed * Time.deltaTime);
        }
    }
}
