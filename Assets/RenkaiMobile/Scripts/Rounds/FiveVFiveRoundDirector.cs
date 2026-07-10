using System.Collections.Generic;
using UnityEngine;
using Renkai.Core;
using Renkai.Rounds;
using RenkaiMobile.Teams;

namespace RenkaiMobile.Rounds
{
    public sealed class FiveVFiveRoundDirector : MonoBehaviour
    {
        [SerializeField] private RoundManager roundManager;
        [SerializeField] private float eliminationCheckInterval = 0.25f;
        private readonly List<FiveVFiveRosterAgent> agents = new List<FiveVFiveRosterAgent>();
        private float nextCheck;

        private void Awake()
        {
            if (roundManager == null) roundManager = GetComponent<RoundManager>();
            RefreshRoster();
        }

        private void OnEnable()
        {
            if (roundManager != null) roundManager.PhaseChanged += OnPhaseChanged;
        }

        private void OnDisable()
        {
            if (roundManager != null) roundManager.PhaseChanged -= OnPhaseChanged;
        }

        private void Update()
        {
            if (roundManager == null || roundManager.Phase != RoundPhase.Live) return;
            if (Time.time < nextCheck) return;
            nextCheck = Time.time + eliminationCheckInterval;

            int attackersAlive = CountAlive(TeamId.Attackers);
            int defendersAlive = CountAlive(TeamId.Defenders);

            if (attackersAlive == 0 && defendersAlive > 0)
                roundManager.EndRound(TeamId.Defenders);
            else if (defendersAlive == 0 && attackersAlive > 0)
                roundManager.EndRound(TeamId.Attackers);
        }

        public void RefreshRoster()
        {
            agents.Clear();
            agents.AddRange(FindObjectsByType<FiveVFiveRosterAgent>(FindObjectsSortMode.None));
        }

        private int CountAlive(TeamId team)
        {
            int count = 0;
            for (int i = 0; i < agents.Count; i++)
                if (agents[i] != null && agents[i].Team == team && agents[i].IsAlive) count++;
            return count;
        }

        private void OnPhaseChanged(RoundPhase phase)
        {
            if (phase != RoundPhase.Buy) return;
            RefreshRoster();
            for (int i = 0; i < agents.Count; i++)
                agents[i]?.ResetForRound();
        }
    }
}
