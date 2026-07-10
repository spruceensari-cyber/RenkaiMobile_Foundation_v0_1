using UnityEngine;
using UnityEngine.EventSystems;
using RenkaiMobile.Weapons;

namespace RenkaiMobile.UI
{
    public sealed class MobileRifleFireButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private MobileRifleController rifle;

        private void Awake()
        {
            if (rifle == null) rifle = FindFirstObjectByType<MobileRifleController>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            rifle?.BeginAutomaticFire();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            rifle?.EndAutomaticFire();
        }
    }
}
