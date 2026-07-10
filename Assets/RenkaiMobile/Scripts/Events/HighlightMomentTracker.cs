using System;
using System.Collections.Generic;
using UnityEngine;

namespace RenkaiMobile.Events
{
    public enum HighlightMomentType
    {
        Ace,
        ClutchWin,
        OneVFour,
        OneVFive,
        FastTripleKill,
        LongHeadshot,
        LastSecondDisrupt,
        LastSecondCollapse
    }

    [Serializable]
    public struct HighlightMoment
    {
        public HighlightMomentType type;
        public string actorId;
        public float timestamp;
        public int roundContext;
        public Vector3 position;
    }

    public sealed class HighlightMomentTracker : MonoBehaviour
    {
        [SerializeField] private CompetitiveMomentTracker competitiveTracker;
        [SerializeField] private MobileKillEventBus killEventBus;
        [SerializeField] private float fastTripleWindow = 2.8f;

        public event Action<HighlightMoment> HighlightRecorded;
        public IReadOnlyList<HighlightMoment> Moments => moments;

        private readonly List<HighlightMoment> moments = new List<HighlightMoment>();
        private readonly Dictionary<string, Queue<float>> recentKillTimes = new Dictionary<string, Queue<float>>();

        private void Awake()
        {
            if (competitiveTracker == null) competitiveTracker = FindFirstObjectByType<CompetitiveMomentTracker>();
            if (killEventBus == null) killEventBus = FindFirstObjectByType<MobileKillEventBus>();
        }

        private void OnEnable()
        {
            if (competitiveTracker != null)
            {
                competitiveTracker.AceAchieved += OnAce;
                competitiveTracker.ClutchWon += OnClutchWon;
            }
            if (killEventBus != null) killEventBus.KillPublished += OnKill;
        }

        private void OnDisable()
        {
            if (competitiveTracker != null)
            {
                competitiveTracker.AceAchieved -= OnAce;
                competitiveTracker.ClutchWon -= OnClutchWon;
            }
            if (killEventBus != null) killEventBus.KillPublished -= OnKill;
        }

        private void OnAce(GameObject actor)
        {
            Record(HighlightMomentType.Ace, actor);
        }

        private void OnClutchWon(GameObject actor, int enemiesAtStart)
        {
            HighlightMomentType type = enemiesAtStart >= 5
                ? HighlightMomentType.OneVFive
                : enemiesAtStart >= 4
                    ? HighlightMomentType.OneVFour
                    : HighlightMomentType.ClutchWin;
            Record(type, actor);
        }

        private void OnKill(MobileKillEvent kill)
        {
            if (kill.Killer == null) return;
            string id = kill.Killer.CombatantId;
            if (!recentKillTimes.TryGetValue(id, out Queue<float> queue))
            {
                queue = new Queue<float>();
                recentKillTimes[id] = queue;
            }

            queue.Enqueue(kill.Timestamp);
            while (queue.Count > 0 && kill.Timestamp - queue.Peek() > fastTripleWindow)
                queue.Dequeue();

            if (queue.Count >= 3)
            {
                Record(HighlightMomentType.FastTripleKill, kill.Killer.gameObject);
                queue.Clear();
            }
        }

        private void Record(HighlightMomentType type, GameObject actor)
        {
            string actorId = actor != null && actor.TryGetComponent(out Combat.MobileCombatantIdentity identity)
                ? identity.CombatantId
                : actor != null ? actor.name : "unknown";

            HighlightMoment moment = new HighlightMoment
            {
                type = type,
                actorId = actorId,
                timestamp = Time.time,
                roundContext = 0,
                position = actor != null ? actor.transform.position : Vector3.zero
            };

            moments.Add(moment);
            HighlightRecorded?.Invoke(moment);
        }
    }
}
