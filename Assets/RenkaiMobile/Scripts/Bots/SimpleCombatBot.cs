using UnityEngine;

namespace RenkaiMobile.Bots
{
    public sealed class SimpleCombatBot : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float moveSpeed = 3.2f;
        [SerializeField] private float stopDistance = 12f;
        [SerializeField] private float turnSpeed = 8f;

        private void Update()
        {
            if (target == null) return;
            Vector3 flat = target.position - transform.position;
            flat.y = 0f;
            if (flat.sqrMagnitude > 0.01f)
            {
                Quaternion desired = Quaternion.LookRotation(flat.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, desired, turnSpeed * Time.deltaTime);
            }
            if (flat.magnitude > stopDistance)
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }

        public void SetTarget(Transform value) => target = value;
    }
}
