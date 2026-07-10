using UnityEngine;
using Renkai.Core;
using Renkai.Combat;

namespace RenkaiMobile.Bots
{
    [RequireComponent(typeof(TeamMember), typeof(Health))]
    public sealed class TacticalBotBrain : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2.1f;
        [SerializeField] private float preferredRange = 15f;
        [SerializeField] private float fireRange = 30f;
        [SerializeField] private float fireInterval = 0.45f;
        [SerializeField] private float damage = 18f;
        [SerializeField] private float aimTurnSpeed = 7f;
        [SerializeField] private LayerMask sightMask = ~0;

        private TeamMember team;
        private Health health;
        private Transform target;
        private float nextTargetSearch;
        private float nextFire;
        private float strafeSign = 1f;

        private void Awake()
        {
            team = GetComponent<TeamMember>();
            health = GetComponent<Health>();
            strafeSign = Random.value > 0.5f ? 1f : -1f;
        }

        private void Update()
        {
            if (!health.IsAlive) return;
            if (Time.time >= nextTargetSearch)
            {
                nextTargetSearch = Time.time + 0.5f;
                target = FindNearestEnemy();
            }
            if (target == null) return;

            Vector3 toTarget = target.position - transform.position;
            toTarget.y = 0f;
            float distance = toTarget.magnitude;
            Vector3 dir = distance > 0.01f ? toTarget / distance : transform.forward;

            Quaternion desired = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, desired, aimTurnSpeed * Time.deltaTime);

            Vector3 move = Vector3.zero;
            if (distance > preferredRange + 2f) move += dir;
            else if (distance < preferredRange - 3f) move -= dir;
            move += transform.right * strafeSign * 0.35f;
            transform.position += move.normalized * moveSpeed * Time.deltaTime;

            if (distance <= fireRange && Time.time >= nextFire && HasLineOfSight(target))
            {
                nextFire = Time.time + fireInterval + Random.Range(-0.08f, 0.12f);
                var enemyHealth = target.GetComponent<Health>();
                if (enemyHealth != null)
                    enemyHealth.ApplyDamage(new DamageInfo(damage, target.position + Vector3.up * 1.2f, dir, gameObject, false));
            }
        }

        private bool HasLineOfSight(Transform candidate)
        {
            Vector3 origin = transform.position + Vector3.up * 1.45f;
            Vector3 aim = candidate.position + Vector3.up * 1.25f;
            Vector3 ray = aim - origin;
            if (!Physics.Raycast(origin, ray.normalized, out RaycastHit hit, ray.magnitude + 0.5f, sightMask, QueryTriggerInteraction.Ignore)) return true;
            return hit.transform == candidate || hit.transform.IsChildOf(candidate);
        }

        private Transform FindNearestEnemy()
        {
            TeamMember[] all = FindObjectsByType<TeamMember>(FindObjectsSortMode.None);
            Transform best = null;
            float bestSqr = float.MaxValue;
            foreach (var member in all)
            {
                if (member == null || member == team || member.Team == TeamId.None || member.Team == team.Team) continue;
                Health candidateHealth = member.GetComponent<Health>();
                if (candidateHealth == null || !candidateHealth.IsAlive) continue;
                float sqr = (member.transform.position - transform.position).sqrMagnitude;
                if (sqr < bestSqr) { bestSqr = sqr; best = member.transform; }
            }
            return best;
        }
    }
}
