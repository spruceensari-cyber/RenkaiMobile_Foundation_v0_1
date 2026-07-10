using UnityEngine;
using RenkaiMobile.Audio;

namespace RenkaiMobile.Bots
{
    public enum BotContactSourceType
    {
        Vision,
        Damage,
        Sound
    }

    public struct BotKnownContact
    {
        public Vector3 LastKnownPosition;
        public float LastSensedTime;
        public float Confidence;
        public BotContactSourceType SourceType;
        public GameObject Target;
        public MobileSoundType SoundType;
    }

    public sealed class BotPerceptionMemory : MonoBehaviour
    {
        [SerializeField] private float memorySeconds = 7f;

        public bool HasContact { get; private set; }
        public BotKnownContact CurrentContact { get; private set; }

        private void Update()
        {
            if (!HasContact) return;
            if (Time.time - CurrentContact.LastSensedTime > memorySeconds)
                Clear();
        }

        public void Remember(BotKnownContact contact)
        {
            CurrentContact = contact;
            HasContact = true;
        }

        public void Clear()
        {
            HasContact = false;
            CurrentContact = default;
        }
    }
}
