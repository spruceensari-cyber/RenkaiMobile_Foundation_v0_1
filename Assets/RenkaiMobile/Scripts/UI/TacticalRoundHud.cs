using UnityEngine;
using UnityEngine.UI;
using Renkai.Rounds;
using RenkaiMobile.Objective;

namespace RenkaiMobile.UI
{
    public sealed class TacticalRoundHud : MonoBehaviour
    {
        [SerializeField] private RoundManager roundManager;
        [SerializeField] private SpiritCoreRoundObjective objective;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text phaseText;
        [SerializeField] private Text timerText;
        [SerializeField] private Text objectiveText;

        private void Awake()
        {
            if (roundManager == null) roundManager = FindFirstObjectByType<RoundManager>();
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
            if (roundManager != null && timerText != null)
            {
                float seconds = objective != null && objective.IsPlanted
                    ? objective.RemainingDetonationSeconds
                    : roundManager.RemainingSeconds;
                int whole = Mathf.CeilToInt(Mathf.Max(0f, seconds));
                timerText.text = (whole / 60).ToString("00") + ":" + (whole % 60).ToString("00");
            }
        }

        private void OnPhaseChanged(RoundPhase phase)
        {
            if (phaseText != null) phaseText.text = phase.ToString().ToUpperInvariant();
            if (objectiveText != null && phase == RoundPhase.Buy) objectiveText.text = "BUY PHASE";
        }

        private void OnScoreChanged(Renkai.Core.TeamId winner, int attackers, int defenders)
        {
            if (scoreText != null) scoreText.text = attackers + "  -  " + defenders;
        }

        private void OnPlanted() { if (objectiveText != null) objectiveText.text = "SPIRIT CORE PLANTED"; }
        private void OnDefused() { if (objectiveText != null) objectiveText.text = "SPIRIT CORE DEFUSED"; }
        private void OnDetonated() { if (objectiveText != null) objectiveText.text = "SPIRIT CORE DETONATED"; }
    }
}
