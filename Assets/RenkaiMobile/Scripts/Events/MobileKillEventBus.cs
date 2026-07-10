using System;
using UnityEngine;
using RenkaiMobile.Combat;

namespace RenkaiMobile.Events
{
    public readonly struct MobileKillEvent
    {
        public MobileKillEvent(MobileCombatantIdentity killer, MobileCombatantIdentity victim, string weaponId, string abilityId, bool headshot, bool environmental, float timestamp)
        {
            Killer = killer;
            Victim = victim;
            WeaponId = weaponId;
            AbilityId = abilityId;
            Headshot = headshot;
            Environmental = environmental;
            Timestamp = timestamp;
        }

        public MobileCombatantIdentity Killer { get; }
        public MobileCombatantIdentity Victim { get; }
        public string WeaponId { get; }
        public string AbilityId { get; }
        public bool Headshot { get; }
        public bool Environmental { get; }
        public float Timestamp { get; }
    }

    public sealed class MobileKillEventBus : MonoBehaviour
    {
        public event Action<MobileKillEvent> KillPublished;

        public void Publish(MobileKillEvent killEvent)
        {
            KillPublished?.Invoke(killEvent);
        }
    }
}
