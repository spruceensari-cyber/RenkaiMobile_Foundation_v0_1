using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.Abilities;

namespace RenkaiMobile.UI
{
    public sealed class AbilityCooldownRing : MonoBehaviour
    {
        [SerializeField] private AbilityRuntimeController controller;
        [SerializeField, Range(0, 2)] private int slot;
        [SerializeField] private Image radialFill;
        [SerializeField] private Text cooldownText;
        [SerializeField] private CanvasGroup readyPulse;
        [SerializeField] private float readyPulseSpeed = 4f;

        private float knownMaxCooldown = 1f;

        private void Awake()
        {
            if (controller == null) controller = FindFirstObjectByType<AbilityRuntimeController>();
        }

        private void Update()
        {
            if (controller == null) return;
            float remaining = controller.RemainingCooldown(slot);
            if (remaining > knownMaxCooldown) knownMaxCooldown = remaining;
            float normalized = knownMaxCooldown > 0f ? Mathf.Clamp01(remaining / knownMaxCooldown) : 0f;

            if (radialFill != null) radialFill.fillAmount = normalized;
            if (cooldownText != null) cooldownText.text = remaining > 0.05f ? Mathf.CeilToInt(remaining).ToString() : string.Empty;
            if (readyPulse != null)
            {
                readyPulse.alpha = remaining <= 0.05f
                    ? 0.45f + 0.35f * (0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * readyPulseSpeed))
                    : 0f;
            }

            if (remaining <= 0.05f) knownMaxCooldown = 1f;
        }
    }
}
