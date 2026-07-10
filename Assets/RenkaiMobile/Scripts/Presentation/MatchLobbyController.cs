using System;
using System.Collections.Generic;
using UnityEngine;
using RenkaiMobile.Combat;

namespace RenkaiMobile.Presentation
{
    public sealed class MatchLobbyController : MonoBehaviour
    {
        public event Action<int, int> ReadyCountChanged;
        public event Action AllPlayersReady;

        private readonly HashSet<string> readyCombatants = new HashSet<string>();
        private readonly List<MobileCombatantIdentity> combatants = new List<MobileCombatantIdentity>();

        public int ReadyCount => readyCombatants.Count;
        public int TotalCount => combatants.Count;

        private void Awake()
        {
            RefreshCombatants();
        }

        public void RefreshCombatants()
        {
            combatants.Clear();
            MobileCombatantIdentity[] found = FindObjectsByType<MobileCombatantIdentity>(FindObjectsSortMode.None);
            foreach (MobileCombatantIdentity identity in found)
                if (identity != null) combatants.Add(identity);
            ReadyCountChanged?.Invoke(ReadyCount, TotalCount);
        }

        public void SetReady(MobileCombatantIdentity identity, bool ready)
        {
            if (identity == null) return;
            if (ready) readyCombatants.Add(identity.CombatantId);
            else readyCombatants.Remove(identity.CombatantId);

            ReadyCountChanged?.Invoke(ReadyCount, TotalCount);
            if (TotalCount > 0 && ReadyCount >= TotalCount) AllPlayersReady?.Invoke();
        }

        public void MarkBotsReady()
        {
            foreach (MobileCombatantIdentity identity in combatants)
                if (identity != null && !identity.PlayerControlled) readyCombatants.Add(identity.CombatantId);
            ReadyCountChanged?.Invoke(ReadyCount, TotalCount);
        }
    }
}
