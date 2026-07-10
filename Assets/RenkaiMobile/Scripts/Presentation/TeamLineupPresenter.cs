using System.Text;
using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.Combat;
using RenkaiMobile.Core;
using RenkaiMobile.Teams;

namespace RenkaiMobile.Presentation
{
    public sealed class TeamLineupPresenter : MonoBehaviour
    {
        [SerializeField] private Text attackersText;
        [SerializeField] private Text defendersText;
        [SerializeField] private Text headerText;

        public void Refresh()
        {
            if (headerText != null) headerText.text = "KAGAMI DISTRICT // TEAM LINK";
            FiveVFiveRosterAgent[] roster = FindObjectsByType<FiveVFiveRosterAgent>(FindObjectsSortMode.None);
            StringBuilder attackers = new StringBuilder();
            StringBuilder defenders = new StringBuilder();

            foreach (FiveVFiveRosterAgent agent in roster)
            {
                if (agent == null) continue;
                MobileCombatantIdentity identity = agent.GetComponent<MobileCombatantIdentity>();
                string shown = identity != null ? identity.DisplayName : agent.name;
                string agentId = identity != null ? identity.AgentId.ToUpperInvariant() : "AGENT";
                string line = shown + " // " + agentId + "\n";
                if (agent.Team == MobileTeamId.Attackers) attackers.Append(line);
                else if (agent.Team == MobileTeamId.Defenders) defenders.Append(line);
            }

            if (attackersText != null) attackersText.text = attackers.ToString();
            if (defendersText != null) defendersText.text = defenders.ToString();
        }

        private void OnEnable() => Refresh();
    }
}
