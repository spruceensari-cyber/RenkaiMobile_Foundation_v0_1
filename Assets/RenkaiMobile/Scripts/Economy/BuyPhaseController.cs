using UnityEngine;
using RenkaiMobile.Rounds;

namespace RenkaiMobile.Economy
{
    public sealed class BuyPhaseController : MonoBehaviour
    {
        [SerializeField] private MobileRoundManager roundManager;
        [SerializeField] private CreditWallet wallet;
        [SerializeField] private int rifleCost = 2900;
        [SerializeField] private int shieldCost = 1000;
        [SerializeField] private int abilityChargeCost = 400;

        public bool HasRifle { get; private set; }
        public bool HasShield { get; private set; }
        public int AbilityCharges { get; private set; }

        private void Awake()
        {
            if (roundManager == null) roundManager = FindFirstObjectByType<MobileRoundManager>();
            if (wallet == null) wallet = GetComponent<CreditWallet>();
        }

        public bool BuyRifle()
        {
            if (!CanBuy() || HasRifle || wallet == null || !wallet.TrySpend(rifleCost)) return false;
            HasRifle = true;
            return true;
        }

        public bool BuyShield()
        {
            if (!CanBuy() || HasShield || wallet == null || !wallet.TrySpend(shieldCost)) return false;
            HasShield = true;
            return true;
        }

        public bool BuyAbilityCharge()
        {
            if (!CanBuy() || wallet == null || !wallet.TrySpend(abilityChargeCost)) return false;
            AbilityCharges++;
            return true;
        }

        public void ResetRoundLoadout()
        {
            HasRifle = false;
            HasShield = false;
            AbilityCharges = 0;
        }

        private bool CanBuy()
        {
            return roundManager != null && roundManager.Phase == MobileRoundPhase.Buy;
        }
    }
}
