using System;
using UnityEngine;
using RenkaiMobile.Core;

namespace RenkaiMobile.Audio
{
    public enum MobileSoundType
    {
        Footstep,
        Gunshot,
        Reload,
        Ability,
        ZodiacActivate,
        ZodiacDisrupt,
        Landing,
        Melee,
        Decoy
    }

    public readonly struct MobileSoundEvent
    {
        public MobileSoundEvent(Vector3 position, float radius, MobileSoundType soundType, GameObject source, MobileTeamId team, float timestamp)
        {
            Position = position;
            Radius = radius;
            SoundType = soundType;
            Source = source;
            Team = team;
            Timestamp = timestamp;
        }

        public Vector3 Position { get; }
        public float Radius { get; }
        public MobileSoundType SoundType { get; }
        public GameObject Source { get; }
        public MobileTeamId Team { get; }
        public float Timestamp { get; }
    }

    public sealed class MobileSoundEventBus : MonoBehaviour
    {
        public event Action<MobileSoundEvent> SoundEmitted;

        public void Emit(MobileSoundEvent soundEvent)
        {
            SoundEmitted?.Invoke(soundEvent);
        }
    }
}
