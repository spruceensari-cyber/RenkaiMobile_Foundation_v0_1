using UnityEngine;
using RenkaiMobile.Core;

namespace RenkaiMobile.Objective
{
    [RequireComponent(typeof(MobileTeamMember))]
    public sealed class ZodiacCarrier : MonoBehaviour
    {
        [SerializeField] private ZodiacObjective objective;
        [SerializeField] private KeyCode editorInteractKey = KeyCode.E;

        public bool HasZodiac { get; private set; }
        public ZodiacZone CurrentZone { get; private set; }

        private MobileTeamMember team;
        private bool interacting;

        private void Awake()
        {
            team = GetComponent<MobileTeamMember>();
            if (objective == null) objective = FindFirstObjectByType<ZodiacObjective>();
            HasZodiac = team != null && team.Team == MobileTeamId.Attackers;
            if (HasZodiac) objective?.MarkCarried();
        }

        private void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetKeyDown(editorInteractKey)) BeginInteract();
            if (Input.GetKey(editorInteractKey)) TickInteract(Time.deltaTime);
            if (Input.GetKeyUp(editorInteractKey)) EndInteract();
#endif
        }

        public bool CanInteract()
        {
            if (objective == null || team == null) return false;
            if (team.Team == MobileTeamId.Attackers)
                return HasZodiac && CurrentZone != null && (objective.State == ZodiacState.Carried || objective.State == ZodiacState.Dormant);
            if (team.Team == MobileTeamId.Defenders)
                return objective.State == ZodiacState.Active;
            return false;
        }

        public void BeginInteract()
        {
            if (!CanInteract()) return;
            interacting = objective.BeginInteraction(team.Team);
        }

        public void TickInteract(float deltaTime)
        {
            if (!interacting || objective == null) return;
            objective.TickInteraction(deltaTime);
            if (objective.State == ZodiacState.Active && team.Team == MobileTeamId.Attackers)
                HasZodiac = false;
        }

        public void EndInteract()
        {
            if (!interacting) return;
            interacting = false;
            objective?.CancelInteraction();
        }

        public void GiveZodiac()
        {
            if (team != null && team.Team == MobileTeamId.Attackers)
            {
                HasZodiac = true;
                objective?.MarkCarried();
            }
        }

        public void RemoveZodiac()
        {
            HasZodiac = false;
            EndInteract();
        }

        private void OnTriggerEnter(Collider other)
        {
            ZodiacZone zone = other.GetComponent<ZodiacZone>();
            if (zone != null) CurrentZone = zone;
        }

        private void OnTriggerExit(Collider other)
        {
            ZodiacZone zone = other.GetComponent<ZodiacZone>();
            if (zone != null && CurrentZone == zone)
            {
                CurrentZone = null;
                EndInteract();
            }
        }
    }
}
