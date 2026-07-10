using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RenkaiMobile.UI
{
    public sealed class CrosshairController : MonoBehaviour
    {
        [SerializeField] private RectTransform[] arms;
        [SerializeField] private float baseGap = 8f;
        [SerializeField] private float maxGap = 28f;
        [SerializeField] private float recovery = 10f;
        [SerializeField] private CanvasGroup hitMarker;
        [SerializeField] private CanvasGroup headshotMarker;
        [SerializeField] private Color restingColor = new Color(0.45f, 0.9f, 1f, 0.9f);
        [SerializeField] private Color firingColor = Color.white;
        private float pulse;
        private Coroutine markerRoutine;

        public void Pulse(float amount)
        {
            pulse = Mathf.Clamp01(pulse + amount);
        }

        public void ShowHitMarker(bool headshot)
        {
            if (markerRoutine != null)
                StopCoroutine(markerRoutine);

            markerRoutine = StartCoroutine(ShowMarkerRoutine(headshot));
        }

        private void Update()
        {
            pulse = Mathf.MoveTowards(pulse, 0f, recovery * Time.deltaTime);
            float gap = Mathf.Lerp(baseGap, maxGap, pulse);
            if (arms == null || arms.Length < 4) return;
            Color armColor = Color.Lerp(restingColor, firingColor, pulse);
            for (int i = 0; i < arms.Length; i++)
            {
                if (arms[i] == null) continue;
                arms[i].anchoredPosition = i switch
                {
                    0 => new Vector2(-gap, 0f),
                    1 => new Vector2(gap, 0f),
                    2 => new Vector2(0f, gap),
                    _ => new Vector2(0f, -gap)
                };

                Image image = arms[i].GetComponent<Image>();
                if (image != null) image.color = armColor;
            }
        }

        private IEnumerator ShowMarkerRoutine(bool headshot)
        {
            CanvasGroup active = headshot ? headshotMarker : hitMarker;
            CanvasGroup inactive = headshot ? hitMarker : headshotMarker;
            if (inactive != null) inactive.alpha = 0f;
            if (active == null) yield break;

            active.alpha = 1f;
            float duration = headshot ? 0.2f : 0.12f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                active.alpha = 1f - elapsed / duration;
                yield return null;
            }

            active.alpha = 0f;
            markerRoutine = null;
        }
    }
}
