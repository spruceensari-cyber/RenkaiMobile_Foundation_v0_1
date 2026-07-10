using System;
using UnityEngine;

namespace RenkaiMobile.Combat
{
    public sealed class MobileHealth : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float maxHealth = 100f;

        public event Action<float, float> Changed;
        public event Action<MobileDamageInfo> Damaged;
        public event Action<MobileDamageInfo> Died;

        public float Current { get; private set; }
        public float Max => maxHealth;
        public bool IsAlive => Current > 0f;

        private void Awake()
        {
            Current = maxHealth;
        }

        public void ApplyDamage(MobileDamageInfo damage)
        {
            if (!IsAlive || damage.Amount <= 0f) return;
            Current = Mathf.Max(0f, Current - damage.Amount);
            Changed?.Invoke(Current, maxHealth);
            Damaged?.Invoke(damage);
            if (Current <= 0f) Died?.Invoke(damage);
        }

        public void ResetHealth()
        {
            Current = maxHealth;
            Changed?.Invoke(Current, maxHealth);
        }
    }
}
