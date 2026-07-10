using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.Objective;

namespace RenkaiMobile.UI
{
    public sealed class ZodiacObjectiveHud : MonoBehaviour
    {
        [SerializeField] private ZodiacObjective objective;
        [SerializeField] private Text stateText;
        [SerializeField] private Text timerText;
        [SerializeField] private Image progressFill;

        private void Awake()
        {
            if (objective == null) objective = FindFirstObjectByType<ZodiacObjective>();
        }

        private void OnEnable()
        {
            if (objective == null) return;
            objective.StateChanged += OnStateChanged;
            objective.InteractionProgressChanged += OnProgressChanged;
            OnStateChanged(objective.State);
        }

        private void OnDisable()
        {
            if (objective == null) return;
            objective.StateChanged -= OnStateChanged;
            objective.InteractionProgressChanged -= OnProgressChanged;
        }

        private void Update()
        {
            if (timerText == null || objective == null) return;
            timerText.text = objective.State == ZodiacState.Active
                ? Mathf.CeilToInt(objective.RemainingCollapseSeconds).ToString("00")
                : string.Empty;
        }

        private void OnStateChanged(ZodiacState state)
        {
            if (stateText == null) return;
            stateText.text = state switch
            {
                ZodiacState.Dormant => "ZODIAC DORMANT",
                ZodiacState.Carried => "ZODIAC LINKED",
                ZodiacState.Activating => "ACTIVATING ZODIAC",
                ZodiacState.Active => "ZODIAC ONLINE",
                ZodiacState.Disrupting => "DISRUPTION STARTED",
                ZodiacState.Collapsed => "COLLAPSE",
                ZodiacState.Disrupted => "ZODIAC DISRUPTED",
                _ => state.ToString().ToUpperInvariant()
            };
        }

        private void OnProgressChanged(float current, float required)
        {
            if (progressFill == null) return;
            progressFill.fillAmount = required > 0f ? Mathf.Clamp01(current / required) : 0f;
        }
    }
}
