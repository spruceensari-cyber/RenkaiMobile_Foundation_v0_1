using UnityEngine;
using RenkaiMobile.Core;

namespace RenkaiMobile.Spawning
{
    public sealed class MobileSpawnPoint : MonoBehaviour
    {
        [SerializeField] private MobileTeamId team = MobileTeamId.Attackers;
        [SerializeField] private int slotIndex;

        public MobileTeamId Team => team;
        public int SlotIndex => slotIndex;

        public void Configure(MobileTeamId spawnTeam, int slot)
        {
            team = spawnTeam;
            slotIndex = Mathf.Max(0, slot);
        }
    }
}
