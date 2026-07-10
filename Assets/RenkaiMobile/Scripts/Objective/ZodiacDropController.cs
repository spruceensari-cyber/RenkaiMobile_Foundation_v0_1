using UnityEngine;
using RenkaiMobile.Combat;

namespace RenkaiMobile.Objective
{
    [RequireComponent(typeof(ZodiacCarrier), typeof(MobileHealth))]
    public sealed class ZodiacDropController : MonoBehaviour
    {
        [SerializeField] private ZodiacWorldItem worldItemPrefab;
        [SerializeField] private float groundProbeDistance = 5f;
        [SerializeField] private LayerMask groundMask = ~0;

        private ZodiacCarrier carrier;
        private MobileHealth health;

        private void Awake()
        {
            carrier = GetComponent<ZodiacCarrier>();
            health = GetComponent<MobileHealth>();
        }

        private void OnEnable()
        {
            if (health != null) health.Died += OnDied;
        }

        private void OnDisable()
        {
            if (health != null) health.Died -= OnDied;
        }

        private void OnDied(MobileDamageInfo damage)
        {
            if (carrier == null || !carrier.HasZodiac || worldItemPrefab == null) return;
            Vector3 position = SafeDropPosition(transform.position);
            Instantiate(worldItemPrefab, position, Quaternion.identity);
            carrier.RemoveZodiac();
        }

        private Vector3 SafeDropPosition(Vector3 origin)
        {
            Vector3 probe = origin + Vector3.up * 0.5f;
            if (Physics.Raycast(probe, Vector3.down, out RaycastHit hit, groundProbeDistance, groundMask, QueryTriggerInteraction.Ignore))
                return hit.point + Vector3.up * 0.2f;
            return origin + Vector3.up * 0.2f;
        }
    }
}
