using System;
using UnityEngine;
using RenkaiMobile.Combat;

namespace RenkaiMobile.Weapons
{
    public sealed class MobileReloadState : MonoBehaviour
    {
        [SerializeField] private MobileWeaponRouter weaponRouter;
        [SerializeField] private MobileRifleController rifle;
        [SerializeField] private MobilePistolController pistol;
        [SerializeField] private MobileAdsController adsController;

        public event Action<WeaponSlot> ReloadStarted;
        public event Action<WeaponSlot> ReloadFinished;

        public bool IsReloading { get; private set; }
        public WeaponSlot ReloadingSlot { get; private set; }

        private void Awake()
        {
            if (weaponRouter == null) weaponRouter = FindFirstObjectByType<MobileWeaponRouter>();
            if (adsController == null) adsController = FindFirstObjectByType<MobileAdsController>();
        }

        private void OnEnable()
        {
            if (rifle != null)
            {
                rifle.ReloadStarted += OnRifleReloadStarted;
                rifle.ReloadFinished += OnRifleReloadFinished;
            }

            if (pistol != null)
            {
                pistol.ReloadStarted += OnPistolReloadStarted;
                pistol.ReloadFinished += OnPistolReloadFinished;
            }
        }

        private void OnDisable()
        {
            if (rifle != null)
            {
                rifle.ReloadStarted -= OnRifleReloadStarted;
                rifle.ReloadFinished -= OnRifleReloadFinished;
            }

            if (pistol != null)
            {
                pistol.ReloadStarted -= OnPistolReloadStarted;
                pistol.ReloadFinished -= OnPistolReloadFinished;
            }
        }

        private void OnRifleReloadStarted() => Begin(WeaponSlot.Primary);
        private void OnPistolReloadStarted() => Begin(WeaponSlot.Secondary);
        private void OnRifleReloadFinished() => Finish(WeaponSlot.Primary);
        private void OnPistolReloadFinished() => Finish(WeaponSlot.Secondary);

        private void Begin(WeaponSlot slot)
        {
            IsReloading = true;
            ReloadingSlot = slot;
            adsController?.CancelAds();
            ReloadStarted?.Invoke(slot);
        }

        private void Finish(WeaponSlot slot)
        {
            if (!IsReloading || ReloadingSlot != slot) return;
            IsReloading = false;
            ReloadFinished?.Invoke(slot);
        }
    }
}
