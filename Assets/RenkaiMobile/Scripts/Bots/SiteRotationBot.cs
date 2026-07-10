using UnityEngine;
using RenkaiMobile.Core;
using RenkaiMobile.Rounds;
using RenkaiMobile.Objective;

namespace RenkaiMobile.Bots
{
    [RequireComponent(typeof(MobileTeamMember), typeof(BotNavigationIntentController))]
    public sealed class SiteRotationBot : MonoBehaviour
    {
        [SerializeField] private float rotateDecisionSeconds = 5f;

        private MobileTeamMember team;
        private MobileRoundManager roundManager;
        private ZodiacObjective objective;
        private ZodiacZone[] zones;
        private BotNavigationIntentController navigationIntent;
        private Transform destination;
        private float nextDecision;

        private void Awake()
        {
            team = GetComponent<MobileTeamMember>();
            navigationIntent = GetComponent<BotNavigationIntentController>();
            roundManager = FindFirstObjectByType<MobileRoundManager>();
            objective = FindFirstObjectByType<ZodiacObjective>();
            zones = FindObjectsByType<ZodiacZone>(FindObjectsSortMode.None);
        }

        private void Update()
        {
            if (roundManager == null || roundManager.Phase != MobileRoundPhase.Live) return;
            if (zones == null || zones.Length == 0) return;

            if (destination == null || Time.time >= nextDecision)
            {
                nextDecision = Time.time + rotateDecisionSeconds;
                destination = ChooseDestination();
            }

            if (destination == null) return;
            BotNavigationIntentPriority priority = objective != null && objective.State == ZodiacState.Active
                ? BotNavigationIntentPriority.ObjectiveCritical
                : BotNavigationIntentPriority.Rotate;
            navigationIntent?.Submit(destination.position, priority, 0.5f);
        }

        private Transform ChooseDestination()
        {
            if (zones.Length == 0) return null;

            if (team != null && team.Team == MobileTeamId.Defenders && objective != null && objective.State == ZodiacState.Active)
                return FindNearestZone();

            int index = Mathf.Abs(gameObject.GetInstanceID()) % zones.Length;
            if (Random.value > 0.6f) index = Random.Range(0, zones.Length);
            return zones[index] != null ? zones[index].transform : null;
        }

        private Transform FindNearestZone()
        {
            Transform best = null;
            float bestSqr = float.MaxValue;
            foreach (ZodiacZone zone in zones)
            {
                if (zone == null) continue;
                float sqr = (zone.transform.position - transform.position).sqrMagnitude;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    best = zone.transform;
                }
            }
            return best;
        }
    }
}
