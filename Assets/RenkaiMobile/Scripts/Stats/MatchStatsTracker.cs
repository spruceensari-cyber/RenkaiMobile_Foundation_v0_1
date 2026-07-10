using System;
using System.Collections.Generic;
using UnityEngine;
using RenkaiMobile.Combat;
using RenkaiMobile.Events;
using RenkaiMobile.Objective;

namespace RenkaiMobile.Stats
{
    public sealed class MatchStatsTracker : MonoBehaviour
    {
        [SerializeField] private MobileKillEventBus killEventBus;
        [SerializeField] private CompetitiveMomentTracker momentTracker;
        [SerializeField] private ZodiacObjective zodiacObjective;

        public IReadOnlyList<PlayerMatchStats> Stats => stats;

        private readonly List<PlayerMatchStats> stats = new List<PlayerMatchStats>();
        private readonly Dictionary<MobileCombatantIdentity, PlayerMatchStats> lookup = new Dictionary<MobileCombatantIdentity, PlayerMatchStats>();
        private bool firstBloodAwarded;

        private void Awake()
        {
            if (killEventBus == null) killEventBus = FindFirstObjectByType<MobileKillEventBus>();
            if (momentTracker == null) momentTracker = FindFirstObjectByType<CompetitiveMomentTracker>();
            if (zodiacObjective == null) zodiacObjective = FindFirstObjectByType<ZodiacObjective>();
            CacheCombatants();
        }

        private void OnEnable()
        {
            if (killEventBus != null) killEventBus.KillPublished += OnKill;
            if (momentTracker != null) momentTracker.ClutchWon += OnClutchWon;
            if (zodiacObjective != null) zodiacObjective.StateChanged += OnZodiacStateChanged;
        }

        private void OnDisable()
        {
            if (killEventBus != null) killEventBus.KillPublished -= OnKill;
            if (momentTracker != null) momentTracker.ClutchWon -= OnClutchWon;
            if (zodiacObjective != null) zodiacObjective.StateChanged -= OnZodiacStateChanged;
        }

        public PlayerMatchStats GetStats(MobileCombatantIdentity identity)
        {
            return identity != null && lookup.TryGetValue(identity, out PlayerMatchStats value) ? value : null;
        }

        private void CacheCombatants()
        {
            stats.Clear();
            lookup.Clear();
            MobileCombatantIdentity[] identities = FindObjectsByType<MobileCombatantIdentity>(FindObjectsSortMode.None);
            foreach (MobileCombatantIdentity identity in identities)
            {
                if (identity == null) continue;
                PlayerMatchStats playerStats = new PlayerMatchStats { identity = identity };
                stats.Add(playerStats);
                lookup[identity] = playerStats;
            }
        }

        private void OnKill(MobileKillEvent kill)
        {
            if (kill.Victim != null && lookup.TryGetValue(kill.Victim, out PlayerMatchStats victimStats))
                victimStats.deaths++;

            if (kill.Killer == null || !lookup.TryGetValue(kill.Killer, out PlayerMatchStats killerStats)) return;
            killerStats.kills++;
            if (kill.Headshot) killerStats.headshots++;
            if (!firstBloodAwarded)
            {
                firstBloodAwarded = true;
                killerStats.firstBloods++;
            }
        }

        private void OnClutchWon(GameObject actor, int enemiesAtStart)
        {
            if (actor == null) return;
            MobileCombatantIdentity identity = actor.GetComponentInParent<MobileCombatantIdentity>();
            if (identity != null && lookup.TryGetValue(identity, out PlayerMatchStats playerStats))
                playerStats.clutchWins++;
        }

        private void OnZodiacStateChanged(ZodiacState state)
        {
            if (state != ZodiacState.Active && state != ZodiacState.Disrupted) return;
            ZodiacCarrier[] carriers = FindObjectsByType<ZodiacCarrier>(FindObjectsSortMode.None);
            foreach (ZodiacCarrier carrier in carriers)
            {
                if (carrier == null) continue;
                MobileCombatantIdentity identity = carrier.GetComponentInParent<MobileCombatantIdentity>();
                if (identity == null || !lookup.TryGetValue(identity, out PlayerMatchStats playerStats)) continue;
                if (state == ZodiacState.Active && carrier.HasZodiac == false) playerStats.zodiacActivations++;
                else if (state == ZodiacState.Disrupted) playerStats.zodiacDisruptions++;
            }
        }
    }
}
