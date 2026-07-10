using UnityEngine;
using RenkaiMobile.Combat;
using RenkaiMobile.Core;
using RenkaiMobile.Rounds;

namespace RenkaiMobile.Spectate
{
    public sealed class LocalDeathSpectateBridge : MonoBehaviour
    {
        [SerializeField] private TeamSpectateController spectateController;
        [SerializeField] private MobileRoundManager roundManager;
        [SerializeField] private MobileHealth localHealth;
        [SerializeField] private MobileTeamMember localTeam;

        private void Awake()
        {
            if (spectateController == null) spectateController = FindFirstObjectByType<TeamSpectateController>();
            if (roundManager == null) roundManager = FindFirstObjectByType<MobileRoundManager>();
            ResolveLocalPlayer();
        }

        private void OnEnable()
        {
            if (localHealth != null) localHealth.Died += OnLocalDied;
            if (roundManager != null) roundManager.PhaseChanged += OnPhaseChanged;
        }

        private void OnDisable()
        {
            if (localHealth != null) localHealth.Died -= OnLocalDied;
            if (roundManager != null) roundManager.PhaseChanged -= OnPhaseChanged;
        }

        private void ResolveLocalPlayer()
        {
            if (localHealth != null && localTeam != null) return;
            MobileCombatantIdentity[] identities = FindObjectsByType<MobileCombatantIdentity>(FindObjectsSortMode.None);
            foreach (MobileCombatantIdentity identity in identities)
            {
                if (identity == null || !identity.PlayerControlled) continue;
                localHealth = identity.GetComponent<MobileHealth>();
                localTeam = identity.GetComponent<MobileTeamMember>();
                break;
            }
        }

        private void OnLocalDied(MobileDamageInfo damage)
        {
            if (spectateController != null && localTeam != null)
                spectateController.BeginSpectating(localTeam.Team);
        }

        private void OnPhaseChanged(MobileRoundPhase phase)
        {
            if (phase == MobileRoundPhase.Buy)
                spectateController?.EndSpectating();
        }
    }
}
