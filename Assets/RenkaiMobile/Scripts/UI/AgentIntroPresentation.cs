using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.Combat;

namespace RenkaiMobile.UI
{
    public sealed class AgentIntroPresentation : MonoBehaviour
    {
        [SerializeField] private CanvasGroup rootGroup;
        [SerializeField] private Text agentNameText;
        [SerializeField] private Text playerNameText;
        [SerializeField] private Text statusText;
        [SerializeField] private RectTransform scanBar;
        [SerializeField] private float showSeconds = 2.4f;
        [SerializeField] private float fadeSpeed = 4f;

        private Coroutine routine;
        private bool visible;

        private void Update()
        {
            if (rootGroup != null)
                rootGroup.alpha = Mathf.MoveTowards(rootGroup.alpha, visible ? 1f : 0f, fadeSpeed * Time.unscaledDeltaTime);

            if (scanBar != null && visible)
            {
                Vector2 p = scanBar.anchoredPosition;
                p.x = Mathf.PingPong(Time.unscaledTime * 220f, 440f) - 220f;
                scanBar.anchoredPosition = p;
            }
        }

        public void Present(MobileCombatantIdentity identity)
        {
            if (identity == null) return;
            if (routine != null) StopCoroutine(routine);
            routine = StartCoroutine(PresentRoutine(identity));
        }

        private IEnumerator PresentRoutine(MobileCombatantIdentity identity)
        {
            if (agentNameText != null) agentNameText.text = identity.AgentId.ToUpperInvariant();
            if (playerNameText != null) playerNameText.text = identity.DisplayName.ToUpperInvariant();
            if (statusText != null) statusText.text = "RESONANCE LINK // READY";
            visible = true;
            yield return new WaitForSecondsRealtime(showSeconds);
            visible = false;
            routine = null;
        }
    }
}
