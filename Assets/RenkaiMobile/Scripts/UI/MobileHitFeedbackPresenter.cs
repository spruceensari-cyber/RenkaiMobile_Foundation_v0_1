using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.Combat;

namespace RenkaiMobile.UI
{
    public sealed class MobileHitFeedbackPresenter : MonoBehaviour
    {
        [SerializeField] private MobileHitFeedbackBus bus;
        [SerializeField] private CrosshairController crosshair;
        [SerializeField] private Text killText;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip bodyHitClip;
        [SerializeField] private AudioClip headshotClip;
        [SerializeField] private AudioClip killClip;
        [SerializeField] private float killTextSeconds = 0.55f;

        private Coroutine hideRoutine;

        private void Awake()
        {
            if (bus == null) bus = FindFirstObjectByType<MobileHitFeedbackBus>();
            if (crosshair == null) crosshair = FindFirstObjectByType<CrosshairController>();
        }

        private void OnEnable()
        {
            if (bus != null) bus.HitConfirmed += OnHitConfirmed;
        }

        private void OnDisable()
        {
            if (bus != null) bus.HitConfirmed -= OnHitConfirmed;
        }

        private void OnHitConfirmed(MobileHitFeedbackEvent hit)
        {
            crosshair?.ShowHitMarker(hit.Headshot);
            Play(hit.Killed ? killClip : hit.Headshot ? headshotClip : bodyHitClip);

            if (hit.Killed && killText != null)
            {
                killText.text = hit.Headshot ? "HEADSHOT ELIMINATION" : "ELIMINATION";
                killText.gameObject.SetActive(true);
                if (hideRoutine != null) StopCoroutine(hideRoutine);
                hideRoutine = StartCoroutine(HideKillText());
            }
        }

        private void Play(AudioClip clip)
        {
            if (audioSource != null && clip != null) audioSource.PlayOneShot(clip);
        }

        private IEnumerator HideKillText()
        {
            yield return new WaitForSecondsRealtime(killTextSeconds);
            if (killText != null) killText.gameObject.SetActive(false);
            hideRoutine = null;
        }
    }
}
