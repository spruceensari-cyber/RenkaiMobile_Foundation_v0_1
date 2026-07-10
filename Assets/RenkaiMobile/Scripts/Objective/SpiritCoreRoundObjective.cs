using System;
using UnityEngine;
using RenkaiMobile.Core;
using RenkaiMobile.Rounds;

namespace RenkaiMobile.Objective
{
    public sealed class SpiritCoreRoundObjective : MonoBehaviour
    {
        [SerializeField] private MobileRoundManager roundManager;
        [SerializeField] private float plantSeconds = 4f;
        [SerializeField] private float defuseSeconds = 7f;
        [SerializeField] private float detonationSeconds = 45f;

        public event Action Planted;
        public event Action Defused;
        public event Action Detonated;
        public event Action<float, float> ProgressChanged;

        public bool IsPlanted { get; private set; }
        public float RemainingDetonationSeconds { get; private set; }

        private float interactionProgress;
        private MobileTeamId interactingTeam = MobileTeamId.None;

        private void Awake()
        {
            if (roundManager == null) roundManager = FindFirstObjectByType<MobileRoundManager>();
        }

        private void OnEnable()
        {
            if (roundManager != null) roundManager.PhaseChanged += OnPhaseChanged;
        }

        private void OnDisable()
        {
            if (roundManager != null) roundManager.PhaseChanged -= OnPhaseChanged;
        }

        private void Update()
        {
            if (!IsPlanted) return;
            RemainingDetonationSeconds -= Time.deltaTime;
            if (RemainingDetonationSeconds > 0f) return;

            RemainingDetonationSeconds = 0f;
            IsPlanted = false;
            Detonated?.Invoke();
            roundManager?.EndRound(MobileTeamId.Attackers);
        }

        public void BeginInteraction(MobileTeamId team)
        {
            interactingTeam = team;
            interactionProgress = 0f;
            ProgressChanged?.Invoke(0f, RequiredInteractionSeconds());
        }

        public void TickInteraction(float deltaTime)
        {
            if (interactingTeam == MobileTeamId.None) return;
            interactionProgress += deltaTime;
            float required = RequiredInteractionSeconds();
            ProgressChanged?.Invoke(interactionProgress, required);
            if (interactionProgress < required) return;

            if (!IsPlanted && interactingTeam == MobileTeamId.Attackers)
            {
                IsPlanted = true;
                RemainingDetonationSeconds = detonationSeconds;
                Planted?.Invoke();
            }
            else if (IsPlanted && interactingTeam == MobileTeamId.Defenders)
            {
                IsPlanted = false;
                Defused?.Invoke();
                roundManager?.EndRound(MobileTeamId.Defenders);
            }

            CancelInteraction();
        }

        public void CancelInteraction()
        {
            interactingTeam = MobileTeamId.None;
            interactionProgress = 0f;
            ProgressChanged?.Invoke(0f, 0f);
        }

        public void ResetObjective()
        {
            IsPlanted = false;
            RemainingDetonationSeconds = 0f;
            CancelInteraction();
        }

        private void OnPhaseChanged(MobileRoundPhase phase)
        {
            if (phase == MobileRoundPhase.Buy) ResetObjective();
        }

        private float RequiredInteractionSeconds()
        {
            return IsPlanted ? defuseSeconds : plantSeconds;
        }
    }
}
