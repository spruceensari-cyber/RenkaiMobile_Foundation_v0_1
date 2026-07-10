using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.Combat;
using RenkaiMobile.Weapons;

namespace RenkaiMobile.UI
{
    public sealed class MobileWeaponHudPresenter : MonoBehaviour
    {
        [SerializeField] private MobileWeaponRouter router;
        [SerializeField] private MobileRifleController rifle;
        [SerializeField] private MobilePistolController pistol;
        [SerializeField] private Text weaponNameText;
        [SerializeField] private Text ammoText;
        [SerializeField] private Text slotText;

        private void Awake()
        {
            if (router == null) router = FindFirstObjectByType<MobileWeaponRouter>();
        }

        private void OnEnable()
        {
            if (router != null) router.ActiveWeaponChanged += OnWeaponChanged;
            if (rifle != null) rifle.AmmoChanged += OnRifleAmmoChanged;
            if (pistol != null) pistol.AmmoChanged += OnPistolAmmoChanged;
        }

        private void OnDisable()
        {
            if (router != null) router.ActiveWeaponChanged -= OnWeaponChanged;
            if (rifle != null) rifle.AmmoChanged -= OnRifleAmmoChanged;
            if (pistol != null) pistol.AmmoChanged -= OnPistolAmmoChanged;
        }

        private void Start()
        {
            if (router != null) OnWeaponChanged(router.ActiveSlot);
        }

        private void OnWeaponChanged(WeaponSlot slot)
        {
            if (slotText != null) slotText.text = slot.ToString().ToUpperInvariant();
            switch (slot)
            {
                case WeaponSlot.Primary:
                    if (weaponNameText != null) weaponNameText.text = "KITSUNE AR";
                    break;
                case WeaponSlot.Secondary:
                    if (weaponNameText != null) weaponNameText.text = pistol != null ? pistol.DisplayName.ToUpperInvariant() : "ZERO PULSE";
                    break;
                case WeaponSlot.Melee:
                    if (weaponNameText != null) weaponNameText.text = "HIKARI BLADE";
                    if (ammoText != null) ammoText.text = "—";
                    break;
            }
        }

        private void OnRifleAmmoChanged(int current, int max)
        {
            if (router != null && router.ActiveSlot == WeaponSlot.Primary && ammoText != null)
                ammoText.text = current + " / " + max;
        }

        private void OnPistolAmmoChanged(int current, int max)
        {
            if (router != null && router.ActiveSlot == WeaponSlot.Secondary && ammoText != null)
                ammoText.text = current + " / " + max;
        }
    }
}
