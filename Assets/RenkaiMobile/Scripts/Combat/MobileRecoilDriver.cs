using UnityEngine;
using RenkaiMobile.UI;
using RenkaiMobile.VFX;

namespace RenkaiMobile.Combat
{
    public sealed class MobileRecoilDriver : MonoBehaviour
    {
        [SerializeField] private DeterministicRecoilPattern pattern;
        [SerializeField] private MobileAdsController ads;
        [SerializeField] private MobileCameraImpulseAdapter cameraImpulse;
        [SerializeField] private HolographicWeaponSight holographicSight;
        [SerializeField] private CrosshairController crosshair;
        [SerializeField, Range(0f, 1f)] private float crosshairPulse = 0.18f;

        private void Awake()
        {
            if (pattern == null) pattern = GetComponent<DeterministicRecoilPattern>();
            if (ads == null) ads = GetComponent<MobileAdsController>();
            if (cameraImpulse == null) cameraImpulse = GetComponent<MobileCameraImpulseAdapter>();
            if (holographicSight == null) holographicSight = FindFirstObjectByType<HolographicWeaponSight>();
            if (crosshair == null) crosshair = FindFirstObjectByType<CrosshairController>();
        }

        public Vector2 RegisterShot()
        {
            if (pattern == null) return Vector2.zero;
            bool isAds = ads != null && ads.IsAds;
            Vector2 kick = pattern.NextKick(isAds);
            if (ads != null) kick *= ads.RecoilMultiplier;

            cameraImpulse?.Recoil(kick);
            holographicSight?.PulseShot(Mathf.Clamp01(kick.magnitude));
            crosshair?.Pulse(crosshairPulse * (isAds ? 0.55f : 1f));
            return kick;
        }

        public void ResetPattern()
        {
            pattern?.ResetPattern();
        }
    }
}
