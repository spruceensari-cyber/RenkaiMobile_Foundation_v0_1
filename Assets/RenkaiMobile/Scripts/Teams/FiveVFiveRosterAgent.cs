using UnityEngine;
using Renkai.Core;
using Renkai.Combat;

namespace RenkaiMobile.Teams
{
    [RequireComponent(typeof(TeamMember), typeof(Health))]
    public sealed class FiveVFiveRosterAgent : MonoBehaviour
    {
        [SerializeField] private int slotIndex;
        [SerializeField] private bool playerControlled;
        private Vector3 spawnPosition;
        private Quaternion spawnRotation;
        private Health health;
        private TeamMember team;

        public TeamId Team => team != null ? team.Team : TeamId.None;
        public bool IsAlive => health != null && health.IsAlive;
        public bool PlayerControlled => playerControlled;
        public int SlotIndex => slotIndex;

        private void Awake()
        {
            health = GetComponent<Health>();
            team = GetComponent<TeamMember>();
            spawnPosition = transform.position;
            spawnRotation = transform.rotation;
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
