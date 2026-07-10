using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.Objective;
using RenkaiMobile.Rounds;

namespace RenkaiMobile.UI
{
    public sealed class PremiumMatchHudLayer : MonoBehaviour
    {
        [SerializeField] private MobileRoundManager roundManager;
        [SerializeField] private ZodiacObjective zodiacObjective;
        [SerializeField] private Text phaseText;
        [SerializeField] private Text timerText;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text zodiacText;
        [SerializeField] private Image threatPulse;
        [SerializeField] private RectTransform horizonLine;
        [SerializeField] private float pulseSpeed = 3f;

        private void Awake()
        {
            if (roundManager == null) roundManager = FindFirstObjectByType<MobileRoundManager>();
            if (zodiacObjective == null) zodiacObjective = FindFirstObjectByType<ZodiacObjective>();
        }

        private void Update()
        {
            if (roundManager != null)
            {
                if (phaseText != null) phaseText.text = "PHASE // " + roundManager.Phase.ToString().ToUpperInvariant();
                if (timerText != null) timerText.text = Mathf.CeilToInt(roundManager.RemainingSeconds).ToString("00");
                if (scoreText != null) scoreText.text = roundManager.AttackersScore + "  //  " + roundManager.DefendersScore;
            }

            if (zodiacObjective != null && zodiacText != null)
            {
                zodiacText.text = zodiacObjective.State switch
                {
                    ZodiacState.Dormant => "ZODIAC // DORMANT",
                    ZodiacState.Carried => "ZODIAC // LINKED",
                    ZodiacState.Activating => "ZODIAC // ACTIVATING",
                    ZodiacState.Active => "ZODIAC // ONLINE",
                    ZodiacState.Disrupting => "ZODIAC // DISRUPTION",
                    ZodiacState.Collapsed => "ZODIAC // COLLAPSE",
                    ZodiacState.Disrupted => "ZODIAC // DISRUPTED",
                    _ => "ZODIAC"
                };
            }

            float pulse = 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * pulseSpeed);
            if (threatPulse != null)
            {
                Color c = threatPulse.color;
                c.a = 0.18f + pulse * 0.25f;
                threatPulse.color = c;
            }

            if (horizonLine != null)
                horizonLine.anchoredPosition = new Vector2(Mathf.Sin(Time.unscaledTime * 0.7f) * 2f, horizonLine.anchoredPosition.y);
        }
    }
}
