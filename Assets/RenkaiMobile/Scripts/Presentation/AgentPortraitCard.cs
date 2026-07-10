using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.Agents;

namespace RenkaiMobile.Presentation
{
    public sealed class AgentPortraitCard : MonoBehaviour
    {
        [SerializeField] private Image portrait;
        [SerializeField] private Text nameText;
        [SerializeField] private Text roleText;
        [SerializeField] private Graphic frame;
        [SerializeField] private CanvasGroup selectedGlow;

        public AgentDefinition Agent { get; private set; }

        public void Bind(AgentDefinition agent)
        {
            Agent = agent;
            if (agent == null) return;
            if (portrait != null) portrait.sprite = agent.portrait;
            if (nameText != null) nameText.text = agent.displayName.ToUpperInvariant();
            if (roleText != null) roleText.text = agent.roleLabel.ToUpperInvariant();
            if (frame != null) frame.color = agent.primaryColor;
            SetSelected(false);
        }

        public void SetSelected(bool selected)
        {
            if (selectedGlow != null) selectedGlow.alpha = selected ? 1f : 0f;
        }
    }
}
