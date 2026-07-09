using UnityEngine;
using UnityEngine.EventSystems;
using Renkai.Input;

namespace RenkaiMobile.Input
{
    public sealed class MobileActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        public enum ActionType { Fire, Ads, Crouch, Jump, Reload }

        [SerializeField] private MobileInputRouter router;
        [SerializeField] private ActionType action;

        public void Configure(MobileInputRouter inputRouter, ActionType actionType)
        {
            router = inputRouter;
            action = actionType;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (router == null) return;
            if (action == ActionType.Fire) router.SetFireHeld(true);
            if (action == ActionType.Crouch) router.SetCrouch(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (router == null) return;
            if (action == ActionType.Fire) router.SetFireHeld(false);
            if (action == ActionType.Crouch) router.SetCrouch(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (router == null) return;
            if (action == ActionType.Jump) router.Jump();
            if (action == ActionType.Reload) router.Reload();
        }
    }
}
