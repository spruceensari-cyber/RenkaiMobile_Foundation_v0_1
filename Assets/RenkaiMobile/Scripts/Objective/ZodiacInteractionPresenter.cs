using UnityEngine;
using UnityEngine.UI;

namespace RenkaiMobile.Objective
{
    public sealed class ZodiacInteractionPresenter : MonoBehaviour
    {
        [SerializeField] private ZodiacObjective objective;
        [SerializeField] private Image progressRing;
        [SerializeField] private Text actionText;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip activateLoop;
        [SerializeField] private AudioClip disruptLoop;

        private bool interacting;

        private void Awake()
        {
            if (objective == null) objective = FindFirstObjectByType<ZodiacObjective>();
            if (group == null) group = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            if (objective == null) return;
            objective.StateChanged += OnStateChanged;
            objective.InteractionProgressChanged += OnProgressChanged;
        }

        private void OnDisable()
        {
            if (objective == null) return;
            objective.StateChanged -= OnStateChanged;
            objective.InteractionProgressChanged -= OnProgressChanged;
        }

        private void Update()
        {
            if (group != null)
                group.alpha = Mathf.MoveTowards(group.alpha, interacting ? 1f : 0f, 8f * Time.unscaledDeltaTime);
        }

        private void OnStateChanged(ZodiacState state)
        {
            interacting = state == ZodiacState.Activating || state == ZodiacState.Disrupting;
            if (actionText != null)
            {
                actionText.text = state switch
                {
                    ZodiacState.Activating => "ACTIVATING ZODIAC",
                    ZodiacState.Disrupting => "DISRUPTING ZODIAC",
                    ZodiacState.Active => "SIGNAL LINKED",
                    ZodiacState.Collapsed => "COLLAPSE",
                    ZodiacState.Disrupted => "ZODIAC DISRUPTED",
                    _ => string.Empty
                };
            }

            if (audioSource != null)
            {
                AudioClip desired = state == ZodiacState.Activating ? activateLoop : state == ZodiacState.Disrupting ? disruptLoop : null;
                if (desired != null)
                {
                    if (audioSource.clip != desired || !audioSource.isPlaying)
                    {
                        audioSource.clip = desired;
                        audioSource.loop = true;
                        audioSource.Play();
                    }
                }
                else if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }

        private void OnProgressChanged(float current, float required)
        {
            if (progressRing != null)
                progressRing.fillAmount = required > 0f ? Mathf.Clamp01(current / required) : 0f;
        }
    }
}
