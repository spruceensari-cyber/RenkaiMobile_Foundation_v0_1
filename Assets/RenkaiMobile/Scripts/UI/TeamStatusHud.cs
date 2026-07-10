using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.Core;
using RenkaiMobile.Teams;

namespace RenkaiMobile.UI
{
    public sealed class TeamStatusHud : MonoBehaviour
    {
        [SerializeField] private Text attackersText;
        [SerializeField] private Text defendersText;
        [SerializeField] private float refreshInterval = 0.2f;
        private float nextRefresh;

        private void Update()
        {
            if (Time.unscaledTime < nextRefresh) return;
            nextRefresh = Time.unscaledTime + refreshInterval;
            Refresh();
        }

        public void Refresh()
        {
            FiveVFiveRosterAgent[] roster = FindObjectsByType<FiveVFiveRosterAgent>(FindObjectsSortMode.None);
            int attackersAlive = 0;
            int defendersAlive = 0;
            foreach (FiveVFiveRosterAgent agent in roster)
            {
                if (agent == null || !agent.IsAlive) continue;
                if (agent.Team == MobileTeamId.Attackers) attackersAlive++;
                else if (agent.Team == MobileTeamId.Defenders) defendersAlive++;
            }

            if (attackersText != null) attackersText.text = "ATTACKERS  " + BuildPips(attackersAlive);
            if (defendersText != null) defendersText.text = BuildPips(defendersAlive) + "  DEFENDERS";
        }

        private static string BuildPips(int alive)
        {
            alive = Mathf.Clamp(alive, 0, 5);
            return new string('●', alive) + new string('○', 5 - alive);
        }
    }
}
