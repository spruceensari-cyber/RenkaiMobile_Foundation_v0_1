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
        public event Action<GameObject, int> ClutchWon;
        public event Action<GameObject, int> ClutchLost;

        [SerializeField] private MobileRoundManager roundManager;
        [SerializeField] private float clutchScanInterval = 0.2f;

        private readonly Dictionary<GameObject, int> roundKills = new Dictionary<GameObject, int>();
        private readonly Dictionary<GameObject, int> headshotStreaks = new Dictionary<GameObject, int>();
        private readonly HashSet<GameObject> aceAwarded = new HashSet<GameObject>();
        private readonly Dictionary<GameObject, int> activeClutches = new Dictionary<GameObject, int>();
        private MobileHealth[] trackedHealth = Array.Empty<MobileHealth>();
        private FiveVFiveRosterAgent[] roster = Array.Empty<FiveVFiveRosterAgent>();
        private float nextClutchScan;

        private void Awake()
        {
            if (roundManager == null) roundManager = FindFirstObjectByType<MobileRoundManager>();
        }

        private void Start()
        {
            BindHealthEvents();
            CacheRoster();
        }

        private void OnEnable()
        {
            if (roundManager != null)
            {
                roundManager.PhaseChanged += OnPhaseChanged;
                roundManager.ScoreChanged += OnScoreChanged;
            }
        }

        private void OnDisable()
        {
            if (roundManager != null)
            {
                roundManager.PhaseChanged -= OnPhaseChanged;
                roundManager.ScoreChanged -= OnScoreChanged;
            }

            foreach (MobileHealth health in trackedHealth)
                if (health != null) health.Died -= OnCharacterDied;
        }

        private void Update()
        {
            if (roundManager == null || roundManager.Phase != MobileRoundPhase.Live) return;
            if (Time.time < nextClutchScan) return;
            nextClutchScan = Time.time + clutchScanInterval;
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

        private void CacheRoster()
        {
            roster = FindObjectsByType<FiveVFiveRosterAgent>(FindObjectsSortMode.None);
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

            if (kills >= 5 && aceAwarded.Add(killer))
                AceAchieved?.Invoke(killer);
        }

        private void DetectClutchStates()
        {
            foreach (FiveVFiveRosterAgent candidate in roster)
            {
                if (candidate == null || !candidate.IsAlive || activeClutches.ContainsKey(candidate.gameObject)) continue;

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
                    activeClutches[candidate.gameObject] = enemiesAlive;
                    ClutchStateEntered?.Invoke(candidate.gameObject, enemiesAlive);
                }
            }
        }

        private void OnScoreChanged(MobileTeamId winner, int attackers, int defenders)
        {
            foreach (var pair in activeClutches)
            {
                GameObject actor = pair.Key;
                if (actor == null) continue;
                MobileTeamMember member = actor.GetComponentInParent<MobileTeamMember>();
                bool won = member != null && member.Team == winner;
                if (won) ClutchWon?.Invoke(actor, pair.Value);
                else ClutchLost?.Invoke(actor, pair.Value);
            }
        }

        private void OnPhaseChanged(MobileRoundPhase phase)
        {
            if (phase != MobileRoundPhase.Buy) return;
            roundKills.Clear();
            headshotStreaks.Clear();
            aceAwarded.Clear();
            activeClutches.Clear();
            CacheRoster();
        }
    }
}
