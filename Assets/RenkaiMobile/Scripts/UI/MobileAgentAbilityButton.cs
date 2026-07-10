using UnityEngine;
using UnityEngine.EventSystems;
using RenkaiMobile.Abilities;

namespace RenkaiMobile.UI
{
    public sealed class MobileAgentAbilityButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private AgentAbilityDispatcher dispatcher;
        [SerializeField, Range(0, 2)] private int slot;

        private void Awake()
        {
            if (dispatcher == null) dispatcher = FindFirstObjectByType<AgentAbilityDispatcher>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            dispatcher?.UseSlot(slot);
        }

        public void Configure(AgentAbilityDispatcher value, int abilitySlot)
        {
            dispatcher = value;
            slot = Mathf.Clamp(abilitySlot, 0, 2);
        }
    }
}
