using UnityEngine;
using UnityEngine.UI;

namespace RenkaiMobile.UI
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class HolographicHudPanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Graphic[] glowGraphics;
        [SerializeField] private float parallaxAmount = 4f;
        [SerializeField] private float pulseSpeed = 1.8f;
        [SerializeField] private float minAlpha = 0.82f;
        [SerializeField] private float maxAlpha = 0.98f;
        [SerializeField] private Color baseGlow = new Color(0.1f, 0.75f, 1f, 0.9f);
        [SerializeField] private Color pulseGlow = new Color(0.7f, 0.2f, 1f, 1f);

        private RectTransform rect;
        private Vector2 basePosition;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
            basePosition = rect.anchoredPosition;
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Update()
        {
            float pulse = 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * pulseSpeed);
            if (canvasGroup != null) canvasGroup.alpha = Mathf.Lerp(minAlpha, maxAlpha, pulse);

            Vector2 mouse = Vector2.zero;
#if UNITY_EDITOR || UNITY_STANDALONE
            mouse = new Vector2(Input.mousePosition.x / Mathf.Max(1f, Screen.width), Input.mousePosition.y / Mathf.Max(1f, Screen.height));
            mouse = (mouse - Vector2.one * 0.5f) * 2f;
#endif
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, basePosition + mouse * parallaxAmount, 8f * Time.unscaledDeltaTime);

            if (glowGraphics == null) return;
            Color color = Color.Lerp(baseGlow, pulseGlow, pulse * 0.25f);
            foreach (Graphic graphic in glowGraphics)
                if (graphic != null) graphic.color = color;
        }
    }
}
