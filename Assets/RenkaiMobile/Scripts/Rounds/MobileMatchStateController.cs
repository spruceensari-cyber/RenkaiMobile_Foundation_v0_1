using System;
using UnityEngine;
using RenkaiMobile.Core;

namespace RenkaiMobile.Rounds
{
    public enum MobileMatchState
    {
        Warmup,
        Regulation,
        MatchPoint,
        Overtime,
        MatchEnded
    }

    public sealed class MobileMatchStateController : MonoBehaviour
    {
        [SerializeField] private MobileRoundManager roundManager;
        [SerializeField] private int regulationTargetScore = 7;
        [SerializeField] private int overtimeLeadRequired = 2;

        public event Action<MobileMatchState> MatchStateChanged;
        public event Action<MobileTeamId> MatchEnded;

        public MobileMatchState State { get; private set; } = MobileMatchState.Warmup;
        public MobileTeamId Winner { get; private set; } = MobileTeamId.None;

        private void Awake()
        {
            if (roundManager == null) roundManager = FindFirstObjectByType<MobileRoundManager>();
        }

        private void OnEnable()
        {
            if (roundManager != null) roundManager.ScoreChanged += OnScoreChanged;
        }

        private void OnDisable()
        {
            if (roundManager != null) roundManager.ScoreChanged -= OnScoreChanged;
        }

        private void Start()
        {
            SetState(MobileMatchState.Regulation);
        }

        private void OnScoreChanged(MobileTeamId roundWinner, int attackers, int defenders)
        {
            if (State == MobileMatchState.MatchEnded) return;

            bool regulationReached = attackers >= regulationTargetScore || defenders >= regulationTargetScore;
            if (!regulationReached)
            {
                if (attackers == regulationTargetScore - 1 || defenders == regulationTargetScore - 1)
                    SetState(MobileMatchState.MatchPoint);
                else
                    SetState(MobileMatchState.Regulation);
                return;
            }

            if (attackers == defenders)
            {
                SetState(MobileMatchState.Overtime);
                return;
            }

            int lead = Mathf.Abs(attackers - defenders);
            bool overtime = attackers >= regulationTargetScore && defenders >= regulationTargetScore;
            if (overtime && lead < overtimeLeadRequired)
            {
                SetState(MobileMatchState.Overtime);
                return;
            }

            Winner = attackers > defenders ? MobileTeamId.Attackers : MobileTeamId.Defenders;
            SetState(MobileMatchState.MatchEnded);
            MatchEnded?.Invoke(Winner);
        }

        private void SetState(MobileMatchState next)
        {
            if (State == next) return;
            State = next;
            MatchStateChanged?.Invoke(State);
        }
    }
}
