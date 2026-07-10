using System;
using UnityEngine;

namespace RenkaiMobile.Combat
{
    public enum WeaponSlot
    {
        Primary = 0,
        Secondary = 1,
        Melee = 2
    }

    public sealed class WeaponInventoryController : MonoBehaviour
    {
        [SerializeField] private GameObject primaryView;
        [SerializeField] private GameObject secondaryView;
        [SerializeField] private GameObject meleeView;
        [SerializeField] private WeaponSlot startingSlot = WeaponSlot.Primary;
        [SerializeField] private float switchLockSeconds = 0.35f;

        public event Action<WeaponSlot> WeaponChanged;
        public WeaponSlot CurrentSlot { get; private set; }

        private float nextSwitchTime;

        private void Start()
        {
            ForceSwitch(startingSlot);
        }

        public bool TrySwitch(WeaponSlot slot)
        {
            if (Time.time < nextSwitchTime || slot == CurrentSlot) return false;
            nextSwitchTime = Time.time + switchLockSeconds;
            ForceSwitch(slot);
            return true;
        }

        public void NextWeapon()
        {
            int next = ((int)CurrentSlot + 1) % 3;
            TrySwitch((WeaponSlot)next);
        }

        public void PreviousWeapon()
        {
            int previous = ((int)CurrentSlot + 2) % 3;
            TrySwitch((WeaponSlot)previous);
        }

        public void EquipPrimary() => TrySwitch(WeaponSlot.Primary);
        public void EquipSecondary() => TrySwitch(WeaponSlot.Secondary);
        public void EquipMelee() => TrySwitch(WeaponSlot.Melee);

        private void ForceSwitch(WeaponSlot slot)
        {
            CurrentSlot = slot;
            if (primaryView != null) primaryView.SetActive(slot == WeaponSlot.Primary);
            if (secondaryView != null) secondaryView.SetActive(slot == WeaponSlot.Secondary);
            if (meleeView != null) meleeView.SetActive(slot == WeaponSlot.Melee);
            WeaponChanged?.Invoke(slot);
        }
    }
}
