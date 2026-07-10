using UnityEngine;
using RenkaiMobile.Combat;

namespace RenkaiMobile.Weapons
{
    public sealed class MobileMeleeController : MonoBehaviour
    {
        [SerializeField] private MobileMeleeProfile profile;
        [SerializeField] private Transform attackOrigin;
        [SerializeField] private MobileDamagePolicy damagePolicy;
        [SerializeField] private LayerMask hitMask = ~0;

        public string DisplayName => profile != null ? profile.displayName : "Hikari Blade";
        public float MovementMultiplier => profile != null ? profile.movementMultiplier : 1f;

        private float nextAttackTime;

        private void Awake()
        {
            if (damagePolicy == null) damagePolicy = FindFirstObjectByType<MobileDamagePolicy>();
        }

        public bool TrySlash() => TryAttack(false);
        public bool TryHeavySlash() => TryAttack(true);

        private bool TryAttack(bool heavy)
        {
            if (profile == null || Time.time < nextAttackTime) return false;
            nextAttackTime = Time.time + (heavy ? profile.heavyAttackCooldown : profile.attackCooldown);

            Transform originTransform = attackOrigin != null ? attackOrigin : transform;
            Vector3 origin = originTransform.position;
            Vector3 center = origin + originTransform.forward * (profile.range * 0.5f);
            Vector3 halfExtents = new Vector3(profile.radius, profile.radius, profile.range * 0.5f);
            Collider[] hits = Physics.OverlapBox(center, halfExtents, originTransform.rotation, hitMask, QueryTriggerInteraction.Ignore);

            float damage = heavy ? profile.heavyDamage : profile.damage;
            foreach (Collider hit in hits)
            {
                MobileHealth health = hit.GetComponentInParent<MobileHealth>();
                if (health == null || health.gameObject == gameObject) continue;
                if (damagePolicy != null && !damagePolicy.CanDamage(gameObject, health.gameObject)) continue;
                Vector3 direction = (health.transform.position - origin).normalized;
                health.ApplyDamage(new MobileDamageInfo(damage, hit.ClosestPoint(origin), direction, gameObject, false));
                break;
            }

            return true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (profile == null) return;
            Transform originTransform = attackOrigin != null ? attackOrigin : transform;
            Matrix4x4 old = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(originTransform.position, originTransform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.forward * (profile.range * 0.5f), new Vector3(profile.radius * 2f, profile.radius * 2f, profile.range));
            Gizmos.matrix = old;
        }
#endif
    }
}
