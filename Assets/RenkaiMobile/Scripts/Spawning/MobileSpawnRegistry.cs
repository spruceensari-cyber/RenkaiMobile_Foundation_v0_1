using System.Collections.Generic;
using UnityEngine;
using RenkaiMobile.Core;

namespace RenkaiMobile.Spawning
{
    public sealed class MobileSpawnRegistry : MonoBehaviour
    {
        private readonly Dictionary<MobileTeamId, List<MobileSpawnPoint>> points = new Dictionary<MobileTeamId, List<MobileSpawnPoint>>();

        private void Awake()
        {
            Refresh();
        }

        public void Refresh()
        {
            points.Clear();
            MobileSpawnPoint[] found = FindObjectsByType<MobileSpawnPoint>(FindObjectsSortMode.None);
            foreach (MobileSpawnPoint point in found)
            {
                if (point == null || point.Team == MobileTeamId.None) continue;
                if (!points.TryGetValue(point.Team, out List<MobileSpawnPoint> list))
                {
                    list = new List<MobileSpawnPoint>();
                    points[point.Team] = list;
                }
                list.Add(point);
            }

            foreach (List<MobileSpawnPoint> list in points.Values)
                list.Sort((a, b) => a.SlotIndex.CompareTo(b.SlotIndex));
        }

        public bool TryGet(MobileTeamId team, int slotIndex, out MobileSpawnPoint point)
        {
            point = null;
            if (!points.TryGetValue(team, out List<MobileSpawnPoint> list) || list.Count == 0) return false;
            int index = Mathf.Clamp(slotIndex, 0, list.Count - 1);
            point = list[index];
            return point != null;
        }
    }
}
