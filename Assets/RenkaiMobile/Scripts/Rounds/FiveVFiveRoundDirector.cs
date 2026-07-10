using System.Collections.Generic;
using UnityEngine;
using RenkaiMobile.Core;
using RenkaiMobile.Teams;

namespace RenkaiMobile.Rounds
{
    public sealed class FiveVFiveRoundDirector : MonoBehaviour
    {
        [SerializeField] private MobileRoundManager roundManager;
        [SerializeField] private float eliminationCheckInterval = 0.25f;

        private readonly List<FiveVFiveRosterAgent> agents = new List<FiveVFiveRosterAgent>();
        private float nextCheck;

        private void Awake()
        {
            if (roundManager == null) roundManager = GetComponent<MobileRoundManager>();
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
            if (roundManager == null || roundManager.Phase != MobileRoundPhase.Live) return;
            if (Time.time < nextCheck) return;
            nextCheck = Time.time + eliminationCheckInterval;

            int attackersAlive = CountAlive(MobileTeamId.Attackers);
            int defendersAlive = CountAlive(MobileTeamId.Defenders);

            if (attackersAlive == 0 && defendersAlive > 0)
                roundManager.EndRound(MobileTeamId.Defenders);
            else if (defendersAlive == 0 && attackersAlive > 0)
                roundManager.EndRound(MobileTeamId.Attackers);
        }

        public void RefreshRoster()
        {
            agents.Clear();
            agents.AddRange(FindObjectsByType<FiveVFiveRosterAgent>(FindObjectsSortMode.None));
        }

        private int CountAlive(MobileTeamId team)
        {
            int count = 0;
            for (int i = 0; i < agents.Count; i++)
                if (agents[i] != null && agents[i].Team == team && agents[i].IsAlive) count++;
            return count;
        }

        private void OnPhaseChanged(MobileRoundPhase phase)
        {
            if (phase != MobileRoundPhase.Buy) return;
            RefreshRoster();
            for (int i = 0; i < agents.Count; i++)
                agents[i]?.ResetForRound();
        }
    }
}
