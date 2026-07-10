using UnityEngine;
using Renkai.Core;

namespace RenkaiMobile.Objective
{
    [RequireComponent(typeof(TeamMember))]
    public sealed class SpiritCoreCarrier : MonoBehaviour
    {
        [SerializeField] private SpiritCoreRoundObjective objective;
        [SerializeField] private float interactionRange = 3.5f;
        [SerializeField] private KeyCode editorInteractKey = KeyCode.E;

        public bool HasCore { get; private set; }

        private TeamMember team;
        private SpiritCoreSiteZone currentSite;

        private void Awake()
        {
            team = GetComponent<TeamMember>();
            if (objective == null) objective = FindFirstObjectByType<SpiritCoreRoundObjective>();
            HasCore = team != null && team.Team == TeamId.Attackers;
        }

        private void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetKeyDown(editorInteractKey)) BeginInteraction();
            if (Input.GetKey(editorInteractKey)) TickInteraction(Time.deltaTime);
            if (Input.GetKeyUp(editorInteractKey)) CancelInteraction();
#endif
        }

        public void BeginInteraction()
        {
            if (objective == null || team == null) return;
            if (team.Team == TeamId.Attackers && (!HasCore || currentSite == null || objective.IsPlanted)) return;
            if (team.Team == TeamId.Defenders && !objective.IsPlanted) return;
            objective.BeginInteraction(team.Team);
        }

        public void TickInteraction(float deltaTime)
        {
            if (objective == null) return;
            objective.TickInteraction(deltaTime);
            if (objective.IsPlanted) HasCore = false;
        }

        public void CancelInteraction()
        {
            objective?.CancelInteraction();
        }

        public void GiveCore() => HasCore = true;

        private void OnTriggerEnter(Collider other)
        {
            var site = other.GetComponent<SpiritCoreSiteZone>();
            if (site != null && Vector3.Distance(transform.position, site.transform.position) <= interactionRange + 4f)
                currentSite = site;
        }

        private void OnTriggerExit(Collider other)
        {
            var site = other.GetComponent<SpiritCoreSiteZone>();
            if (site != null && currentSite == site) currentSite = null;
        }
    }
}
