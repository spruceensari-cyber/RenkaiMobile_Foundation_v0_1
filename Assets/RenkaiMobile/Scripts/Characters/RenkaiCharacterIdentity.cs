using UnityEngine;

namespace RenkaiMobile.Characters
{
    public enum AgentRole { Duelist, Initiator, Controller, Sentinel }

    public sealed class RenkaiCharacterIdentity : MonoBehaviour
    {
        [SerializeField] private string agentId = "raika";
        [SerializeField] private string displayName = "Raika";
        [SerializeField] private AgentRole role = AgentRole.Duelist;
        [SerializeField] private Color accentColor = new Color(0.15f, 0.7f, 1f);

        public string AgentId => agentId;
        public string DisplayName => displayName;
        public AgentRole Role => role;
        public Color AccentColor => accentColor;

        public void Configure(string id, string name, AgentRole agentRole, Color accent)
        {
            agentId = id;
            displayName = name;
            role = agentRole;
            accentColor = accent;
        }
    }
}
