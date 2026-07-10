using UnityEngine;
using RenkaiMobile.Teams;

namespace RenkaiMobile.Spawning
{
    public sealed class MobileTeamSpawnController : MonoBehaviour
    {
        [SerializeField] private MobileSpawnRegistry registry;

        private void Awake()
        {
            if (registry == null) registry = FindFirstObjectByType<MobileSpawnRegistry>();
        }

        public bool Place(FiveVFiveRosterAgent agent)
        {
            if (agent == null || registry == null) return false;
            if (!registry.TryGet(agent.Team, agent.SlotIndex, out MobileSpawnPoint point)) return false;
            agent.transform.SetPositionAndRotation(point.transform.position, point.transform.rotation);
            return true;
        }

        public void PlaceAll()
        {
            FiveVFiveRosterAgent[] roster = FindObjectsByType<FiveVFiveRosterAgent>(FindObjectsSortMode.None);
            foreach (FiveVFiveRosterAgent agent in roster) Place(agent);
        }
    }
}
