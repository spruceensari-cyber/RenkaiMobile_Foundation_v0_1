using UnityEngine;
using RenkaiMobile.Combat;

namespace RenkaiMobile.UI
{
    public sealed class AdsCrosshairBridge : MonoBehaviour
    {
        [SerializeField] private MobileAdsController adsController;
        [SerializeField] private CrosshairController crosshair;

        private void Awake()
        {
            if (adsController == null) adsController = FindFirstObjectByType<MobileAdsController>();
            if (crosshair == null) crosshair = FindFirstObjectByType<CrosshairController>();
        }

        private void OnEnable()
        {
            if (adsController != null) adsController.AdsBlendChanged += OnAdsBlendChanged;
        }

        private void OnDisable()
        {
            if (adsController != null) adsController.AdsBlendChanged -= OnAdsBlendChanged;
        }

        private void OnAdsBlendChanged(float value)
        {
            crosshair?.SetAdsBlend(value);
        }
    }
}
