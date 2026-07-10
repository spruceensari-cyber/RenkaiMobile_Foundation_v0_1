using System;
using System.Collections.Generic;
using UnityEngine;

namespace RenkaiMobile.Events
{
    public enum AnnouncementPriority
    {
        Low = 0,
        Medium = 1,
        High = 2
    }

    public readonly struct CompetitiveAnnouncement
    {
        public CompetitiveAnnouncement(string message, AnnouncementPriority priority, float duration)
        {
            Message = message;
            Priority = priority;
            Duration = duration;
        }

        public string Message { get; }
        public AnnouncementPriority Priority { get; }
        public float Duration { get; }
    }

    public sealed class CompetitiveAnnouncementQueue : MonoBehaviour
    {
        public event Action<CompetitiveAnnouncement> AnnouncementStarted;
        public event Action AnnouncementEnded;

        private readonly List<CompetitiveAnnouncement> queue = new List<CompetitiveAnnouncement>();
        private float remaining;
        private bool active;

        private void Update()
        {
            if (active)
            {
                remaining -= Time.unscaledDeltaTime;
                if (remaining <= 0f)
                {
                    active = false;
                    AnnouncementEnded?.Invoke();
                }
            }

            if (!active && queue.Count > 0) StartNext();
        }

        public void Enqueue(string message, AnnouncementPriority priority, float duration = 1.8f)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            CompetitiveAnnouncement item = new CompetitiveAnnouncement(message, priority, Mathf.Max(0.25f, duration));
            int index = queue.FindIndex(x => x.Priority < priority);
            if (index < 0) queue.Add(item);
            else queue.Insert(index, item);
        }

        public void ClearLowPriority()
        {
            queue.RemoveAll(x => x.Priority == AnnouncementPriority.Low);
        }

        private void StartNext()
        {
            CompetitiveAnnouncement next = queue[0];
            queue.RemoveAt(0);
            active = true;
            remaining = next.Duration;
            AnnouncementStarted?.Invoke(next);
        }
    }
}
