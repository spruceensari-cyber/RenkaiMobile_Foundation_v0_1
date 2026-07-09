using UnityEngine;
using UnityEngine.EventSystems;

namespace Renkai.Input
{
    public sealed class TouchLookArea : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private float sensitivity = 0.12f;

        private Vector2 accumulatedDelta;
        private Vector2 lastPosition;
        private bool tracking;

        public void OnPointerDown(PointerEventData eventData)
        {
            tracking = true;
            lastPosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!tracking)
                return;

            Vector2 delta = eventData.position - lastPosition;
            lastPosition = eventData.position;
            accumulatedDelta += delta * sensitivity;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            tracking = false;
        }

        public Vector2 ConsumeDelta()
        {
            Vector2 value = accumulatedDelta;
            accumulatedDelta = Vector2.zero;
            return value;
        }
    }
}
