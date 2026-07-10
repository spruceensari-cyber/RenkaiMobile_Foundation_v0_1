using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.Events;

namespace RenkaiMobile.UI
{
    public sealed class CompetitiveMomentBanner : MonoBehaviour
    {
        [SerializeField] private CompetitiveMomentTracker tracker;
        [SerializeField] private Text bannerText;
        [SerializeField] private float visibleSeconds = 1.8f;

        private Coroutine hideRoutine;

        private void Awake()
        {
            if (tracker == null) tracker = FindFirstObjectByType<CompetitiveMomentTracker>();
        }

        private void OnEnable()
        {
            if (tracker == null) return;
            tracker.AceAchieved += OnAce;
            tracker.ClutchStateEntered += OnClutch;
            tracker.HeadshotStreakChanged += OnHeadshotStreak;
        }

        private void OnDisable()
        {
            if (tracker == null) return;
            tracker.AceAchieved -= OnAce;
            tracker.ClutchStateEntered -= OnClutch;
            tracker.HeadshotStreakChanged -= OnHeadshotStreak;
        }

        private void OnAce(GameObject player) => Show("ACE — " + player.name);
        private void OnClutch(GameObject player, int enemies) => Show("CLUTCH 1v" + enemies + " — " + player.name);

        private void OnHeadshotStreak(GameObject player, int streak)
        {
            if (streak >= 3) Show("HEADSHOT STREAK x" + streak + " — " + player.name);
        }

        private void Show(string message)
        {
            if (bannerText == null) return;
            bannerText.text = message;
            bannerText.gameObject.SetActive(true);
            if (hideRoutine != null) StopCoroutine(hideRoutine);
            hideRoutine = StartCoroutine(HideLater());
        }

        private IEnumerator HideLater()
        {
            yield return new WaitForSecondsRealtime(visibleSeconds);
            if (bannerText != null) bannerText.gameObject.SetActive(false);
            hideRoutine = null;
        }
    }
}
