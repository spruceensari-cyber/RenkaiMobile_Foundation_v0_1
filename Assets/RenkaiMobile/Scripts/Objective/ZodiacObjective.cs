using System;
using UnityEngine;
using RenkaiMobile.Core;
using RenkaiMobile.Rounds;

namespace RenkaiMobile.Objective
{
    public enum ZodiacState
    {
        Dormant,
        Carried,
        Activating,
        Active,
        Disrupting,
        Collapsed,
        Disrupted
    }

    public sealed class ZodiacObjective : MonoBehaviour
    {
        [SerializeField] private MobileRoundManager roundManager;
        [SerializeField] private float activateSeconds = 4f;
        [SerializeField] private float disruptSeconds = 7f;
        [SerializeField] private float collapseSeconds = 45f;

        public event Action<ZodiacState> StateChanged;
        public event Action<float, float> InteractionProgressChanged;

        public ZodiacState State { get; private set; } = ZodiacState.Dormant;
        public float RemainingCollapseSeconds { get; private set; }

        private MobileTeamId interactingTeam = MobileTeamId.None;
        private float progress;

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
            if (State != ZodiacState.Active) return;
            RemainingCollapseSeconds = Mathf.Max(0f, RemainingCollapseSeconds - Time.deltaTime);
            if (RemainingCollapseSeconds > 0f) return;

            SetState(ZodiacState.Collapsed);
            roundManager?.EndRound(MobileTeamId.Attackers);
        }

        public void MarkCarried()
        {
            if (State == ZodiacState.Dormant) SetState(ZodiacState.Carried);
        }

        public bool BeginInteraction(MobileTeamId team)
        {
            bool canActivate = team == MobileTeamId.Attackers && (State == ZodiacState.Carried || State == ZodiacState.Dormant);
            bool canDisrupt = team == MobileTeamId.Defenders && State == ZodiacState.Active;
            if (!canActivate && !canDisrupt) return false;

            interactingTeam = team;
            progress = 0f;
            SetState(canActivate ? ZodiacState.Activating : ZodiacState.Disrupting);
            InteractionProgressChanged?.Invoke(0f, RequiredSeconds());
            return true;
        }

        public void TickInteraction(float deltaTime)
        {
            if (interactingTeam == MobileTeamId.None) return;
            progress += Mathf.Max(0f, deltaTime);
            float required = RequiredSeconds();
            InteractionProgressChanged?.Invoke(progress, required);
            if (progress < required) return;

            if (interactingTeam == MobileTeamId.Attackers)
            {
                RemainingCollapseSeconds = collapseSeconds;
                SetState(ZodiacState.Active);
            }
            else
            {
                SetState(ZodiacState.Disrupted);
                roundManager?.EndRound(MobileTeamId.Defenders);
            }

            interactingTeam = MobileTeamId.None;
            progress = 0f;
            InteractionProgressChanged?.Invoke(0f, 0f);
        }

        public void CancelInteraction()
        {
            if (State == ZodiacState.Activating) SetState(ZodiacState.Carried);
            else if (State == ZodiacState.Disrupting) SetState(ZodiacState.Active);
            interactingTeam = MobileTeamId.None;
            progress = 0f;
            InteractionProgressChanged?.Invoke(0f, 0f);
        }

        private float RequiredSeconds()
        {
            return interactingTeam == MobileTeamId.Defenders ? disruptSeconds : activateSeconds;
        }

        private void OnPhaseChanged(MobileRoundPhase phase)
        {
            if (phase == MobileRoundPhase.Buy)
            {
                RemainingCollapseSeconds = 0f;
                interactingTeam = MobileTeamId.None;
                progress = 0f;
                SetState(ZodiacState.Dormant);
            }
        }

        private void SetState(ZodiacState next)
        {
            if (State == next) return;
            State = next;
            StateChanged?.Invoke(State);
        }
    }
}
