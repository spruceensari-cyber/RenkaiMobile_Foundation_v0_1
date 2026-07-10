using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Renkai.Core;
using RenkaiMobile.Teams;

namespace RenkaiMobile.UI
{
    public sealed class SimpleMinimapController : MonoBehaviour
    {
        [SerializeField] private RectTransform mapRoot;
        [SerializeField] private Vector2 worldMin = new Vector2(-24f, -16f);
        [SerializeField] private Vector2 worldMax = new Vector2(24f, 34f);
        [SerializeField] private Vector2 mapSize = new Vector2(240f, 240f);
        [SerializeField] private float refreshInterval = 0.1f;

        private readonly Dictionary<FiveVFiveRosterAgent, RectTransform> icons = new Dictionary<FiveVFiveRosterAgent, RectTransform>();
        private float nextRefresh;

        private void Start()
        {
            BuildIcons();
        }

        private void Update()
        {
            if (Time.unscaledTime < nextRefresh) return;
            nextRefresh = Time.unscaledTime + refreshInterval;
            Refresh();
        }

        private void BuildIcons()
        {
            if (mapRoot == null) return;
            FiveVFiveRosterAgent[] roster = FindObjectsByType<FiveVFiveRosterAgent>(FindObjectsSortMode.None);
            foreach (FiveVFiveRosterAgent agent in roster)
            {
                if (agent == null || icons.ContainsKey(agent)) continue;
                GameObject go = new GameObject(agent.name + "_MapIcon", typeof(RectTransform), typeof(Image));
                go.transform.SetParent(mapRoot, false);
                RectTransform rt = go.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(12f, 12f);
                Image image = go.GetComponent<Image>();
                image.color = agent.Team == TeamId.Attackers
                    ? new Color(0.1f, 0.75f, 1f, 0.95f)
                    : new Color(0.95f, 0.2f, 0.45f, 0.95f);
                icons.Add(agent, rt);
            }
        }

        private void Refresh()
        {
            BuildIcons();
            foreach (var pair in icons)
            {
                FiveVFiveRosterAgent agent = pair.Key;
                RectTransform icon = pair.Value;
                if (agent == null || icon == null) continue;
                icon.gameObject.SetActive(agent.IsAlive);
                Vector3 p = agent.transform.position;
                float x = Mathf.InverseLerp(worldMin.x, worldMax.x, p.x);
                float y = Mathf.InverseLerp(worldMin.y, worldMax.y, p.z);
                icon.anchoredPosition = new Vector2((x - 0.5f) * mapSize.x, (y - 0.5f) * mapSize.y);
            }
        }
    }
}
