using UnityEngine;
using RenkaiMobile.Core;
using RenkaiMobile.Rounds;
using RenkaiMobile.Stats;

namespace RenkaiMobile.UI
{
    public sealed class MatchPresentationDirector : MonoBehaviour
    {
        [SerializeField] private MobileRoundManager roundManager;
        [SerializeField] private MobileMatchStateController matchStateController;
        [SerializeField] private MatchStatsTracker statsTracker;
        [SerializeField] private MvpScoreCalculator mvpCalculator;
        [SerializeField] private HolographicMvpPresenter mvpPresenter;
        [SerializeField, Min(1)] private int fallbackTargetScore = 7;

        private bool presented;

        private void Awake()
        {
            if (roundManager == null) roundManager = FindFirstObjectByType<MobileRoundManager>();
            if (matchStateController == null) matchStateController = FindFirstObjectByType<MobileMatchStateController>();
            if (statsTracker == null) statsTracker = FindFirstObjectByType<MatchStatsTracker>();
            if (mvpCalculator == null) mvpCalculator = FindFirstObjectByType<MvpScoreCalculator>();
            if (mvpPresenter == null) mvpPresenter = FindFirstObjectByType<HolographicMvpPresenter>();
        }

        private void OnEnable()
        {
            if (matchStateController != null) matchStateController.MatchEnded += OnMatchEnded;
            else if (roundManager != null) roundManager.ScoreChanged += OnScoreChangedFallback;
        }

        private void OnDisable()
        {
            if (matchStateController != null) matchStateController.MatchEnded -= OnMatchEnded;
            if (roundManager != null) roundManager.ScoreChanged -= OnScoreChangedFallback;
        }

        private void OnMatchEnded(MobileTeamId winner)
        {
            PresentMvp();
        }

        private void OnScoreChangedFallback(MobileTeamId winner, int attackers, int defenders)
        {
            if (Mathf.Max(attackers, defenders) < fallbackTargetScore) return;
            PresentMvp();
        }

        private void PresentMvp()
        {
            if (presented) return;
            presented = true;
            if (statsTracker == null || mvpCalculator == null || mvpPresenter == null) return;
            PlayerMatchStats mvp = mvpCalculator.FindMvp(statsTracker.Stats);
            if (mvp != null) mvpPresenter.Show(mvp);
        }

        public void ResetPresentation()
        {
            presented = false;
            mvpPresenter?.Hide();
        }
    }
}
