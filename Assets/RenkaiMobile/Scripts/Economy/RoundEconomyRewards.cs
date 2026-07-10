using UnityEngine;
using RenkaiMobile.Core;
using RenkaiMobile.Rounds;

namespace RenkaiMobile.Economy
{
    public sealed class RoundEconomyRewards : MonoBehaviour
    {
        [SerializeField] private MobileRoundManager roundManager;
        [SerializeField] private int winReward = 3000;
        [SerializeField] private int lossRewardBase = 1900;
        [SerializeField] private int lossStreakStep = 500;
        [SerializeField] private int lossRewardMax = 2900;

        private int attackersLossStreak;
        private int defendersLossStreak;

        private void Awake()
        {
            if (roundManager == null) roundManager = FindFirstObjectByType<MobileRoundManager>();
        }

        private void OnEnable()
        {
            if (roundManager != null) roundManager.ScoreChanged += OnScoreChanged;
        }

        private void OnDisable()
        {
            if (roundManager != null) roundManager.ScoreChanged -= OnScoreChanged;
        }

        private void OnScoreChanged(MobileTeamId winner, int attackers, int defenders)
        {
            MobileTeamId loser = winner == MobileTeamId.Attackers ? MobileTeamId.Defenders : MobileTeamId.Attackers;
            if (winner == MobileTeamId.Attackers)
            {
                attackersLossStreak = 0;
                defendersLossStreak++;
            }
            else if (winner == MobileTeamId.Defenders)
            {
                defendersLossStreak = 0;
                attackersLossStreak++;
            }

            CreditWallet[] wallets = FindObjectsByType<CreditWallet>(FindObjectsSortMode.None);
            foreach (CreditWallet wallet in wallets)
            {
                MobileTeamMember member = wallet.GetComponent<MobileTeamMember>();
                if (member == null || member.Team == MobileTeamId.None) continue;
                if (member.Team == winner) wallet.AddCredits(winReward);
                else if (member.Team == loser) wallet.AddCredits(GetLossReward(member.Team));
            }
        }

        private int GetLossReward(MobileTeamId team)
        {
            int streak = team == MobileTeamId.Attackers ? attackersLossStreak : defendersLossStreak;
            int reward = lossRewardBase + Mathf.Max(0, streak - 1) * lossStreakStep;
            return Mathf.Min(reward, lossRewardMax);
        }
    }
}
