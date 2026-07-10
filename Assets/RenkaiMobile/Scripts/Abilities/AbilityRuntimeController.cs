using System;
using UnityEngine;

namespace RenkaiMobile.Abilities
{
    public sealed class AbilityRuntimeController : MonoBehaviour
    {
        [SerializeField] private float abilityOneCooldown = 10f;
        [SerializeField] private float abilityTwoCooldown = 18f;
        [SerializeField] private float signatureCooldown = 35f;

        public event Action<int> AbilityUsed;

        private readonly float[] readyTimes = new float[3];

        public bool TryUseAbility(int slot)
        {
            if (slot < 0 || slot >= readyTimes.Length) return false;
            if (Time.time < readyTimes[slot]) return false;

            float cooldown = slot == 0 ? abilityOneCooldown : slot == 1 ? abilityTwoCooldown : signatureCooldown;
            readyTimes[slot] = Time.time + cooldown;
            AbilityUsed?.Invoke(slot);
            return true;
        }

        public float RemainingCooldown(int slot)
        {
            if (slot < 0 || slot >= readyTimes.Length) return 0f;
            return Mathf.Max(0f, readyTimes[slot] - Time.time);
        }
    }
}
