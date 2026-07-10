using System.Collections.Generic;
using UnityEngine;

namespace RenkaiMobile.Bots
{
    public sealed class CoverRegistry : MonoBehaviour
    {
        private readonly List<CoverPoint> points = new List<CoverPoint>();
        public IReadOnlyList<CoverPoint> Points => points;

        private void Awake()
        {
            Refresh();
        }

        public void Refresh()
        {
            points.Clear();
            CoverPoint[] found = FindObjectsByType<CoverPoint>(FindObjectsSortMode.None);
            foreach (CoverPoint point in found)
                if (point != null) points.Add(point);
        }

        public CoverPoint FindBest(Vector3 botPosition, Vector3 threatPosition, float maxDistance)
        {
            CoverPoint best = null;
            float bestScore = float.MaxValue;
            float maxSqr = maxDistance * maxDistance;

            foreach (CoverPoint point in points)
            {
                if (point == null || point.Occupied) continue;
                float sqr = (point.transform.position - botPosition).sqrMagnitude;
                if (sqr > maxSqr) continue;

                Vector3 toThreat = (threatPosition - point.transform.position).normalized;
                float exposure = Vector3.Dot(point.transform.forward, toThreat);
                float score = sqr * 0.02f + point.DangerScore * 12f + Mathf.Max(0f, exposure) * 8f;
                if (score < bestScore)
                {
                    bestScore = score;
                    best = point;
                }
            }
            return best;
        }
    }
}
