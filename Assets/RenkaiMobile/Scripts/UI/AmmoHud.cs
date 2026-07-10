using UnityEngine;
using UnityEngine.UI;
using Renkai.Combat;

namespace RenkaiMobile.UI
{
    public sealed class AmmoHud : MonoBehaviour
    {
        [SerializeField] private HitscanWeapon weapon;
        [SerializeField] private Text ammoText;

        private void Start()
        {
            if (weapon == null) weapon = Object.FindFirstObjectByType<HitscanWeapon>();
            if (weapon != null)
            {
                weapon.AmmoChanged += OnAmmoChanged;
                OnAmmoChanged(weapon.AmmoInMagazine, weapon.MagazineSize);
            }
        }

        private void OnDestroy()
        {
            if (weapon != null) weapon.AmmoChanged -= OnAmmoChanged;
        }

        private void OnAmmoChanged(int current, int max)
        {
            if (ammoText != null) ammoText.text = current + " / " + max;
        }
    }
}
