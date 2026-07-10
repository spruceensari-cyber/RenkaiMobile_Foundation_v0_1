using UnityEngine;

namespace RenkaiMobile.Combat
{
    public sealed class MobileHitFeedbackRelay : MonoBehaviour
    {
        [SerializeField] private MobileHitFeedbackBus bus;

        private MobileHealth[] tracked;

        private void Awake()
        {
            if (bus == null) bus = FindFirstObjectByType<MobileHitFeedbackBus>();
        }

        private void Start()
        {
            tracked = FindObjectsByType<MobileHealth>(FindObjectsSortMode.None);
            foreach (MobileHealth health in tracked)
            {
                if (health == null) continue;
                MobileHealth captured = health;
                health.Damaged += damage => OnDamaged(captured, damage);
            }
        }

        private void OnDamaged(MobileHealth victim, MobileDamageInfo damage)
        {
            if (bus == null || damage.Instigator == null) return;
            bus.Publish(new MobileHitFeedbackEvent(
                damage.Instigator,
                victim.gameObject,
                damage.Point,
                damage.Headshot,
                !victim.IsAlive));
        }
    }
}
