using UnityEngine;
using UnityEngine.EventSystems;
using RenkaiMobile.Presentation;

namespace RenkaiMobile.UI
{
    public sealed class MobileScoreboardButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private ScoreboardPresenter scoreboard;

        private void Awake()
        {
            if (scoreboard == null) scoreboard = FindFirstObjectByType<ScoreboardPresenter>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            scoreboard?.Show();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            scoreboard?.Hide();
        }
    }
}
