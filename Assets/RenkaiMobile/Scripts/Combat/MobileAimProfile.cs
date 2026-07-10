using UnityEngine;

namespace RenkaiMobile.Combat
{
    [CreateAssetMenu(menuName = "Renkai Mobile/Combat/Mobile Aim Profile", fileName = "AimProfile_")]
    public sealed class MobileAimProfile : ScriptableObject
    {
        [Header("Camera")]
        public float hipFov = 72f;
        public float adsFov = 54f;
        public float transitionInSeconds = 0.16f;
        public float transitionOutSeconds = 0.12f;

        [Header("Handling")]
        [Range(0.1f, 1f)] public float adsSensitivityMultiplier = 0.62f;
        [Range(0.1f, 1f)] public float adsRecoilMultiplier = 0.72f;
        [Range(0.1f, 1f)] public float adsSpreadMultiplier = 0.45f;
        [Range(0.1f, 1f)] public float adsSwayMultiplier = 0.35f;

        [Header("Viewmodel")]
        public Vector3 hipLocalPosition = new Vector3(0.18f, -0.16f, 0.42f);
        public Vector3 adsLocalPosition = new Vector3(0f, -0.09f, 0.3f);
        public Vector3 hipLocalEuler;
        public Vector3 adsLocalEuler;
    }
}
