using UnityEngine;
using UnityEngine.EventSystems;

namespace RenkaiMobile.UI
{
    public enum BuyMenuAction
    {
        Rifle,
        Shield,
        AbilityCharge
    }

    public sealed class BuyMenuActionButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private BuyMenuController buyMenu;
        [SerializeField] private BuyMenuAction action;

        private void Awake()
        {
            if (buyMenu == null) buyMenu = FindFirstObjectByType<BuyMenuController>();
        }

        public void Configure(BuyMenuController menu, BuyMenuAction buttonAction)
        {
            buyMenu = menu;
            action = buttonAction;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (buyMenu == null) return;
            switch (action)
            {
                case BuyMenuAction.Rifle:
                    buyMenu.BuyRifle();
                    break;
                case BuyMenuAction.Shield:
                    buyMenu.BuyShield();
                    break;
                case BuyMenuAction.AbilityCharge:
                    buyMenu.BuyAbilityCharge();
                    break;
            }
        }
    }
}
