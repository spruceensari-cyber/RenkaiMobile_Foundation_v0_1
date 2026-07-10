using System;
using UnityEngine;
using RenkaiMobile.Core;

namespace RenkaiMobile.Rounds
{
    public enum MobileRoundPhase
    {
        Buy,
        Live,
        PostRound
    }

    public sealed class MobileRoundManager : MonoBehaviour
    {
        [SerializeField] private float buySeconds = 20f;
        [SerializeField] private float liveSeconds = 100f;
        [SerializeField] private float postRoundSeconds = 7f;

        public event Action<MobileRoundPhase> PhaseChanged;
        public event Action<MobileTeamId, int, int> ScoreChanged;

        public MobileRoundPhase Phase { get; private set; } = MobileRoundPhase.Buy;
        public float RemainingSeconds { get; private set; }
        public int AttackersScore { get; private set; }
        public int DefendersScore { get; private set; }

        private void Start()
        {
            EnterPhase(MobileRoundPhase.Buy);
        }

        private void Update()
        {
            RemainingSeconds = Mathf.Max(0f, RemainingSeconds - Time.deltaTime);
            if (RemainingSeconds > 0f) return;

            if (Phase == MobileRoundPhase.Buy)
                EnterPhase(MobileRoundPhase.Live);
            else if (Phase == MobileRoundPhase.Live)
                EndRound(MobileTeamId.Defenders);
            else
                EnterPhase(MobileRoundPhase.Buy);
        }

        public void EndRound(MobileTeamId winner)
        {
            if (Phase != MobileRoundPhase.Live || winner == MobileTeamId.None) return;

            if (winner == MobileTeamId.Attackers) AttackersScore++;
            else if (winner == MobileTeamId.Defenders) DefendersScore++;

            ScoreChanged?.Invoke(winner, AttackersScore, DefendersScore);
            EnterPhase(MobileRoundPhase.PostRound);
        }

        public void ForceLivePhase()
        {
            EnterPhase(MobileRoundPhase.Live);
        }

        private void EnterPhase(MobileRoundPhase phase)
        {
            Phase = phase;
            RemainingSeconds = phase == MobileRoundPhase.Buy
                ? buySeconds
                : phase == MobileRoundPhase.Live
                    ? liveSeconds
                    : postRoundSeconds;
            PhaseChanged?.Invoke(phase);
        }
    }
}
