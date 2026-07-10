using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.Combat;

namespace RenkaiMobile.Presentation
{
    public sealed class DeathRecapPresenter : MonoBehaviour
    {
        [SerializeField] private MobileHealth localHealth;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private Text killerText;
        [SerializeField] private Text damageText;
        [SerializeField] private Text headshotText;
        [SerializeField] private float visibleSeconds = 2.8f;

        private Coroutine routine;

        private void Awake()
        {
            if (localHealth == null)
            {
                MobileCombatantIdentity[] identities = FindObjectsByType<MobileCombatantIdentity>(FindObjectsSortMode.None);
                foreach (MobileCombatantIdentity identity in identities)
                {
                    if (identity != null && identity.PlayerControlled)
                    {
                        localHealth = identity.GetComponent<MobileHealth>();
                        break;
                    }
                }
            }
            if (group != null) group.alpha = 0f;
        }

        private void OnEnable()
        {
            if (localHealth != null) localHealth.Died += OnDied;
        }

        private void OnDisable()
        {
            if (localHealth != null) localHealth.Died -= OnDied;
        }

        private void OnDied(MobileDamageInfo damage)
        {
            MobileCombatantIdentity killer = damage.Instigator != null
                ? damage.Instigator.GetComponentInParent<MobileCombatantIdentity>()
                : null;

            if (killerText != null) killerText.text = killer != null ? "ELIMINATED BY // " + killer.DisplayName.ToUpperInvariant() : "ELIMINATED // ENVIRONMENT";
            if (damageText != null) damageText.text = "FINAL IMPACT // " + Mathf.RoundToInt(damage.Amount);
            if (headshotText != null) headshotText.text = damage.Headshot ? "HEADSHOT SIGNAL" : string.Empty;

            if (routine != null) StopCoroutine(routine);
            routine = StartCoroutine(ShowRoutine());
        }

        private IEnumerator ShowRoutine()
        {
            if (group != null) group.alpha = 1f;
            yield return new WaitForSecondsRealtime(visibleSeconds);
            if (group != null) group.alpha = 0f;
            routine = null;
        }
    }
}
