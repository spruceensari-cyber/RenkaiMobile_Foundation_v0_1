using UnityEngine;
using UnityEngine.EventSystems;

namespace Renkai.Input
{
    public sealed class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform knob;
        [SerializeField, Range(0.2f, 1f)] private float radiusScale = 0.85f;

        public Vector2 Value { get; private set; }

        public void OnPointerDown(PointerEventData eventData) => UpdateValue(eventData);

        public void OnDrag(PointerEventData eventData) => UpdateValue(eventData);

        public void OnPointerUp(PointerEventData eventData)
        {
            Value = Vector2.zero;
            if (knob != null)
                knob.anchoredPosition = Vector2.zero;
        }

        private void UpdateValue(PointerEventData eventData)
        {
            if (background == null)
                return;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    background, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
                return;

            Vector2 half = background.rect.size * 0.5f;
            Vector2 normalized = new Vector2(
                half.x > 0f ? localPoint.x / half.x : 0f,
                half.y > 0f ? localPoint.y / half.y : 0f);

            Value = Vector2.ClampMagnitude(normalized, 1f);

            if (knob != null)
                knob.anchoredPosition = new Vector2(
                    Value.x * half.x * radiusScale,
                    Value.y * half.y * radiusScale);
        }
    }
}
