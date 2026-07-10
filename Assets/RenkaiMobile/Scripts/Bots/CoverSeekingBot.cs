using UnityEngine;
using RenkaiMobile.Combat;

namespace RenkaiMobile.Bots
{
    [RequireComponent(typeof(MobileHealth))]
    public sealed class CoverSeekingBot : MonoBehaviour
    {
        [SerializeField] private string coverNamePrefix = "Cover_";
        [SerializeField] private float seekHealthFraction = 0.55f;
        [SerializeField] private float moveSpeed = 2.8f;
        [SerializeField] private float stopDistance = 1.25f;
        [SerializeField] private float reassessInterval = 1.2f;

        private MobileHealth health;
        private Transform coverTarget;
        private float nextReassess;

        private void Awake() => health = GetComponent<MobileHealth>();

        private void Update()
        {
            if (health == null || !health.IsAlive) return;
            if (health.Current / Mathf.Max(1f, health.Max) > seekHealthFraction) return;

            if (coverTarget == null || Time.time >= nextReassess)
            {
                nextReassess = Time.time + reassessInterval;
                coverTarget = FindNearestCover();
            }

            if (coverTarget == null) return;
            Vector3 delta = coverTarget.position - transform.position;
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

        private Transform FindNearestCover()
        {
            Transform best = null;
            float bestSqr = float.MaxValue;
            GameObject[] all = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (GameObject candidate in all)
            {
                if (candidate == null || !candidate.name.StartsWith(coverNamePrefix)) continue;
                float sqr = (candidate.transform.position - transform.position).sqrMagnitude;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    best = candidate.transform;
                }
            }
            return best;
        }
    }
}
