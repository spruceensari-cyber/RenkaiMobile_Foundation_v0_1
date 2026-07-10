using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.Agents;

namespace RenkaiMobile.UI
{
    public sealed class AgentSelectionPresenter : MonoBehaviour
    {
        [SerializeField] private AgentSelectionController selection;
        [SerializeField] private Text agentNameText;
        [SerializeField] private Text roleText;
        [SerializeField] private Text taglineText;
        [SerializeField] private Text abilitiesText;
        [SerializeField] private Image portrait;
        [SerializeField] private Graphic[] accentGraphics;
        [SerializeField] private Transform previewAnchor;
        [SerializeField] private CanvasGroup lockBanner;

        private GameObject activePreview;

        private void Awake()
        {
            if (selection == null) selection = FindFirstObjectByType<AgentSelectionController>();
        }

        private void OnEnable()
        {
            if (selection == null) return;
            selection.SelectionChanged += OnSelectionChanged;
            selection.SelectionLocked += OnSelectionLocked;
        }

        private void OnDisable()
        {
            if (selection == null) return;
            selection.SelectionChanged -= OnSelectionChanged;
            selection.SelectionLocked -= OnSelectionLocked;
        }

        private void Start()
        {
            if (selection != null && selection.SelectedAgent != null)
                OnSelectionChanged(selection.SelectedAgent, 0);
        }

        private void OnSelectionChanged(AgentDefinition agent, int index)
        {
            if (agent == null) return;
            if (agentNameText != null) agentNameText.text = agent.displayName.ToUpperInvariant();
            if (roleText != null) roleText.text = agent.roleLabel.ToUpperInvariant();
            if (taglineText != null) taglineText.text = agent.tagline;
            if (abilitiesText != null)
                abilitiesText.text = agent.abilityOneName + "  //  " + agent.abilityTwoName + "  //  " + agent.signatureName;
            if (portrait != null) portrait.sprite = agent.portrait;

            if (accentGraphics != null)
            {
                for (int i = 0; i < accentGraphics.Length; i++)
                {
                    Graphic graphic = accentGraphics[i];
                    if (graphic != null) graphic.color = i % 2 == 0 ? agent.primaryColor : agent.secondaryColor;
                }
            }

            if (activePreview != null) Destroy(activePreview);
            if (previewAnchor != null && agent.previewPrefab != null)
            {
                activePreview = Instantiate(agent.previewPrefab, previewAnchor);
                activePreview.transform.localPosition = Vector3.zero;
                activePreview.transform.localRotation = Quaternion.identity;
            }

            if (lockBanner != null) lockBanner.alpha = 0f;
        }

        private void OnSelectionLocked(AgentDefinition agent)
        {
            if (lockBanner != null) lockBanner.alpha = 1f;
        }
    }
}
