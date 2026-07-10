using UnityEngine;
using RenkaiMobile.Combat;

namespace RenkaiMobile.VFX
{
    public sealed class HolographicWeaponSight : MonoBehaviour
    {
        [SerializeField] private MobileAdsController adsController;
        [SerializeField] private Transform outerRing;
        [SerializeField] private Transform middleRing;
        [SerializeField] private Transform innerReticle;
        [SerializeField] private Renderer[] glowRenderers;
        [SerializeField] private float outerSpinSpeed = 28f;
        [SerializeField] private float middleSpinSpeed = -46f;
        [SerializeField] private float idlePulseSpeed = 2.2f;
        [SerializeField] private float adsDepthTravel = 0.035f;
        [SerializeField] private float recoilJitterAmount = 0.012f;
        [SerializeField] private Color hipColor = new Color(0.1f, 0.75f, 1f, 0.72f);
        [SerializeField] private Color adsColor = new Color(0.75f, 0.2f, 1f, 0.95f);

        private Vector3 innerBasePosition;
        private float recoilNoise;
        private MaterialPropertyBlock block;

        private void Awake()
        {
            if (adsController == null) adsController = FindFirstObjectByType<MobileAdsController>();
            if (innerReticle != null) innerBasePosition = innerReticle.localPosition;
            block = new MaterialPropertyBlock();
        }

        private void Update()
        {
            float ads = adsController != null ? adsController.Blend : 0f;
            float pulse = 0.5f + 0.5f * Mathf.Sin(Time.time * idlePulseSpeed);

            if (outerRing != null)
                outerRing.Rotate(0f, 0f, outerSpinSpeed * Time.deltaTime * Mathf.Lerp(0.4f, 1f, ads), Space.Self);
            if (middleRing != null)
                middleRing.Rotate(0f, 0f, middleSpinSpeed * Time.deltaTime * Mathf.Lerp(0.4f, 1f, ads), Space.Self);

            recoilNoise = Mathf.MoveTowards(recoilNoise, 0f, 5f * Time.deltaTime);
            if (innerReticle != null)
            {
                Vector3 depthOffset = Vector3.forward * adsDepthTravel * ads;
                Vector3 jitter = new Vector3(
                    Mathf.Sin(Time.time * 31f),
                    Mathf.Cos(Time.time * 27f),
                    0f) * recoilNoise * recoilJitterAmount;
                innerReticle.localPosition = innerBasePosition + depthOffset + jitter;
                float scale = Mathf.Lerp(0.92f + pulse * 0.03f, 1.04f + pulse * 0.02f, ads);
                innerReticle.localScale = Vector3.one * scale;
            }

            ApplyColor(Color.Lerp(hipColor, adsColor, ads));
        }

        public void PulseShot(float intensity = 1f)
        {
            recoilNoise = Mathf.Clamp01(recoilNoise + Mathf.Max(0f, intensity));
        }

        private void ApplyColor(Color color)
        {
            if (glowRenderers == null) return;
            foreach (Renderer renderer in glowRenderers)
            {
                if (renderer == null) continue;
                renderer.GetPropertyBlock(block);
                if (renderer.sharedMaterial != null && renderer.sharedMaterial.HasProperty("_BaseColor"))
                    block.SetColor("_BaseColor", color);
                if (renderer.sharedMaterial != null && renderer.sharedMaterial.HasProperty("_Color"))
                    block.SetColor("_Color", color);
                renderer.SetPropertyBlock(block);
            }
        }
    }
}
