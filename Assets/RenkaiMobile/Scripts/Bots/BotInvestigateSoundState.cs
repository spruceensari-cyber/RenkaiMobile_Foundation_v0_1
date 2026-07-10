using UnityEngine;
using RenkaiMobile.Rounds;

namespace RenkaiMobile.Bots
{
    [RequireComponent(typeof(BotPerceptionMemory), typeof(BotNavigationIntentController))]
    public sealed class BotInvestigateSoundState : MonoBehaviour
    {
        [SerializeField] private MobileRoundManager roundManager;
        [SerializeField] private float investigateTimeout = 4.5f;
        [SerializeField] private float minimumConfidence = 0.3f;

        private BotPerceptionMemory memory;
        private BotNavigationIntentController navigationIntent;
        private float investigateUntil;
        private Vector3 destination;
        private bool investigating;

        private void Awake()
        {
            memory = GetComponent<BotPerceptionMemory>();
            navigationIntent = GetComponent<BotNavigationIntentController>();
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

            navigationIntent?.Submit(destination, BotNavigationIntentPriority.InvestigateSound, 0.35f);
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
