using UnityEngine;
using UnityEngine.EventSystems;
using RenkaiMobile.Movement;

namespace RenkaiMobile.UI
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class MobileVirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private MobilePlayerMotor motor;
        [SerializeField] private RectTransform knob;
        [SerializeField, Range(0.2f, 1f)] private float knobTravel = 0.55f;

        private RectTransform pad;

        private void Awake()
        {
            pad = GetComponent<RectTransform>();
            if (motor == null) motor = FindFirstObjectByType<MobilePlayerMotor>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            UpdateInput(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            UpdateInput(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            motor?.SetMoveInput(Vector2.zero);
            if (knob != null) knob.anchoredPosition = Vector2.zero;
        }

        private void UpdateInput(PointerEventData eventData)
        {
            if (pad == null) return;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(pad, eventData.position, eventData.pressEventCamera, out Vector2 local)) return;

            Vector2 radius = pad.rect.size * 0.5f;
            Vector2 normalized = new Vector2(
                radius.x > 0f ? local.x / radius.x : 0f,
                radius.y > 0f ? local.y / radius.y : 0f);
            normalized = Vector2.ClampMagnitude(normalized, 1f);

            motor?.SetMoveInput(normalized);
            if (knob != null)
                knob.anchoredPosition = Vector2.Scale(normalized, radius) * knobTravel;
        }
    }
}
