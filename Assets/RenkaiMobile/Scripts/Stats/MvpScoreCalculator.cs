using System.Collections.Generic;
using UnityEngine;

namespace RenkaiMobile.Stats
{
    public sealed class MvpScoreCalculator : MonoBehaviour
    {
        [SerializeField] private float killWeight = 100f;
        [SerializeField] private float assistWeight = 45f;
        [SerializeField] private float headshotWeight = 20f;
        [SerializeField] private float activationWeight = 140f;
        [SerializeField] private float disruptionWeight = 160f;
        [SerializeField] private float clutchWeight = 220f;
        [SerializeField] private float firstBloodWeight = 70f;
        [SerializeField] private float damageWeight = 0.15f;
        [SerializeField] private float survivalWeight = 25f;

        public float Calculate(PlayerMatchStats stats)
        {
            if (stats == null) return float.MinValue;
            return stats.kills * killWeight
                + stats.assists * assistWeight
                + stats.headshots * headshotWeight
                + stats.zodiacActivations * activationWeight
                + stats.zodiacDisruptions * disruptionWeight
                + stats.clutchWins * clutchWeight
                + stats.firstBloods * firstBloodWeight
                + stats.damageDealt * damageWeight
                + stats.roundsSurvived * survivalWeight;
        }

        public PlayerMatchStats FindMvp(IReadOnlyList<PlayerMatchStats> allStats)
        {
            PlayerMatchStats best = null;
            float bestScore = float.MinValue;
            if (allStats == null) return null;

            foreach (PlayerMatchStats stats in allStats)
            {
                float score = Calculate(stats);
                if (score <= bestScore) continue;
                bestScore = score;
                best = stats;
            }

            return best;
        }
    }
}
