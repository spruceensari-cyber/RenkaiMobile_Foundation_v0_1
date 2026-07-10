using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.Rounds;
using RenkaiMobile.Objective;

namespace RenkaiMobile.UI
{
    public sealed class MobileMatchHud : MonoBehaviour
    {
        [SerializeField] private MobileRoundManager roundManager;
        [SerializeField] private SpiritCoreRoundObjective objective;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text phaseText;
        [SerializeField] private Text timerText;
        [SerializeField] private Text objectiveText;

        private void Awake()
        {
            if (roundManager == null) roundManager = FindFirstObjectByType<MobileRoundManager>();
            if (objective == null) objective = FindFirstObjectByType<SpiritCoreRoundObjective>();
        }

        private void OnEnable()
        {
            if (roundManager != null)
            {
                roundManager.PhaseChanged += OnPhaseChanged;
                roundManager.ScoreChanged += OnScoreChanged;
            }
            if (objective != null)
            {
                objective.Planted += OnPlanted;
                objective.Defused += OnDefused;
                objective.Detonated += OnDetonated;
            }
        }

        private void OnDisable()
        {
            if (roundManager != null)
            {
                roundManager.PhaseChanged -= OnPhaseChanged;
                roundManager.ScoreChanged -= OnScoreChanged;
            }
            if (objective != null)
            {
                objective.Planted -= OnPlanted;
                objective.Defused -= OnDefused;
                objective.Detonated -= OnDetonated;
            }
        }

        private void Update()
        {
            if (timerText == null || roundManager == null) return;
            float seconds = objective != null && objective.IsPlanted
                ? objective.RemainingDetonationSeconds
                : roundManager.RemainingSeconds;
            int whole = Mathf.CeilToInt(Mathf.Max(0f, seconds));
            timerText.text = (whole / 60).ToString("00") + ":" + (whole % 60).ToString("00");
        }

        private void OnPhaseChanged(MobileRoundPhase phase)
        {
            if (phaseText != null) phaseText.text = phase.ToString().ToUpperInvariant();
            if (objectiveText != null && phase == MobileRoundPhase.Buy) objectiveText.text = "BUY PHASE";
        }

        private void OnScoreChanged(RenkaiMobile.Core.MobileTeamId winner, int attackers, int defenders)
        {
            if (scoreText != null) scoreText.text = attackers + "  -  " + defenders;
        }

        private void OnPlanted() { if (objectiveText != null) objectiveText.text = "SPIRIT CORE PLANTED"; }
        private void OnDefused() { if (objectiveText != null) objectiveText.text = "SPIRIT CORE DEFUSED"; }
        private void OnDetonated() { if (objectiveText != null) objectiveText.text = "SPIRIT CORE DETONATED"; }
    }
}
