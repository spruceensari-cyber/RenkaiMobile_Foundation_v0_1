using System.Collections;
using UnityEngine;
using RenkaiMobile.Combat;

namespace RenkaiMobile.Abilities
{
    public sealed class RaikaAbilityKit : MonoBehaviour
    {
        [SerializeField] private AbilityRuntimeController runtime;
        [SerializeField] private MobileDamagePolicy damagePolicy;
        [SerializeField] private float surgeDistance = 5.5f;
        [SerializeField] private float surgeSeconds = 0.16f;
        [SerializeField] private float pulseRadius = 6f;
        [SerializeField] private float pulseDamage = 28f;

        private bool surging;

        private void Awake()
        {
            if (runtime == null) runtime = GetComponent<AbilityRuntimeController>();
            if (damagePolicy == null) damagePolicy = FindFirstObjectByType<MobileDamagePolicy>();
        }

        public void UseVelocitySurge()
        {
            if (surging || runtime == null || !runtime.TryUseAbility(0)) return;
            StartCoroutine(SurgeRoutine());
        }

        public void UseArcPulse()
        {
            if (runtime == null || !runtime.TryUseAbility(1)) return;
            Collider[] hits = Physics.OverlapSphere(transform.position, pulseRadius, ~0, QueryTriggerInteraction.Ignore);
            foreach (Collider hit in hits)
            {
                MobileHealth health = hit.GetComponentInParent<MobileHealth>();
                if (health == null || health.gameObject == gameObject) continue;
                if (damagePolicy != null && !damagePolicy.CanDamage(gameObject, health.gameObject)) continue;
                Vector3 direction = (health.transform.position - transform.position).normalized;
                health.ApplyDamage(new MobileDamageInfo(pulseDamage, health.transform.position, direction, gameObject, false));
            }
        }

        public void UseOverdrive()
        {
            if (runtime == null || !runtime.TryUseAbility(2)) return;
            StartCoroutine(OverdriveRoutine());
        }

        private IEnumerator SurgeRoutine()
        {
            surging = true;
            Vector3 start = transform.position;
            Vector3 end = start + transform.forward * surgeDistance;
            float t = 0f;
            while (t < surgeSeconds)
            {
                t += Time.deltaTime;
                transform.position = Vector3.Lerp(start, end, Mathf.Clamp01(t / surgeSeconds));
                yield return null;
            }
            surging = false;
        }

        private IEnumerator OverdriveRoutine()
        {
            float duration = 5f;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                transform.position += transform.forward * 0.4f * Time.deltaTime;
                yield return null;
            }
        }
    }
}
