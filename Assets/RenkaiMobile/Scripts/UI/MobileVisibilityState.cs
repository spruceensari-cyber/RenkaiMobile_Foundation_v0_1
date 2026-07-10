using UnityEngine;

namespace RenkaiMobile.UI
{
    public sealed class MobileVisibilityState : MonoBehaviour
    {
        [SerializeField] private float defaultRevealSeconds = 3.5f;

        public float RevealedUntil { get; private set; }
        public bool IsRevealed => Time.time <= RevealedUntil;

        public void Reveal(float seconds = -1f)
        {
            float duration = seconds > 0f ? seconds : defaultRevealSeconds;
            RevealedUntil = Mathf.Max(RevealedUntil, Time.time + duration);
        }

        public void ClearReveal()
        {
            RevealedUntil = 0f;
        }
    }
}
