using System;
using UnityEngine;

namespace Renkai.Combat
{
    public sealed class Health : MonoBehaviour, IDamageable
    {
        [SerializeField, Min(1f)] private float maxHealth = 100f;
        [SerializeField] private bool destroyOnDeath;

        public event Action<float, float> Changed;
        public event Action<DamageInfo> Died;

        public float Current { get; private set; }
        public float Max => maxHealth;
        public bool IsAlive => Current > 0f;

        private void Awake()
        {
            Current = maxHealth;
        }

        public void ApplyDamage(DamageInfo damage)
        {
            if (!IsAlive || damage.Amount <= 0f)
                return;

            Current = Mathf.Max(0f, Current - damage.Amount);
            Changed?.Invoke(Current, maxHealth);

            if (Current <= 0f)
            {
                Died?.Invoke(damage);
                if (destroyOnDeath)
                    Destroy(gameObject);
            }
        }

        public void ResetHealth()
        {
            Current = maxHealth;
            Changed?.Invoke(Current, maxHealth);
        }
    }
}
