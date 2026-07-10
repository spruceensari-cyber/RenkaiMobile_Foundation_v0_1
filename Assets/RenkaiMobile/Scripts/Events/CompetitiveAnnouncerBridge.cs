using UnityEngine;

namespace RenkaiMobile.Events
{
    public sealed class CompetitiveAnnouncerBridge : MonoBehaviour
    {
        [SerializeField] private CompetitiveMomentTracker tracker;
        [SerializeField] private CompetitiveAnnouncementQueue queue;

        private void Awake()
        {
            if (tracker == null) tracker = FindFirstObjectByType<CompetitiveMomentTracker>();
            if (queue == null) queue = FindFirstObjectByType<CompetitiveAnnouncementQueue>();
        }

        private void OnEnable()
        {
            if (tracker == null) return;
            tracker.AceAchieved += OnAce;
            tracker.ClutchStateEntered += OnClutchState;
            tracker.HeadshotStreakChanged += OnHeadshotStreak;
        }

        private void OnDisable()
        {
            if (tracker == null) return;
            tracker.AceAchieved -= OnAce;
            tracker.ClutchStateEntered -= OnClutchState;
            tracker.HeadshotStreakChanged -= OnHeadshotStreak;
        }

        private void OnAce(GameObject actor)
        {
            queue?.ClearLowPriority();
            queue?.Enqueue("ACE — " + actor.name, AnnouncementPriority.High, 2.4f);
        }

        private void OnClutchState(GameObject actor, int enemies)
        {
            queue?.Enqueue("LAST UNIT // 1v" + enemies, AnnouncementPriority.Medium, 1.6f);
        }

        private void OnHeadshotStreak(GameObject actor, int streak)
        {
            if (streak < 3) return;
            queue?.Enqueue("HEADSHOT SIGNAL x" + streak, AnnouncementPriority.Low, 1.2f);
        }
    }
}
