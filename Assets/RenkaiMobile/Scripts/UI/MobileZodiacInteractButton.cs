using UnityEngine;
using UnityEngine.EventSystems;
using RenkaiMobile.Objective;

namespace RenkaiMobile.UI
{
    public sealed class MobileZodiacInteractButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private ZodiacCarrier carrier;
        [SerializeField] private GameObject visualRoot;

        private void Awake()
        {
            if (carrier == null) carrier = FindFirstObjectByType<ZodiacCarrier>();
            if (visualRoot == null) visualRoot = gameObject;
        }

        private void Update()
        {
            if (visualRoot != null && carrier != null)
                visualRoot.SetActive(carrier.CanInteract());
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            carrier?.BeginInteract();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            carrier?.EndInteract();
        }
    }
}
