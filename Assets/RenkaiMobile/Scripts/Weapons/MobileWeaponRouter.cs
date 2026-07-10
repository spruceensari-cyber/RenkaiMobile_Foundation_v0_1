using System;
using UnityEngine;
using RenkaiMobile.Combat;

namespace RenkaiMobile.Weapons
{
    public sealed class MobileWeaponRouter : MonoBehaviour
    {
        [SerializeField] private WeaponInventoryController inventory;
        [SerializeField] private MobileRifleController rifle;
        [SerializeField] private MobilePistolController pistol;
        [SerializeField] private MobileMeleeController melee;
        [SerializeField] private MobileAdsController ads;
        [SerializeField] private MobileRecoilDriver recoilDriver;

        public event Action<WeaponSlot> ActiveWeaponChanged;

        public WeaponSlot ActiveSlot => inventory != null ? inventory.CurrentSlot : WeaponSlot.Primary;

        private void Awake()
        {
            if (inventory == null) inventory = GetComponent<WeaponInventoryController>();
            if (ads == null) ads = GetComponent<MobileAdsController>();
            if (recoilDriver == null) recoilDriver = GetComponent<MobileRecoilDriver>();
        }

        private void OnEnable()
        {
            if (inventory != null) inventory.WeaponChanged += OnWeaponChanged;
        }

        private void OnDisable()
        {
            if (inventory != null) inventory.WeaponChanged -= OnWeaponChanged;
        }

        public void FireDown()
        {
            switch (ActiveSlot)
            {
                case WeaponSlot.Primary:
                    rifle?.BeginAutomaticFire();
                    break;
                case WeaponSlot.Secondary:
                    pistol?.TryFire();
                    break;
                case WeaponSlot.Melee:
                    melee?.TrySlash();
                    break;
            }
        }

        public void FireUp()
        {
            if (ActiveSlot == WeaponSlot.Primary) rifle?.EndAutomaticFire();
        }

        public void Reload()
        {
            if (ActiveSlot == WeaponSlot.Primary) rifle?.TryReload();
            else if (ActiveSlot == WeaponSlot.Secondary) pistol?.TryReload();
        }

        public void SwitchTo(WeaponSlot slot)
        {
            inventory?.TrySwitch(slot);
        }

        private void OnWeaponChanged(WeaponSlot slot)
        {
            rifle?.EndAutomaticFire();
            ads?.CancelAds();
            recoilDriver?.ResetPattern();
            SetControllerState(slot);
            ActiveWeaponChanged?.Invoke(slot);
        }

        private void SetControllerState(WeaponSlot slot)
        {
            if (rifle != null) rifle.enabled = slot == WeaponSlot.Primary;
            if (pistol != null) pistol.enabled = slot == WeaponSlot.Secondary;
            if (melee != null) melee.enabled = slot == WeaponSlot.Melee;
        }
    }
}
