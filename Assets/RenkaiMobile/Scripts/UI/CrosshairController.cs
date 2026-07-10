using UnityEngine;

namespace RenkaiMobile.UI
{
    public sealed class CrosshairController : MonoBehaviour
    {
        [SerializeField] private RectTransform[] arms;
        [SerializeField] private float baseGap = 8f;
        [SerializeField] private float maxGap = 28f;
        [SerializeField] private float recovery = 10f;
        private float pulse;

        public void Pulse(float amount)
        {
            pulse = Mathf.Clamp01(pulse + amount);
        }

        private void Update()
        {
            pulse = Mathf.MoveTowards(pulse, 0f, recovery * Time.deltaTime);
            float gap = Mathf.Lerp(baseGap, maxGap, pulse);
            if (arms == null || arms.Length < 4) return;
            arms[0].anchoredPosition = new Vector2(-gap, 0f);
            arms[1].anchoredPosition = new Vector2(gap, 0f);
            arms[2].anchoredPosition = new Vector2(0f, gap);
            arms[3].anchoredPosition = new Vector2(0f, -gap);
        }
    }
}
