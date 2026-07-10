using UnityEngine;
using RenkaiMobile.Combat;

namespace RenkaiMobile.Bots
{
    [RequireComponent(typeof(MobileHealth))]
    public sealed class CoverSeekingBot : MonoBehaviour
    {
        [SerializeField] private CoverRegistry registry;
        [SerializeField] private float seekHealthFraction = 0.55f;
        [SerializeField] private float moveSpeed = 2.8f;
        [SerializeField] private float stopDistance = 1.25f;
        [SerializeField] private float reassessInterval = 1.2f;
        [SerializeField] private float searchRadius = 28f;

        private MobileHealth health;
        private CoverPoint coverTarget;
        private float nextReassess;

        private void Awake()
        {
            health = GetComponent<MobileHealth>();
            if (registry == null) registry = FindFirstObjectByType<CoverRegistry>();
        }

        private void OnDisable()
        {
            coverTarget?.Release(gameObject);
        }

        private void Update()
        {
            if (health == null || !health.IsAlive) return;
            if (health.Current / Mathf.Max(1f, health.Max) > seekHealthFraction) return;

            if (coverTarget == null || Time.time >= nextReassess)
            {
                nextReassess = Time.time + reassessInterval;
                SelectCover();
            }

            if (coverTarget == null) return;
            Vector3 delta = coverTarget.transform.position - transform.position;
            delta.y = 0f;
            if (delta.magnitude <= stopDistance) return;

            Vector3 dir = delta.normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion desired = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, desired, 8f * Time.deltaTime);
            }
        }

        private void SelectCover()
        {
            if (registry == null) return;
            MobileHealth[] all = FindObjectsByType<MobileHealth>(FindObjectsSortMode.None);
            Vector3 threat = transform.position + transform.forward * 10f;
            float bestEnemySqr = float.MaxValue;
            foreach (MobileHealth candidate in all)
            {
                if (candidate == null || candidate == health || !candidate.IsAlive) continue;
                float sqr = (candidate.transform.position - transform.position).sqrMagnitude;
                if (sqr < bestEnemySqr)
                {
                    bestEnemySqr = sqr;
                    threat = candidate.transform.position;
                }
            }

            coverTarget?.Release(gameObject);
            CoverPoint next = registry.FindBest(transform.position, threat, searchRadius);
            if (next != null && next.TryOccupy(gameObject)) coverTarget = next;
            else coverTarget = null;
        }
    }
}
