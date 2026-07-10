using UnityEngine;
using UnityEngine.EventSystems;
using RenkaiMobile.Combat;

namespace RenkaiMobile.UI
{
    public sealed class MobileAdsButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private MobileAdsController adsController;

        private void Awake()
        {
            if (adsController == null) adsController = FindFirstObjectByType<MobileAdsController>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (adsController == null) return;
            adsController.SetAds(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (adsController == null) return;
            adsController.SetAds(false);
        }

        public void Toggle()
        {
            adsController?.ToggleAds();
        }
    }
}
