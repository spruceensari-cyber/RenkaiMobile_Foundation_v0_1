using System;
using UnityEngine;
using RenkaiMobile.Abilities;
using RenkaiMobile.Combat;
using RenkaiMobile.Objective;
using RenkaiMobile.Weapons;

namespace RenkaiMobile.Input
{
    public sealed class MobileCombatInputRouter : MonoBehaviour
    {
        [SerializeField] private MobileWeaponRouter weaponRouter;
        [SerializeField] private MobileAdsController adsController;
        [SerializeField] private AgentAbilityDispatcher abilityDispatcher;
        [SerializeField] private ZodiacCarrier zodiacCarrier;

        public event Action FirePressed;
        public event Action FireReleased;
        public event Action ReloadPressed;
        public event Action<int> AbilityPressed;

        private void Awake()
        {
            if (weaponRouter == null) weaponRouter = FindFirstObjectByType<MobileWeaponRouter>();
            if (adsController == null) adsController = FindFirstObjectByType<MobileAdsController>();
            if (abilityDispatcher == null) abilityDispatcher = FindFirstObjectByType<AgentAbilityDispatcher>();
            if (zodiacCarrier == null) zodiacCarrier = FindFirstObjectByType<ZodiacCarrier>();
        }

        public void FireDown()
        {
            FirePressed?.Invoke();
            weaponRouter?.FireDown();
        }

        public void FireUp()
        {
            FireReleased?.Invoke();
            weaponRouter?.FireUp();
        }

        public void Reload()
        {
            ReloadPressed?.Invoke();
            weaponRouter?.Reload();
        }

        public void AdsDown()
        {
            adsController?.SetAds(true);
        }

        public void AdsUp()
        {
            adsController?.SetAds(false);
        }

        public void SwitchPrimary() => weaponRouter?.SwitchTo(WeaponSlot.Primary);
        public void SwitchSecondary() => weaponRouter?.SwitchTo(WeaponSlot.Secondary);
        public void SwitchMelee() => weaponRouter?.SwitchTo(WeaponSlot.Melee);

        public void UseAbility(int slot)
        {
            slot = Mathf.Clamp(slot, 0, 2);
            AbilityPressed?.Invoke(slot);
            abilityDispatcher?.UseSlot(slot);
        }

        public void InteractDown() => zodiacCarrier?.BeginInteract();
        public void InteractHeld(float deltaTime) => zodiacCarrier?.TickInteract(deltaTime);
        public void InteractUp() => zodiacCarrier?.EndInteract();
    }
}
