using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.Core;
using RenkaiMobile.Rounds;

namespace RenkaiMobile.Presentation
{
    public sealed class RoundTransitionPresenter : MonoBehaviour
    {
        [SerializeField] private MobileRoundManager roundManager;
        [SerializeField] private MobileMatchStateController matchStateController;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private Text primaryText;
        [SerializeField] private Text secondaryText;
        [SerializeField] private float visibleSeconds = 1.8f;

        private Coroutine routine;

        private void Awake()
        {
            if (roundManager == null) roundManager = FindFirstObjectByType<MobileRoundManager>();
            if (matchStateController == null) matchStateController = FindFirstObjectByType<MobileMatchStateController>();
            if (group != null) group.alpha = 0f;
        }

        private void OnEnable()
        {
            if (roundManager != null)
            {
                roundManager.PhaseChanged += OnPhaseChanged;
                roundManager.ScoreChanged += OnScoreChanged;
            }
            if (matchStateController != null) matchStateController.MatchStateChanged += OnMatchStateChanged;
        }

        private void OnDisable()
        {
            if (roundManager != null)
            {
                roundManager.PhaseChanged -= OnPhaseChanged;
                roundManager.ScoreChanged -= OnScoreChanged;
            }
            if (matchStateController != null) matchStateController.MatchStateChanged -= OnMatchStateChanged;
        }

        private void OnPhaseChanged(MobileRoundPhase phase)
        {
            if (phase == MobileRoundPhase.Buy) Show("SYNC PHASE", "ARMORY LINK ACTIVE");
            else if (phase == MobileRoundPhase.Live) Show("ROUND LIVE", "RESONANCE COMBAT ENABLED");
        }

        private void OnScoreChanged(MobileTeamId winner, int attackers, int defenders)
        {
            Show(winner == MobileTeamId.Attackers ? "ATTACKERS RESONATE" : "DEFENDERS HOLD", attackers + " // " + defenders);
        }

        private void OnMatchStateChanged(MobileMatchState state)
        {
            if (state == MobileMatchState.MatchPoint) Show("MATCH POINT", "ONE ROUND FROM COLLAPSE");
            else if (state == MobileMatchState.Overtime) Show("OVERTIME", "RESONANCE UNSTABLE");
            else if (state == MobileMatchState.MatchEnded) Show("MATCH COMPLETE", "FINAL RESONANCE LOCKED");
        }

        private void Show(string primary, string secondary)
        {
            if (primaryText != null) primaryText.text = primary;
            if (secondaryText != null) secondaryText.text = secondary;
            if (routine != null) StopCoroutine(routine);
            routine = StartCoroutine(ShowRoutine());
        }

        private IEnumerator ShowRoutine()
        {
            if (group != null) group.alpha = 1f;
            yield return new WaitForSecondsRealtime(visibleSeconds);
            if (group != null) group.alpha = 0f;
            routine = null;
        }
    }
}
