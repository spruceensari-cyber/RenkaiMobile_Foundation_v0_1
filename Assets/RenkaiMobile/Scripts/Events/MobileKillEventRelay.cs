using UnityEngine;
using RenkaiMobile.Combat;

namespace RenkaiMobile.Events
{
    public sealed class MobileKillEventRelay : MonoBehaviour
    {
        [SerializeField] private MobileKillEventBus bus;
        private MobileHealth[] tracked;

        private void Awake()
        {
            if (bus == null) bus = FindFirstObjectByType<MobileKillEventBus>();
        }

        private void Start()
        {
            tracked = FindObjectsByType<MobileHealth>(FindObjectsSortMode.None);
            foreach (MobileHealth health in tracked)
            {
                if (health == null) continue;
                MobileHealth captured = health;
                health.Died += damage => OnDied(captured, damage);
            }
        }

        private void OnDied(MobileHealth victimHealth, MobileDamageInfo damage)
        {
            if (bus == null || victimHealth == null) return;

            MobileCombatantIdentity victim = victimHealth.GetComponentInParent<MobileCombatantIdentity>();
            MobileCombatantIdentity killer = damage.Instigator != null
                ? damage.Instigator.GetComponentInParent<MobileCombatantIdentity>()
                : null;

            bool environmental = damage.Instigator == null;
            bus.Publish(new MobileKillEvent(
                killer,
                victim,
                weaponId: string.Empty,
                abilityId: string.Empty,
                headshot: damage.Headshot,
                environmental: environmental,
                timestamp: Time.time));
        }
    }
}
