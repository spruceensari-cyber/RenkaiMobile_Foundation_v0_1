using UnityEngine;

namespace RenkaiMobile.Map
{
    public sealed class KagamiAtmosphereController : MonoBehaviour
    {
        [Header("Rain")]
        [SerializeField] private ParticleSystem rain;
        [SerializeField] private float lowRainRate = 180f;
        [SerializeField] private float highRainRate = 520f;

        [Header("Transit")]
        [SerializeField] private Transform[] transitVehicles;
        [SerializeField] private Vector3 transitDirection = Vector3.forward;
        [SerializeField] private float transitSpeed = 8f;
        [SerializeField] private float transitLoopDistance = 120f;

        [Header("Synthetic Blossoms")]
        [SerializeField] private ParticleSystem blossoms;
        [SerializeField] private float blossomRate = 28f;

        [Header("Holographic Skyline")]
        [SerializeField] private Renderer[] skylineRenderers;
        [SerializeField] private float pulseSpeed = 0.7f;
        [SerializeField] private Color pulseA = new Color(0.05f, 0.35f, 0.7f, 1f);
        [SerializeField] private Color pulseB = new Color(0.7f, 0.1f, 0.8f, 1f);

        private MaterialPropertyBlock block;
        private Vector3[] transitStarts;

        private void Awake()
        {
            block = new MaterialPropertyBlock();
            transitStarts = transitVehicles != null ? new Vector3[transitVehicles.Length] : System.Array.Empty<Vector3>();
            for (int i = 0; i < transitStarts.Length; i++)
                if (transitVehicles[i] != null) transitStarts[i] = transitVehicles[i].position;
        }

        private void Start()
        {
            SetRainIntensity(0.7f);
            SetBlossomEmission(blossomRate);
        }

        private void Update()
        {
            UpdateTransit();
            UpdateSkylinePulse();
        }

        public void SetRainIntensity(float normalized)
        {
            if (rain == null) return;
            var emission = rain.emission;
            emission.rateOverTime = Mathf.Lerp(lowRainRate, highRainRate, Mathf.Clamp01(normalized));
            if (!rain.isPlaying) rain.Play();
        }

        public void SetBlossomEmission(float rate)
        {
            if (blossoms == null) return;
            var emission = blossoms.emission;
            emission.rateOverTime = Mathf.Max(0f, rate);
            if (!blossoms.isPlaying) blossoms.Play();
        }

        private void UpdateTransit()
        {
            if (transitVehicles == null) return;
            Vector3 direction = transitDirection.sqrMagnitude > 0.001f ? transitDirection.normalized : Vector3.forward;
            for (int i = 0; i < transitVehicles.Length; i++)
            {
                Transform vehicle = transitVehicles[i];
                if (vehicle == null) continue;
                vehicle.position += direction * transitSpeed * Time.deltaTime;
                if (Vector3.Distance(transitStarts[i], vehicle.position) >= transitLoopDistance)
                    vehicle.position = transitStarts[i];
            }
        }

        private void UpdateSkylinePulse()
        {
            if (skylineRenderers == null) return;
            float t = 0.5f + 0.5f * Mathf.Sin(Time.time * pulseSpeed);
            Color color = Color.Lerp(pulseA, pulseB, t);
            foreach (Renderer renderer in skylineRenderers)
            {
                if (renderer == null) continue;
                renderer.GetPropertyBlock(block);
                if (renderer.sharedMaterial != null && renderer.sharedMaterial.HasProperty("_BaseColor")) block.SetColor("_BaseColor", color);
                if (renderer.sharedMaterial != null && renderer.sharedMaterial.HasProperty("_Color")) block.SetColor("_Color", color);
                renderer.SetPropertyBlock(block);
            }
        }
    }
}
