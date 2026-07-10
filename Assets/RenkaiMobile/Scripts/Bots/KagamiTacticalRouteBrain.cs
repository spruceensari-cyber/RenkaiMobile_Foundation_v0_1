using UnityEngine;
using RenkaiMobile.Core;
using RenkaiMobile.Map;
using RenkaiMobile.Objective;
using RenkaiMobile.Rounds;
using RenkaiMobile.Teams;

namespace RenkaiMobile.Bots
{
    [RequireComponent(typeof(MobileTeamMember), typeof(BotNavigationIntentController))]
    public sealed class KagamiTacticalRouteBrain : MonoBehaviour
    {
        [SerializeField] private KagamiRouteGraph routeGraph;
        [SerializeField] private MobileRoundManager roundManager;
        [SerializeField] private ZodiacObjective zodiacObjective;
        [SerializeField] private float decisionInterval = 2.5f;

        private MobileTeamMember team;
        private FiveVFiveRosterAgent rosterAgent;
        private BotNavigationIntentController navigationIntent;
        private float nextDecision;
        private KagamiLocationId currentTarget;

        private void Awake()
        {
            team = GetComponent<MobileTeamMember>();
            rosterAgent = GetComponent<FiveVFiveRosterAgent>();
            navigationIntent = GetComponent<BotNavigationIntentController>();
            if (routeGraph == null) routeGraph = FindFirstObjectByType<KagamiRouteGraph>();
            if (roundManager == null) roundManager = FindFirstObjectByType<MobileRoundManager>();
            if (zodiacObjective == null) zodiacObjective = FindFirstObjectByType<ZodiacObjective>();
        }

        private void Update()
        {
            if (roundManager == null || roundManager.Phase != MobileRoundPhase.Live || routeGraph == null) return;

            if (Time.time >= nextDecision)
            {
                nextDecision = Time.time + decisionInterval;
                currentTarget = ChooseTarget();
            }

            if (!routeGraph.TryGetPosition(currentTarget, out Vector3 destination)) return;
            BotNavigationIntentPriority priority = zodiacObjective != null && zodiacObjective.State == ZodiacState.Active
                ? BotNavigationIntentPriority.ObjectiveCritical
                : BotNavigationIntentPriority.Rotate;
            navigationIntent?.Submit(destination, priority, decisionInterval + 0.3f);
        }

        private KagamiLocationId ChooseTarget()
        {
            int slot = rosterAgent != null ? rosterAgent.SlotIndex : Mathf.Abs(gameObject.GetInstanceID()) % 5;
            bool objectiveActive = zodiacObjective != null && zodiacObjective.State == ZodiacState.Active;

            if (team != null && team.Team == MobileTeamId.Attackers)
            {
                if (objectiveActive)
                    return slot % 2 == 0 ? KagamiLocationId.AZodiacZone : KagamiLocationId.BZodiacZone;

                return slot switch
                {
                    0 => KagamiLocationId.AMain,
                    1 => KagamiLocationId.AShort,
                    2 => KagamiLocationId.Mid,
                    3 => KagamiLocationId.BShort,
                    _ => KagamiLocationId.BMain
                };
            }

            if (objectiveActive)
                return slot < 3 ? KagamiLocationId.AZodiacZone : KagamiLocationId.BZodiacZone;

            return slot switch
            {
                0 => KagamiLocationId.AZodiacZone,
                1 => KagamiLocationId.AShort,
                2 => KagamiLocationId.MidConnector,
                3 => KagamiLocationId.BShort,
                _ => KagamiLocationId.BZodiacZone
            };
        }
    }
}
