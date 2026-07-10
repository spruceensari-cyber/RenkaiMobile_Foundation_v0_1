using UnityEngine;

namespace RenkaiMobile.Agents
{
    [CreateAssetMenu(menuName = "Renkai Mobile/Agents/Agent Definition", fileName = "Agent_")]
    public sealed class AgentDefinition : ScriptableObject
    {
        public string agentId = "raika";
        public string displayName = "Raika";
        [TextArea] public string roleLabel = "Duelist";
        [TextArea] public string tagline = "Velocity becomes a weapon.";
        public Sprite portrait;
        public GameObject previewPrefab;
        public Color primaryColor = new Color(0.1f, 0.75f, 1f, 1f);
        public Color secondaryColor = new Color(0.75f, 0.2f, 1f, 1f);
        public string abilityOneName = "Velocity Surge";
        public string abilityTwoName = "Arc Pulse";
        public string signatureName = "Overdrive";
    }
}
