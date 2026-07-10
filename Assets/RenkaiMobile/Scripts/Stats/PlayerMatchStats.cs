using System;
using UnityEngine;
using RenkaiMobile.Combat;

namespace RenkaiMobile.Stats
{
    [Serializable]
    public sealed class PlayerMatchStats
    {
        public MobileCombatantIdentity identity;
        public int kills;
        public int assists;
        public int deaths;
        public int headshots;
        public int zodiacActivations;
        public int zodiacDisruptions;
        public int clutchWins;
        public int firstBloods;
        public float damageDealt;
        public int roundsSurvived;
    }
}
