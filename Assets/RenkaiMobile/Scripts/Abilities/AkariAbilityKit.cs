using System.Collections;
using UnityEngine;
using RenkaiMobile.Combat;

namespace RenkaiMobile.Abilities
{
    public sealed class AkariAbilityKit : MonoBehaviour
    {
        [SerializeField] private AbilityRuntimeController runtime;
        [SerializeField] private MobileDamagePolicy damagePolicy;
        [SerializeField] private float plasmaRadius = 4.5f;
        [SerializeField] private float plasmaDamage = 36f;
        [SerializeField] private float ignitionDuration = 4f;

        private void Awake()
        {
            if (runtime == null) runtime = GetComponent<AbilityRuntimeController>();
            if (damagePolicy == null) damagePolicy = FindFirstObjectByType<MobileDamagePolicy>();
        }

        public void UsePlasmaBurst()
        {
            if (runtime == null || !runtime.TryUseAbility(0)) return;
            DamageNearby(plasmaRadius, plasmaDamage);
        }

        public void UseIgnitionField()
        {
            if (runtime == null || !runtime.TryUseAbility(1)) return;
            StartCoroutine(IgnitionFieldRoutine());
        }

        public void UseSolarBreak()
        {
            if (runtime == null || !runtime.TryUseAbility(2)) return;
            DamageNearby(plasmaRadius * 1.8f, plasmaDamage * 1.6f);
        }

        private IEnumerator IgnitionFieldRoutine()
        {
            float elapsed = 0f;
            while (elapsed < ignitionDuration)
            {
                elapsed += 0.5f;
                DamageNearby(plasmaRadius * 0.75f, plasmaDamage * 0.18f);
                yield return new WaitForSeconds(0.5f);
            }
        }

        private void DamageNearby(float radius, float damage)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, radius, ~0, QueryTriggerInteraction.Ignore);
            foreach (Collider hit in hits)
            {
                MobileHealth health = hit.GetComponentInParent<MobileHealth>();
                if (health == null || health.gameObject == gameObject) continue;
                if (damagePolicy != null && !damagePolicy.CanDamage(gameObject, health.gameObject)) continue;
                Vector3 direction = (health.transform.position - transform.position).normalized;
                health.ApplyDamage(new MobileDamageInfo(damage, health.transform.position, direction, gameObject, false));
            }
        }
    }
}
