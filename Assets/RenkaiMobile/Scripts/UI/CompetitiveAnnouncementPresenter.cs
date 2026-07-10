using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.Events;

namespace RenkaiMobile.UI
{
    public sealed class CompetitiveAnnouncementPresenter : MonoBehaviour
    {
        [SerializeField] private CompetitiveAnnouncementQueue queue;
        [SerializeField] private Text announcementText;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private float fadeSpeed = 8f;

        private bool visible;

        private void Awake()
        {
            if (queue == null) queue = FindFirstObjectByType<CompetitiveAnnouncementQueue>();
            if (group == null) group = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            if (queue == null) return;
            queue.AnnouncementStarted += OnStarted;
            queue.AnnouncementEnded += OnEnded;
        }

        private void OnDisable()
        {
            if (queue == null) return;
            queue.AnnouncementStarted -= OnStarted;
            queue.AnnouncementEnded -= OnEnded;
        }

        private void Update()
        {
            if (group == null) return;
            group.alpha = Mathf.MoveTowards(group.alpha, visible ? 1f : 0f, fadeSpeed * Time.unscaledDeltaTime);
        }

        private void OnStarted(CompetitiveAnnouncement announcement)
        {
            if (announcementText != null) announcementText.text = announcement.Message;
            visible = true;
        }

        private void OnEnded()
        {
            visible = false;
        }
    }
}
