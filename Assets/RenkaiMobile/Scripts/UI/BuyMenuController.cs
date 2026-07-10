using UnityEngine;
using UnityEngine.UI;
using Renkai.Rounds;
using RenkaiMobile.Economy;

namespace RenkaiMobile.UI
{
    public sealed class BuyMenuController : MonoBehaviour
    {
        [SerializeField] private RoundManager roundManager;
        [SerializeField] private BuyPhaseController buyController;
        [SerializeField] private CreditWallet wallet;
        [SerializeField] private GameObject panel;
        [SerializeField] private Text creditsText;
        [SerializeField] private Text loadoutText;

        private void Awake()
        {
            if (roundManager == null) roundManager = FindFirstObjectByType<RoundManager>();
            if (buyController == null) buyController = FindFirstObjectByType<BuyPhaseController>();
            if (wallet == null && buyController != null) wallet = buyController.GetComponent<CreditWallet>();
        }

        private void OnEnable()
        {
            if (roundManager != null) roundManager.PhaseChanged += OnPhaseChanged;
            if (wallet != null) wallet.CreditsChanged += OnCreditsChanged;
            Refresh();
        }

        private void OnDisable()
        {
            if (roundManager != null) roundManager.PhaseChanged -= OnPhaseChanged;
            if (wallet != null) wallet.CreditsChanged -= OnCreditsChanged;
        }

        public void Toggle()
        {
            if (panel == null || roundManager == null || roundManager.Phase != RoundPhase.Buy) return;
            panel.SetActive(!panel.activeSelf);
            Refresh();
        }

        public void BuyRifle() { if (buyController != null) buyController.BuyRifle(); Refresh(); }
        public void BuyShield() { if (buyController != null) buyController.BuyShield(); Refresh(); }
        public void BuyAbilityCharge() { if (buyController != null) buyController.BuyAbilityCharge(); Refresh(); }

        private void OnPhaseChanged(RoundPhase phase)
        {
            if (panel != null) panel.SetActive(phase == RoundPhase.Buy);
            Refresh();
        }

        private void OnCreditsChanged(int credits) => Refresh();

        private void Refresh()
        {
            if (creditsText != null && wallet != null) creditsText.text = wallet.Credits + " CR";
            if (loadoutText != null && buyController != null)
            {
                loadoutText.text = "RIFLE: " + (buyController.HasRifle ? "OWNED" : "--") +
                                   "   SHIELD: " + (buyController.HasShield ? "OWNED" : "--") +
                                   "   ABILITY: " + buyController.AbilityCharges;
            }
        }
    }
}
