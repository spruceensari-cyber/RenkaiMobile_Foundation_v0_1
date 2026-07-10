using UnityEngine;

namespace RenkaiMobile.Weapons
{
    [CreateAssetMenu(menuName = "Renkai Mobile/Weapons/Melee Profile", fileName = "MeleeProfile_")]
    public sealed class MobileMeleeProfile : ScriptableObject
    {
        public string weaponId = "hikari_blade";
        public string displayName = "Hikari Blade";
        public float damage = 55f;
        public float heavyDamage = 85f;
        public float range = 2.4f;
        public float radius = 0.65f;
        public float attackCooldown = 0.55f;
        public float heavyAttackCooldown = 1.1f;
        public float movementMultiplier = 1.08f;
    }
}
