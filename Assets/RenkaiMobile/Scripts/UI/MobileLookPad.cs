using UnityEngine;
using UnityEngine.EventSystems;
using RenkaiMobile.Movement;

namespace RenkaiMobile.UI
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class MobileLookPad : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private MobileLookController lookController;
        [SerializeField] private float sensitivity = 0.018f;

        private Vector2 previousPosition;
        private bool dragging;

        private void Awake()
        {
            if (lookController == null) lookController = FindFirstObjectByType<MobileLookController>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            previousPosition = eventData.position;
            dragging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!dragging) return;
            Vector2 delta = eventData.position - previousPosition;
            previousPosition = eventData.position;
            lookController?.AddLookInput(delta * sensitivity);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            dragging = false;
        }
    }
}
