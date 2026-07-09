using UnityEngine;

namespace Renkai.Combat
{
    public readonly struct DamageInfo
    {
        public DamageInfo(float amount, Vector3 point, Vector3 direction, GameObject instigator, bool headshot)
        {
            Amount = amount;
            Point = point;
            Direction = direction;
            Instigator = instigator;
            Headshot = headshot;
        }

        public float Amount { get; }
        public Vector3 Point { get; }
        public Vector3 Direction { get; }
        public GameObject Instigator { get; }
        public bool Headshot { get; }
    }
}
