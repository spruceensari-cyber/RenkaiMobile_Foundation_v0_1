using UnityEngine;
using UnityEngine.EventSystems;
using RenkaiMobile.Combat;

namespace RenkaiMobile.UI
{
    public sealed class MobileWeaponSwitchButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private WeaponInventoryController inventory;
        [SerializeField] private WeaponSlot targetSlot = WeaponSlot.Primary;

        private void Awake()
        {
            if (inventory == null) inventory = FindFirstObjectByType<WeaponInventoryController>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            inventory?.TrySwitch(targetSlot);
        }

        public void Configure(WeaponInventoryController controller, WeaponSlot slot)
        {
            inventory = controller;
            targetSlot = slot;
        }
    }
}
