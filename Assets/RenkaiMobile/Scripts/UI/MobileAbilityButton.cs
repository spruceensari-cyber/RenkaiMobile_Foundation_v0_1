using UnityEngine;
using UnityEngine.EventSystems;
using RenkaiMobile.Abilities;

namespace RenkaiMobile.UI
{
    public sealed class MobileAbilityButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private AbilityRuntimeController controller;
        [SerializeField, Range(0, 2)] private int slot;

        private void Awake()
        {
            if (controller == null) controller = FindFirstObjectByType<AbilityRuntimeController>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            controller?.TryUseAbility(slot);
        }

        public void Configure(AbilityRuntimeController runtimeController, int abilitySlot)
        {
            controller = runtimeController;
            slot = Mathf.Clamp(abilitySlot, 0, 2);
        }
    }
}
