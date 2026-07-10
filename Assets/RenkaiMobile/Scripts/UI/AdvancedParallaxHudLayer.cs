using UnityEngine;

namespace RenkaiMobile.UI
{
    public sealed class AdvancedParallaxHudLayer : MonoBehaviour
    {
        [SerializeField] private RectTransform[] depthLayers;
        [SerializeField] private float[] depthAmounts = { 1f, 2.5f, 4.5f };
        [SerializeField] private float response = 6f;
        [SerializeField] private float idleWave = 1.2f;

        private Vector2[] basePositions;

        private void Awake()
        {
            if (depthLayers == null) depthLayers = System.Array.Empty<RectTransform>();
            basePositions = new Vector2[depthLayers.Length];
            for (int i = 0; i < depthLayers.Length; i++)
                if (depthLayers[i] != null) basePositions[i] = depthLayers[i].anchoredPosition;
        }

        private void Update()
        {
            Vector2 input = Vector2.zero;
#if UNITY_EDITOR || UNITY_STANDALONE
            input.x = (UnityEngine.Input.mousePosition.x / Mathf.Max(1f, Screen.width) - 0.5f) * 2f;
            input.y = (UnityEngine.Input.mousePosition.y / Mathf.Max(1f, Screen.height) - 0.5f) * 2f;
#endif
            input.y += Mathf.Sin(Time.unscaledTime * idleWave) * 0.08f;

            for (int i = 0; i < depthLayers.Length; i++)
            {
                RectTransform layer = depthLayers[i];
                if (layer == null) continue;
                float amount = i < depthAmounts.Length ? depthAmounts[i] : 1f + i;
                Vector2 target = basePositions[i] + input * amount;
                layer.anchoredPosition = Vector2.Lerp(layer.anchoredPosition, target, 1f - Mathf.Exp(-response * Time.unscaledDeltaTime));
            }
        }
    }
}
