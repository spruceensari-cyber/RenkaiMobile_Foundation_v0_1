using UnityEngine;
using RenkaiMobile.Core;
using RenkaiMobile.Rounds;
using RenkaiMobile.Stats;

namespace RenkaiMobile.UI
{
    public sealed class MatchPresentationDirector : MonoBehaviour
    {
        [SerializeField] private MobileRoundManager roundManager;
        [SerializeField] private MatchStatsTracker statsTracker;
        [SerializeField] private MvpScoreCalculator mvpCalculator;
        [SerializeField] private HolographicMvpPresenter mvpPresenter;
        [SerializeField, Min(1)] private int targetScore = 7;

        private bool presented;

        private void Awake()
        {
            if (roundManager == null) roundManager = FindFirstObjectByType<MobileRoundManager>();
            if (statsTracker == null) statsTracker = FindFirstObjectByType<MatchStatsTracker>();
            if (mvpCalculator == null) mvpCalculator = FindFirstObjectByType<MvpScoreCalculator>();
            if (mvpPresenter == null) mvpPresenter = FindFirstObjectByType<HolographicMvpPresenter>();
        }

        private void OnEnable()
        {
            if (roundManager != null) roundManager.ScoreChanged += OnScoreChanged;
        }

        private void OnDisable()
        {
            if (roundManager != null) roundManager.ScoreChanged -= OnScoreChanged;
        }

        private void OnScoreChanged(MobileTeamId winner, int attackers, int defenders)
        {
            if (presented || Mathf.Max(attackers, defenders) < targetScore) return;
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
