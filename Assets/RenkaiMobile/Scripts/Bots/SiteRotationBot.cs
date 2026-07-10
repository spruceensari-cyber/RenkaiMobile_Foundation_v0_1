using UnityEngine;
using RenkaiMobile.Core;
using RenkaiMobile.Rounds;
using RenkaiMobile.Objective;

namespace RenkaiMobile.Bots
{
    [RequireComponent(typeof(MobileTeamMember))]
    public sealed class SiteRotationBot : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2.4f;
        [SerializeField] private float stopDistance = 2.2f;
        [SerializeField] private float rotateDecisionSeconds = 5f;

        private MobileTeamMember team;
        private MobileRoundManager roundManager;
        private SpiritCoreRoundObjective objective;
        private SpiritCoreSiteZone[] sites;
        private Transform destination;
        private float nextDecision;

        private void Awake()
        {
            team = GetComponent<MobileTeamMember>();
            roundManager = FindFirstObjectByType<MobileRoundManager>();
            objective = FindFirstObjectByType<SpiritCoreRoundObjective>();
            sites = FindObjectsByType<SpiritCoreSiteZone>(FindObjectsSortMode.None);
        }

        private void Update()
        {
            if (roundManager == null || roundManager.Phase != MobileRoundPhase.Live) return;
            if (sites == null || sites.Length == 0) return;

            if (destination == null || Time.time >= nextDecision)
            {
                nextDecision = Time.time + rotateDecisionSeconds;
                destination = ChooseDestination();
            }

            if (destination == null) return;
            Vector3 delta = destination.position - transform.position;
            delta.y = 0f;
            if (delta.magnitude <= stopDistance) return;

            Vector3 dir = delta.normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion desired = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, desired, 7f * Time.deltaTime);
            }
        }

        private Transform ChooseDestination()
        {
            if (sites.Length == 0) return null;

            if (team != null && team.Team == MobileTeamId.Defenders && objective != null && objective.IsPlanted)
                return FindNearestSite();

            int index = Mathf.Abs(gameObject.GetInstanceID()) % sites.Length;
            if (Random.value > 0.6f) index = Random.Range(0, sites.Length);
            return sites[index] != null ? sites[index].transform : null;
        }

        private Transform FindNearestSite()
        {
            Transform best = null;
            float bestSqr = float.MaxValue;
            foreach (SpiritCoreSiteZone site in sites)
            {
                if (site == null) continue;
                float sqr = (site.transform.position - transform.position).sqrMagnitude;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    best = site.transform;
                }
            }
            return best;
        }
    }
}
