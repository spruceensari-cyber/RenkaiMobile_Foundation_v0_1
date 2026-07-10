using UnityEngine;
using RenkaiMobile.Core;
using RenkaiMobile.Combat;

namespace RenkaiMobile.Bots
{
    [RequireComponent(typeof(MobileTeamMember), typeof(MobileHealth))]
    public sealed class TacticalBotBrain : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2.1f;
        [SerializeField] private float preferredRange = 15f;
        [SerializeField] private float fireRange = 30f;
        [SerializeField] private float fireInterval = 0.45f;
        [SerializeField] private float damage = 18f;
        [SerializeField] private float aimTurnSpeed = 7f;
        [SerializeField] private LayerMask sightMask = ~0;

        private MobileTeamMember team;
        private MobileHealth health;
        private Transform target;
        private float nextTargetSearch;
        private float nextFire;
        private float strafeSign = 1f;

        private void Awake()
        {
            team = GetComponent<MobileTeamMember>();
            health = GetComponent<MobileHealth>();
            strafeSign = Random.value > 0.5f ? 1f : -1f;
        }

        private void Update()
        {
            if (health == null || !health.IsAlive) return;

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
            if (move.sqrMagnitude > 0.001f)
                transform.position += move.normalized * moveSpeed * Time.deltaTime;

            if (distance <= fireRange && Time.time >= nextFire && HasLineOfSight(target))
            {
                nextFire = Time.time + fireInterval + Random.Range(-0.08f, 0.12f);
                MobileHealth enemyHealth = target.GetComponent<MobileHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.ApplyDamage(new MobileDamageInfo(
                        damage,
                        target.position + Vector3.up * 1.2f,
                        dir,
                        gameObject,
                        false));
                }
            }
        }

        private bool HasLineOfSight(Transform candidate)
        {
            Vector3 origin = transform.position + Vector3.up * 1.45f;
            Vector3 aim = candidate.position + Vector3.up * 1.25f;
            Vector3 ray = aim - origin;
            if (!Physics.Raycast(origin, ray.normalized, out RaycastHit hit, ray.magnitude + 0.5f, sightMask, QueryTriggerInteraction.Ignore))
                return true;
            return hit.transform == candidate || hit.transform.IsChildOf(candidate);
        }

        private Transform FindNearestEnemy()
        {
            MobileTeamMember[] all = FindObjectsByType<MobileTeamMember>(FindObjectsSortMode.None);
            Transform best = null;
            float bestSqr = float.MaxValue;

            foreach (MobileTeamMember member in all)
            {
                if (member == null || member == team || member.Team == MobileTeamId.None || member.Team == team.Team) continue;
                MobileHealth candidateHealth = member.GetComponent<MobileHealth>();
                if (candidateHealth == null || !candidateHealth.IsAlive) continue;

                float sqr = (member.transform.position - transform.position).sqrMagnitude;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    best = member.transform;
                }
            }
            return best;
        }
    }
}
