using UnityEngine;
using RenkaiMobile.Rounds;

namespace RenkaiMobile.Bots
{
    [RequireComponent(typeof(BotPerceptionMemory))]
    public sealed class BotInvestigateSoundState : MonoBehaviour
    {
        [SerializeField] private MobileRoundManager roundManager;
        [SerializeField] private float moveSpeed = 2.2f;
        [SerializeField] private float stopDistance = 1.8f;
        [SerializeField] private float investigateTimeout = 4.5f;
        [SerializeField] private float minimumConfidence = 0.3f;

        private BotPerceptionMemory memory;
        private float investigateUntil;
        private Vector3 destination;
        private bool investigating;

        private void Awake()
        {
            memory = GetComponent<BotPerceptionMemory>();
            if (roundManager == null) roundManager = FindFirstObjectByType<MobileRoundManager>();
        }

        private void Update()
        {
            if (roundManager != null && roundManager.Phase != MobileRoundPhase.Live) return;

            if (!investigating)
            {
                TryBeginInvestigation();
                return;
            }

            if (Time.time >= investigateUntil)
            {
                investigating = false;
                return;
            }

            Vector3 delta = destination - transform.position;
            delta.y = 0f;
            if (delta.magnitude <= stopDistance)
            {
                investigating = false;
                return;
            }

            Vector3 dir = delta.normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 6f * Time.deltaTime);
            }
        }

        private void TryBeginInvestigation()
        {
            if (memory == null || !memory.HasContact) return;
            BotKnownContact contact = memory.CurrentContact;
            if (contact.SourceType != BotContactSourceType.Sound || contact.Confidence < minimumConfidence) return;

            destination = contact.LastKnownPosition;
            investigateUntil = Time.time + investigateTimeout;
            investigating = true;
        }
    }
}
