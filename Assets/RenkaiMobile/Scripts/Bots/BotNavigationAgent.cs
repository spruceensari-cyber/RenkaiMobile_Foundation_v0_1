using UnityEngine;

namespace RenkaiMobile.Bots
{
    public sealed class BotNavigationAgent : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2.8f;
        [SerializeField] private float rotationSpeed = 8f;
        [SerializeField] private float stoppingDistance = 0.8f;
        [SerializeField] private float stuckCheckInterval = 1f;
        [SerializeField] private float stuckDistanceThreshold = 0.15f;

        public Vector3 Destination { get; private set; }
        public bool HasDestination { get; private set; }
        public bool ReachedDestination => HasDestination && Vector3.Distance(transform.position, Destination) <= stoppingDistance;

        private Vector3 lastStuckCheckPosition;
        private float nextStuckCheck;

        private void Awake()
        {
            lastStuckCheckPosition = transform.position;
        }

        private void Update()
        {
            if (!HasDestination) return;
            Vector3 delta = Destination - transform.position;
            delta.y = 0f;
            if (delta.magnitude <= stoppingDistance)
            {
                HasDestination = false;
                return;
            }

            Vector3 dir = delta.normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion desired = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, desired, rotationSpeed * Time.deltaTime);
            }

            if (Time.time >= nextStuckCheck)
            {
                nextStuckCheck = Time.time + stuckCheckInterval;
                float moved = Vector3.Distance(transform.position, lastStuckCheckPosition);
                if (moved < stuckDistanceThreshold)
                    transform.position += transform.right * 0.35f;
                lastStuckCheckPosition = transform.position;
            }
        }

        public void SetDestination(Vector3 destination)
        {
            Destination = destination;
            HasDestination = true;
        }

        public void Stop()
        {
            HasDestination = false;
        }
    }
}
