using UnityEngine;
using UnityEngine.EventSystems;
using Renkai.Input;
using RenkaiMobile.Combat;

namespace RenkaiMobile.Input
{
    public sealed class MobileActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        public enum ActionType { Fire, Ads, Crouch, Jump, Reload }

        [SerializeField] private MobileInputRouter router;
        [SerializeField] private AdsController adsController;
        [SerializeField] private ActionType action;

        public void Configure(MobileInputRouter inputRouter, AdsController ads, ActionType actionType)
        {
            router = inputRouter;
            adsController = ads;
            action = actionType;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (action == ActionType.Fire) router?.SetFireHeld(true);
            if (action == ActionType.Crouch) router?.SetCrouch(true);
            if (action == ActionType.Ads) adsController?.SetAds(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (action == ActionType.Fire) router?.SetFireHeld(false);
            if (action == ActionType.Crouch) router?.SetCrouch(false);
            if (action == ActionType.Ads) adsController?.SetAds(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (action == ActionType.Jump) router?.Jump();
            if (action == ActionType.Reload) router?.Reload();
        }
    }
}
