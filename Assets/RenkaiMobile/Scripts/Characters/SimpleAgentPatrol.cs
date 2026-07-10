using UnityEngine;
using Renkai.Combat;

namespace RenkaiMobile.Characters
{
    [RequireComponent(typeof(Health))]
    public sealed class SimpleAgentPatrol : MonoBehaviour
    {
        [SerializeField] private Vector3 localPointA = new Vector3(-2f, 0f, 0f);
        [SerializeField] private Vector3 localPointB = new Vector3(2f, 0f, 0f);
        [SerializeField] private float speed = 1.35f;
        [SerializeField] private float turnSpeed = 7f;

        private Vector3 worldA;
        private Vector3 worldB;
        private Vector3 target;
        private Health health;

        private void Awake()
        {
            health = GetComponent<Health>();
            Vector3 origin = transform.position;
            worldA = origin + localPointA;
            worldB = origin + localPointB;
            target = worldB;
        }

        private void Update()
        {
            if (health != null && !health.IsAlive) return;

            Vector3 toTarget = target - transform.position;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude < 0.16f)
            {
                target = Vector3.Distance(target, worldA) < 0.1f ? worldB : worldA;
                return;
            }

            Vector3 dir = toTarget.normalized;
            transform.position += dir * speed * Time.deltaTime;
            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion desired = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, desired, turnSpeed * Time.deltaTime);
            }
        }
    }
}
