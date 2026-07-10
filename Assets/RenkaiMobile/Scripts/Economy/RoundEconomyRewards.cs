using UnityEngine;
using Renkai.Core;
using Renkai.Rounds;

namespace RenkaiMobile.Economy
{
    public sealed class RoundEconomyRewards : MonoBehaviour
    {
        [SerializeField] private RoundManager roundManager;
        [SerializeField] private int winReward = 3000;
        [SerializeField] private int lossRewardBase = 1900;
        [SerializeField] private int lossStreakStep = 500;
        [SerializeField] private int lossRewardMax = 2900;

        private int attackersLossStreak;
        private int defendersLossStreak;

        private void Awake()
        {
            if (roundManager == null) roundManager = FindFirstObjectByType<RoundManager>();
        }

        private void OnEnable()
        {
            if (roundManager != null) roundManager.ScoreChanged += OnScoreChanged;
        }

        private void OnDisable()
        {
            if (roundManager != null) roundManager.ScoreChanged -= OnScoreChanged;
        }

        private void OnScoreChanged(TeamId winner, int attackers, int defenders)
        {
            TeamId loser = winner == TeamId.Attackers ? TeamId.Defenders : TeamId.Attackers;
            if (winner == TeamId.Attackers)
            {
                attackersLossStreak = 0;
                defendersLossStreak++;
            }
            else if (winner == TeamId.Defenders)
            {
                defendersLossStreak = 0;
                attackersLossStreak++;
            }

            CreditWallet[] wallets = FindObjectsByType<CreditWallet>(FindObjectsSortMode.None);
            foreach (CreditWallet wallet in wallets)
            {
                TeamMember member = wallet.GetComponent<TeamMember>();
                if (member == null || member.Team == TeamId.None) continue;
                if (member.Team == winner) wallet.AddCredits(winReward);
                else if (member.Team == loser) wallet.AddCredits(GetLossReward(member.Team));
            }
        }

        private int GetLossReward(TeamId team)
        {
            int streak = team == TeamId.Attackers ? attackersLossStreak : defendersLossStreak;
            int reward = lossRewardBase + Mathf.Max(0, streak - 1) * lossStreakStep;
            return Mathf.Min(reward, lossRewardMax);
        }
    }
}
