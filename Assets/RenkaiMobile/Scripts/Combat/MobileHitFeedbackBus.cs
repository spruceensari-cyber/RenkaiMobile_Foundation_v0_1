using System;
using UnityEngine;

namespace RenkaiMobile.Combat
{
    public readonly struct MobileHitFeedbackEvent
    {
        public MobileHitFeedbackEvent(GameObject attacker, GameObject victim, Vector3 point, bool headshot, bool killed)
        {
            Attacker = attacker;
            Victim = victim;
            Point = point;
            Headshot = headshot;
            Killed = killed;
        }

        public GameObject Attacker { get; }
        public GameObject Victim { get; }
        public Vector3 Point { get; }
        public bool Headshot { get; }
        public bool Killed { get; }
    }

    public sealed class MobileHitFeedbackBus : MonoBehaviour
    {
        public event Action<MobileHitFeedbackEvent> HitConfirmed;

        public void Publish(MobileHitFeedbackEvent hitEvent)
        {
            HitConfirmed?.Invoke(hitEvent);
        }
    }
}
