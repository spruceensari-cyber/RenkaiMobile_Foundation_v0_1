using System;
using UnityEngine;

namespace RenkaiMobile.Combat
{
    public enum MobileAdsInputMode
    {
        Hold,
        Toggle
    }

    public sealed class MobileAdsController : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform weaponViewmodel;
        [SerializeField] private MobileAimProfile profile;
        [SerializeField] private MobileAdsInputMode inputMode = MobileAdsInputMode.Hold;
        [SerializeField] private bool editorRightMouseFallback = true;

        public event Action<bool> AdsChanged;
        public event Action<float> AdsBlendChanged;

        public bool IsAds { get; private set; }
        public float Blend { get; private set; }
        public float SensitivityMultiplier => Mathf.Lerp(1f, profile != null ? profile.adsSensitivityMultiplier : 0.62f, Blend);
        public float RecoilMultiplier => Mathf.Lerp(1f, profile != null ? profile.adsRecoilMultiplier : 0.72f, Blend);
        public float SpreadMultiplier => Mathf.Lerp(1f, profile != null ? profile.adsSpreadMultiplier : 0.45f, Blend);
        public float SwayMultiplier => Mathf.Lerp(1f, profile != null ? profile.adsSwayMultiplier : 0.35f, Blend);

        private void Awake()
        {
            if (playerCamera == null) playerCamera = Camera.main;
        }

        private void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (editorRightMouseFallback)
            {
                if (inputMode == MobileAdsInputMode.Hold)
                    SetAds(UnityEngine.Input.GetMouseButton(1));
                else if (UnityEngine.Input.GetMouseButtonDown(1))
                    ToggleAds();
            }
#endif
            UpdateBlend();
        }

        public void SetAds(bool value)
        {
            if (inputMode == MobileAdsInputMode.Toggle && value)
            {
                ToggleAds();
                return;
            }

            if (inputMode == MobileAdsInputMode.Toggle) return;
            ApplyAdsState(value);
        }

        public void ToggleAds()
        {
            ApplyAdsState(!IsAds);
        }

        public void CancelAds()
        {
            ApplyAdsState(false);
        }

        private void ApplyAdsState(bool value)
        {
            if (IsAds == value) return;
            IsAds = value;
            AdsChanged?.Invoke(IsAds);
        }

        private void UpdateBlend()
        {
            float inSeconds = profile != null ? Mathf.Max(0.01f, profile.transitionInSeconds) : 0.16f;
            float outSeconds = profile != null ? Mathf.Max(0.01f, profile.transitionOutSeconds) : 0.12f;
            float speed = 1f / (IsAds ? inSeconds : outSeconds);
            float target = IsAds ? 1f : 0f;
            Blend = Mathf.MoveTowards(Blend, target, speed * Time.deltaTime);

            if (playerCamera != null)
            {
                float hip = profile != null ? profile.hipFov : 72f;
                float ads = profile != null ? profile.adsFov : 54f;
                playerCamera.fieldOfView = Mathf.Lerp(hip, ads, Smooth01(Blend));
            }

            if (weaponViewmodel != null && profile != null)
            {
                float t = Smooth01(Blend);
                weaponViewmodel.localPosition = Vector3.Lerp(profile.hipLocalPosition, profile.adsLocalPosition, t);
                Quaternion hip = Quaternion.Euler(profile.hipLocalEuler);
                Quaternion ads = Quaternion.Euler(profile.adsLocalEuler);
                weaponViewmodel.localRotation = Quaternion.Slerp(hip, ads, t);
            }

            AdsBlendChanged?.Invoke(Blend);
        }

        private static float Smooth01(float value)
        {
            value = Mathf.Clamp01(value);
            return value * value * (3f - 2f * value);
        }
    }
}
