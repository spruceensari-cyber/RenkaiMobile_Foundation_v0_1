using UnityEngine;
using UnityEngine.EventSystems;
using RenkaiMobile.Input;

namespace RenkaiMobile.UI
{
    public enum MobileCombatAction
    {
        Fire,
        Ads,
        Reload,
        Primary,
        Secondary,
        Melee,
        Ability0,
        Ability1,
        Ability2,
        Interact
    }

    public sealed class MobileCombatActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private MobileCombatInputRouter inputRouter;
        [SerializeField] private MobileCombatAction action;

        private bool held;

        private void Awake()
        {
            if (inputRouter == null)
                inputRouter = FindFirstObjectByType<MobileCombatInputRouter>();
        }

        private void Update()
        {
            if (held && action == MobileCombatAction.Interact)
                inputRouter?.InteractHeld(Time.deltaTime);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            held = true;
            if (inputRouter == null) return;

            switch (action)
            {
                case MobileCombatAction.Fire: inputRouter.FireDown(); break;
                case MobileCombatAction.Ads: inputRouter.AdsDown(); break;
                case MobileCombatAction.Reload: inputRouter.Reload(); break;
                case MobileCombatAction.Primary: inputRouter.SwitchPrimary(); break;
                case MobileCombatAction.Secondary: inputRouter.SwitchSecondary(); break;
                case MobileCombatAction.Melee: inputRouter.SwitchMelee(); break;
                case MobileCombatAction.Ability0: inputRouter.UseAbility(0); break;
                case MobileCombatAction.Ability1: inputRouter.UseAbility(1); break;
                case MobileCombatAction.Ability2: inputRouter.UseAbility(2); break;
                case MobileCombatAction.Interact: inputRouter.InteractDown(); break;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            held = false;
            if (inputRouter == null) return;

            switch (action)
            {
                case MobileCombatAction.Fire: inputRouter.FireUp(); break;
                case MobileCombatAction.Ads: inputRouter.AdsUp(); break;
                case MobileCombatAction.Interact: inputRouter.InteractUp(); break;
            }
        }
    }
}
