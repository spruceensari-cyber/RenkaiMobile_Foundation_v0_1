using UnityEngine;

namespace RenkaiMobile.Map
{
    public sealed class HologramSignAnimator : MonoBehaviour
    {
        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private float bobAmplitude = 0.08f;
        [SerializeField] private float bobSpeed = 1.6f;
        [SerializeField] private float yawSpeed = 8f;
        [SerializeField] private Color colorA = new Color(0.1f, 0.75f, 1f, 0.75f);
        [SerializeField] private Color colorB = new Color(0.75f, 0.2f, 1f, 0.85f);

        private Vector3 basePosition;
        private MaterialPropertyBlock block;

        private void Awake()
        {
            basePosition = transform.localPosition;
            if (targetRenderer == null) targetRenderer = GetComponentInChildren<Renderer>();
            block = new MaterialPropertyBlock();
        }

        private void Update()
        {
            float wave = 0.5f + 0.5f * Mathf.Sin(Time.time * bobSpeed);
            transform.localPosition = basePosition + Vector3.up * ((wave - 0.5f) * 2f * bobAmplitude);
            transform.Rotate(0f, yawSpeed * Time.deltaTime, 0f, Space.Self);

            if (targetRenderer == null) return;
            Color color = Color.Lerp(colorA, colorB, wave);
            targetRenderer.GetPropertyBlock(block);
            if (targetRenderer.sharedMaterial != null && targetRenderer.sharedMaterial.HasProperty("_BaseColor"))
                block.SetColor("_BaseColor", color);
            if (targetRenderer.sharedMaterial != null && targetRenderer.sharedMaterial.HasProperty("_Color"))
                block.SetColor("_Color", color);
            targetRenderer.SetPropertyBlock(block);
        }
    }
}
