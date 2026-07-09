using System;
using UnityEngine;
using Renkai.Core;

namespace Renkai.Rounds
{
    public enum RoundPhase
    {
        Waiting,
        Buy,
        Live,
        PostRound,
        MatchEnded
    }

    public sealed class RoundManager : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float buyPhaseSeconds = 20f;
        [SerializeField, Min(10f)] private float roundSeconds = 100f;
        [SerializeField, Min(0f)] private float postRoundSeconds = 6f;
        [SerializeField, Min(1)] private int roundsToWin = 7;

        public event Action<RoundPhase> PhaseChanged;
        public event Action<TeamId, int, int> ScoreChanged;

        public RoundPhase Phase { get; private set; } = RoundPhase.Waiting;
        public float RemainingSeconds { get; private set; }
        public int AttackersScore { get; private set; }
        public int DefendersScore { get; private set; }

        private TeamId pendingWinner = TeamId.None;

        private void Start()
        {
            BeginBuyPhase();
        }

        private void Update()
        {
            if (Phase == RoundPhase.MatchEnded || Phase == RoundPhase.Waiting)
                return;

            RemainingSeconds -= Time.deltaTime;
            if (RemainingSeconds > 0f)
                return;

            switch (Phase)
            {
                case RoundPhase.Buy:
                    BeginLiveRound();
                    break;
                case RoundPhase.Live:
                    EndRound(TeamId.Defenders);
                    break;
                case RoundPhase.PostRound:
                    if (HasMatchWinner())
                        SetPhase(RoundPhase.MatchEnded, 0f);
                    else
                        BeginBuyPhase();
                    break;
            }
        }

        public void EndRound(TeamId winner)
        {
            if (Phase != RoundPhase.Live || winner == TeamId.None)
                return;

            pendingWinner = winner;

            if (winner == TeamId.Attackers)
                AttackersScore++;
            else if (winner == TeamId.Defenders)
                DefendersScore++;

            ScoreChanged?.Invoke(winner, AttackersScore, DefendersScore);
            SetPhase(RoundPhase.PostRound, postRoundSeconds);
        }

        private void BeginBuyPhase()
        {
            pendingWinner = TeamId.None;
            SetPhase(RoundPhase.Buy, buyPhaseSeconds);
        }

        private void BeginLiveRound()
        {
            SetPhase(RoundPhase.Live, roundSeconds);
        }

        private bool HasMatchWinner()
        {
            return AttackersScore >= roundsToWin || DefendersScore >= roundsToWin;
        }

        private void SetPhase(RoundPhase next, float seconds)
        {
            Phase = next;
            RemainingSeconds = seconds;
            PhaseChanged?.Invoke(next);
        }
    }
}
