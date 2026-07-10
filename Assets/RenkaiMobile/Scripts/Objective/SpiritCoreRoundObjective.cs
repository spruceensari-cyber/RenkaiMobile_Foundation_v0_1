using System;
using UnityEngine;
using Renkai.Core;
using Renkai.Rounds;

namespace RenkaiMobile.Objective
{
    public sealed class SpiritCoreRoundObjective : MonoBehaviour
    {
        [SerializeField] private RoundManager roundManager;
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
        private TeamId interactingTeam = TeamId.None;

        private void Awake()
        {
            if (roundManager == null) roundManager = FindFirstObjectByType<RoundManager>();
        }

        private void Update()
        {
            if (!IsPlanted) return;
            RemainingDetonationSeconds -= Time.deltaTime;
            if (RemainingDetonationSeconds <= 0f)
            {
                RemainingDetonationSeconds = 0f;
                IsPlanted = false;
                Detonated?.Invoke();
                roundManager?.EndRound(TeamId.Attackers);
            }
        }

        public void BeginInteraction(TeamId team)
        {
            interactingTeam = team;
            interactionProgress = 0f;
            ProgressChanged?.Invoke(0f, RequiredInteractionSeconds());
        }

        public void TickInteraction(float deltaTime)
        {
            if (interactingTeam == TeamId.None) return;
            interactionProgress += deltaTime;
            float required = RequiredInteractionSeconds();
            ProgressChanged?.Invoke(interactionProgress, required);
            if (interactionProgress < required) return;

            if (!IsPlanted && interactingTeam == TeamId.Attackers)
            {
                IsPlanted = true;
                RemainingDetonationSeconds = detonationSeconds;
                Planted?.Invoke();
            }
            else if (IsPlanted && interactingTeam == TeamId.Defenders)
            {
                IsPlanted = false;
                Defused?.Invoke();
                roundManager?.EndRound(TeamId.Defenders);
            }

            CancelInteraction();
        }

        public void CancelInteraction()
        {
            interactingTeam = TeamId.None;
            interactionProgress = 0f;
            ProgressChanged?.Invoke(0f, 0f);
        }

        public void ResetObjective()
        {
            IsPlanted = false;
            RemainingDetonationSeconds = 0f;
            CancelInteraction();
        }

        private float RequiredInteractionSeconds()
        {
            return IsPlanted ? defuseSeconds : plantSeconds;
        }
    }
}
