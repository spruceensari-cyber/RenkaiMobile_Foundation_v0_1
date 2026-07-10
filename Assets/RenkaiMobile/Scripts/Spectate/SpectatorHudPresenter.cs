using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.Combat;
using RenkaiMobile.Teams;

namespace RenkaiMobile.Spectate
{
    public sealed class SpectatorHudPresenter : MonoBehaviour
    {
        [SerializeField] private TeamSpectateController spectateController;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private Text playerText;
        [SerializeField] private Text agentText;
        [SerializeField] private Text healthText;
        [SerializeField] private Text hintText;

        private void Awake()
        {
            if (spectateController == null) spectateController = FindFirstObjectByType<TeamSpectateController>();
            if (group == null) group = GetComponent<CanvasGroup>();
            if (hintText != null) hintText.text = "SPECTATE // PREV  ◀   ▶  NEXT";
        }

        private void OnEnable()
        {
            if (spectateController != null) spectateController.TargetChanged += RefreshTarget;
        }

        private void OnDisable()
        {
            if (spectateController != null) spectateController.TargetChanged -= RefreshTarget;
        }

        private void Update()
        {
            bool visible = spectateController != null && spectateController.IsSpectating;
            if (group != null) group.alpha = Mathf.MoveTowards(group.alpha, visible ? 1f : 0f, 8f * Time.unscaledDeltaTime);
            if (!visible) return;
            RefreshTarget(spectateController.Current);
        }

        private void RefreshTarget(FiveVFiveRosterAgent target)
        {
            if (target == null) return;
            MobileCombatantIdentity identity = target.GetComponent<MobileCombatantIdentity>();
            MobileHealth health = target.GetComponent<MobileHealth>();
            if (playerText != null) playerText.text = identity != null ? identity.DisplayName.ToUpperInvariant() : target.name.ToUpperInvariant();
            if (agentText != null) agentText.text = identity != null ? identity.AgentId.ToUpperInvariant() : "AGENT";
            if (healthText != null && health != null) healthText.text = "HP // " + Mathf.CeilToInt(health.Current);
        }
    }
}
