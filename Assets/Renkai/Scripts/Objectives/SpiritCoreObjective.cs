using System;
using UnityEngine;
using Renkai.Core;
using Renkai.Rounds;

namespace Renkai.Objectives
{
    public enum SpiritCoreState
    {
        Carried,
        Planting,
        Planted,
        Defusing,
        Defused,
        Detonated
    }

    public sealed class SpiritCoreObjective : MonoBehaviour
    {
        [SerializeField] private RoundManager roundManager;
        [SerializeField, Min(0.5f)] private float detonationSeconds = 45f;

        public event Action<SpiritCoreState> StateChanged;

        public SpiritCoreState State { get; private set; } = SpiritCoreState.Carried;
        public float RemainingSeconds { get; private set; }

        private void Update()
        {
            if (State != SpiritCoreState.Planted)
                return;

            RemainingSeconds -= Time.deltaTime;
            if (RemainingSeconds <= 0f)
            {
                State = SpiritCoreState.Detonated;
                StateChanged?.Invoke(State);
                roundManager?.EndRound(TeamId.Attackers);
            }
        }

        public void CompletePlant()
        {
            State = SpiritCoreState.Planted;
            RemainingSeconds = detonationSeconds;
            StateChanged?.Invoke(State);
        }

        public void CompleteDefuse()
        {
            if (State != SpiritCoreState.Planted)
                return;

            State = SpiritCoreState.Defused;
            StateChanged?.Invoke(State);
            roundManager?.EndRound(TeamId.Defenders);
        }

        public void ResetObjective()
        {
            State = SpiritCoreState.Carried;
            RemainingSeconds = 0f;
            StateChanged?.Invoke(State);
        }
    }
}
