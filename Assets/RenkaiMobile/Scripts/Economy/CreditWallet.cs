using System;
using UnityEngine;

namespace RenkaiMobile.Economy
{
    public sealed class CreditWallet : MonoBehaviour
    {
        [SerializeField, Min(0)] private int startingCredits = 800;
        [SerializeField, Min(0)] private int maxCredits = 9000;

        public event Action<int> CreditsChanged;
        public int Credits { get; private set; }

        private void Awake()
        {
            Credits = Mathf.Clamp(startingCredits, 0, maxCredits);
        }

        public bool TrySpend(int amount)
        {
            if (amount <= 0 || Credits < amount) return false;
            Credits -= amount;
            CreditsChanged?.Invoke(Credits);
            return true;
        }

        public void AddCredits(int amount)
        {
            if (amount <= 0) return;
            Credits = Mathf.Clamp(Credits + amount, 0, maxCredits);
            CreditsChanged?.Invoke(Credits);
        }

        public void SetCredits(int value)
        {
            Credits = Mathf.Clamp(value, 0, maxCredits);
            CreditsChanged?.Invoke(Credits);
        }
    }
}
