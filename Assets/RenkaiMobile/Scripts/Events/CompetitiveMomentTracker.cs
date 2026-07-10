using System;
using System.Collections.Generic;
using UnityEngine;
using RenkaiMobile.Combat;
using RenkaiMobile.Core;
using RenkaiMobile.Rounds;
using RenkaiMobile.Teams;

namespace RenkaiMobile.Events
{
    public sealed class CompetitiveMomentTracker : MonoBehaviour
    {
        public event Action<GameObject, int> KillStreakChanged;
        public event Action<GameObject, int> HeadshotStreakChanged;
        public event Action<GameObject> AceAchieved;
        public event Action<GameObject, int> ClutchStateEntered;

        [SerializeField] private MobileRoundManager roundManager;

        private readonly Dictionary<GameObject, int> roundKills = new Dictionary<GameObject, int>();
        private readonly Dictionary<GameObject, int> headshotStreaks = new Dictionary<GameObject, int>();
        private readonly HashSet<GameObject> clutchAnnounced = new HashSet<GameObject>();
        private MobileHealth[] trackedHealth;

        private void Awake()
        {
            if (roundManager == null) roundManager = FindFirstObjectByType<MobileRoundManager>();
        }

        private void Start()
        {
            BindHealthEvents();
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
            DetectClutchStates();
        }

        private void BindHealthEvents()
        {
            trackedHealth = FindObjectsByType<MobileHealth>(FindObjectsSortMode.None);
            foreach (MobileHealth health in trackedHealth)
            {
                if (health == null) continue;
                health.Died += OnCharacterDied;
            }
        }

        private void OnCharacterDied(MobileDamageInfo damage)
        {
            GameObject killer = damage.Instigator;
            if (killer == null) return;

            int kills = roundKills.TryGetValue(killer, out int currentKills) ? currentKills + 1 : 1;
            roundKills[killer] = kills;
            KillStreakChanged?.Invoke(killer, kills);

            if (damage.Headshot)
            {
                int streak = headshotStreaks.TryGetValue(killer, out int currentStreak) ? currentStreak + 1 : 1;
                headshotStreaks[killer] = streak;
                HeadshotStreakChanged?.Invoke(killer, streak);
            }
            else
            {
                headshotStreaks[killer] = 0;
            }

            if (kills >= 5) AceAchieved?.Invoke(killer);
        }

        private void DetectClutchStates()
        {
            FiveVFiveRosterAgent[] roster = FindObjectsByType<FiveVFiveRosterAgent>(FindObjectsSortMode.None);
            foreach (FiveVFiveRosterAgent candidate in roster)
            {
                if (candidate == null || !candidate.IsAlive || clutchAnnounced.Contains(candidate.gameObject)) continue;

                int alliesAlive = 0;
                int enemiesAlive = 0;
                foreach (FiveVFiveRosterAgent agent in roster)
                {
                    if (agent == null || !agent.IsAlive) continue;
                    if (agent.Team == candidate.Team) alliesAlive++;
                    else if (agent.Team != MobileTeamId.None) enemiesAlive++;
                }

                if (alliesAlive == 1 && enemiesAlive >= 2)
                {
                    clutchAnnounced.Add(candidate.gameObject);
                    ClutchStateEntered?.Invoke(candidate.gameObject, enemiesAlive);
                }
            }
        }

        private void OnPhaseChanged(MobileRoundPhase phase)
        {
            if (phase != MobileRoundPhase.Buy) return;
            roundKills.Clear();
            headshotStreaks.Clear();
            clutchAnnounced.Clear();
        }
    }
}
