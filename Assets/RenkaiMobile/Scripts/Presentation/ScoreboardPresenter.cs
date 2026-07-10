using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.Combat;
using RenkaiMobile.Events;

namespace RenkaiMobile.Presentation
{
    public sealed class ScoreboardPresenter : MonoBehaviour
    {
        [SerializeField] private MatchStatsTracker statsTracker;
        [SerializeField] private Text scoreboardText;
        [SerializeField] private GameObject root;

        private void Awake()
        {
            if (statsTracker == null) statsTracker = FindFirstObjectByType<MatchStatsTracker>();
            if (root == null) root = gameObject;
        }

        public void Show()
        {
            if (root != null) root.SetActive(true);
            Refresh();
        }

        public void Hide()
        {
            if (root != null) root.SetActive(false);
        }

        public void Refresh()
        {
            if (scoreboardText == null || statsTracker == null) return;
            IEnumerable<PlayerMatchStats> ordered = statsTracker.AllStats.OrderByDescending(x => x.kills).ThenByDescending(x => x.damageDealt);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("RESONANCE SCOREBOARD");
            sb.AppendLine("PLAYER            K   HS   A   DMG");
            foreach (PlayerMatchStats s in ordered)
            {
                string name = s.identity != null ? s.identity.DisplayName : "UNKNOWN";
                sb.AppendLine($"{name,-16} {s.kills,2}  {s.headshots,2}  {s.assists,2}  {Mathf.RoundToInt(s.damageDealt),4}");
            }
            scoreboardText.text = sb.ToString();
        }
    }
}
