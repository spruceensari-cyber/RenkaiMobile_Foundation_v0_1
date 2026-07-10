using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.Stats;

namespace RenkaiMobile.UI
{
    public sealed class HolographicMvpPresenter : MonoBehaviour
    {
        [SerializeField] private CanvasGroup rootGroup;
        [SerializeField] private Text titleText;
        [SerializeField] private Text playerNameText;
        [SerializeField] private Text agentText;
        [SerializeField] private Text statLineText;
        [SerializeField] private Graphic[] glowGraphics;
        [SerializeField] private float fadeSpeed = 2.8f;

        private bool visible;

        private void Update()
        {
            if (rootGroup == null) return;
            rootGroup.alpha = Mathf.MoveTowards(rootGroup.alpha, visible ? 1f : 0f, fadeSpeed * Time.unscaledDeltaTime);
        }

        public void Show(PlayerMatchStats stats)
        {
            if (stats == null || stats.identity == null) return;
            visible = true;
            if (titleText != null) titleText.text = "MATCH RESONANCE // MVP";
            if (playerNameText != null) playerNameText.text = stats.identity.DisplayName.ToUpperInvariant();
            if (agentText != null) agentText.text = stats.identity.AgentId.ToUpperInvariant();
            if (statLineText != null)
            {
                statLineText.text = stats.kills + " KILLS  //  "
                    + stats.headshots + " HEADSHOTS  //  "
                    + stats.clutchWins + " CLUTCH  //  "
                    + Mathf.RoundToInt(stats.damageDealt) + " DAMAGE";
            }

            if (glowGraphics != null)
            {
                for (int i = 0; i < glowGraphics.Length; i++)
                {
                    Graphic graphic = glowGraphics[i];
                    if (graphic == null) continue;
                    graphic.color = i % 2 == 0
                        ? new Color(0.1f, 0.75f, 1f, 0.95f)
                        : new Color(0.75f, 0.2f, 1f, 0.95f);
                }
            }
        }

        public void Hide()
        {
            visible = false;
        }
    }
}
