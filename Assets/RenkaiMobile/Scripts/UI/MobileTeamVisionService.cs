using UnityEngine;
using RenkaiMobile.Core;
using RenkaiMobile.Teams;

namespace RenkaiMobile.UI
{
    public sealed class MobileTeamVisionService : MonoBehaviour
    {
        [SerializeField] private MobileTeamId localTeam = MobileTeamId.Attackers;
        [SerializeField] private float directVisionRange = 24f;
        [SerializeField] private float recentRevealSeconds = 3.5f;
        [SerializeField] private LayerMask sightMask = ~0;

        private FiveVFiveRosterAgent[] roster = System.Array.Empty<FiveVFiveRosterAgent>();
        private float nextRefresh;

        private void Start()
        {
            RefreshRoster();
        }

        private void Update()
        {
            if (Time.time >= nextRefresh)
            {
                nextRefresh = Time.time + 0.35f;
                RefreshRoster();
                UpdateDirectVision();
            }
        }

        public bool IsVisibleToLocalTeam(FiveVFiveRosterAgent agent)
        {
            if (agent == null) return false;
            if (agent.Team == localTeam) return true;
            MobileVisibilityState state = agent.GetComponent<MobileVisibilityState>();
            return state != null && state.IsRevealed;
        }

        public void Reveal(GameObject target, float seconds = -1f)
        {
            if (target == null) return;
            MobileVisibilityState state = target.GetComponent<MobileVisibilityState>();
            if (state == null) state = target.AddComponent<MobileVisibilityState>();
            state.Reveal(seconds > 0f ? seconds : recentRevealSeconds);
        }

        private void RefreshRoster()
        {
            roster = FindObjectsByType<FiveVFiveRosterAgent>(FindObjectsSortMode.None);
            foreach (FiveVFiveRosterAgent agent in roster)
            {
                if (agent != null && agent.GetComponent<MobileVisibilityState>() == null)
                    agent.gameObject.AddComponent<MobileVisibilityState>();
            }
        }

        private void UpdateDirectVision()
        {
            foreach (FiveVFiveRosterAgent observer in roster)
            {
                if (observer == null || !observer.IsAlive || observer.Team != localTeam) continue;

                foreach (FiveVFiveRosterAgent target in roster)
                {
                    if (target == null || !target.IsAlive || target.Team == localTeam || target.Team == MobileTeamId.None) continue;
                    Vector3 delta = target.transform.position - observer.transform.position;
                    if (delta.sqrMagnitude > directVisionRange * directVisionRange) continue;

                    Vector3 origin = observer.transform.position + Vector3.up * 1.35f;
                    Vector3 end = target.transform.position + Vector3.up * 1.2f;
                    Vector3 ray = end - origin;
                    if (Physics.Raycast(origin, ray.normalized, out RaycastHit hit, ray.magnitude + 0.25f, sightMask, QueryTriggerInteraction.Ignore))
                    {
                        if (hit.transform != target.transform && !hit.transform.IsChildOf(target.transform)) continue;
                    }

                    Reveal(target.gameObject, recentRevealSeconds);
                }
            }
        }
    }
}
