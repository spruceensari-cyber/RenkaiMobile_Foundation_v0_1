using UnityEngine;
using RenkaiMobile.Core;
using RenkaiMobile.Combat;

namespace RenkaiMobile.Teams
{
    [RequireComponent(typeof(MobileTeamMember), typeof(MobileHealth))]
    public sealed class FiveVFiveRosterAgent : MonoBehaviour
    {
        [SerializeField] private int slotIndex;
        [SerializeField] private bool playerControlled;

        private Vector3 spawnPosition;
        private Quaternion spawnRotation;
        private MobileHealth health;
        private MobileTeamMember team;

        public MobileTeamId Team => team != null ? team.Team : MobileTeamId.None;
        public bool IsAlive => health != null && health.IsAlive;
        public bool PlayerControlled => playerControlled;
        public int SlotIndex => slotIndex;

        private void Awake()
        {
            health = GetComponent<MobileHealth>();
            team = GetComponent<MobileTeamMember>();
            CaptureSpawn();
        }

        public void Configure(int slot, bool isPlayer)
        {
            slotIndex = slot;
            playerControlled = isPlayer;
        }

        public void CaptureSpawn()
        {
            spawnPosition = transform.position;
            spawnRotation = transform.rotation;
        }

        public void ResetForRound()
        {
            transform.SetPositionAndRotation(spawnPosition, spawnRotation);
            health?.ResetHealth();
            gameObject.SetActive(true);
        }
    }
}
