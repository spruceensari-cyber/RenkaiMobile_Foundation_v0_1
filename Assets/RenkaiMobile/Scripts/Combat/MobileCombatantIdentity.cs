using UnityEngine;

namespace RenkaiMobile.Combat
{
    public sealed class MobileCombatantIdentity : MonoBehaviour
    {
        [SerializeField] private string combatantId = "combatant";
        [SerializeField] private string displayName = "Renkai Agent";
        [SerializeField] private string agentId = "raika";
        [SerializeField] private bool playerControlled;

        public string CombatantId => combatantId;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? gameObject.name : displayName;
        public string AgentId => agentId;
        public bool PlayerControlled => playerControlled;

        public void Configure(string id, string shownName, string agent, bool isPlayer)
        {
            combatantId = string.IsNullOrWhiteSpace(id) ? gameObject.name : id;
            displayName = string.IsNullOrWhiteSpace(shownName) ? gameObject.name : shownName;
            agentId = string.IsNullOrWhiteSpace(agent) ? "unknown" : agent;
            playerControlled = isPlayer;
        }
    }
}
