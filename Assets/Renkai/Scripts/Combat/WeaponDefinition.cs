using UnityEngine;

namespace Renkai.Combat
{
    [CreateAssetMenu(menuName = "Renkai/Combat/Weapon Definition", fileName = "Weapon_")]
    public sealed class WeaponDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string weaponId = "renkai_rifle";
        public string displayName = "Renkai Rifle";

        [Header("Damage")]
        [Min(1f)] public float bodyDamage = 34f;
        [Min(1f)] public float headDamage = 102f;
        [Min(1f)] public float range = 120f;

        [Header("Timing")]
        [Min(0.02f)] public float fireInterval = 0.1f;
        [Min(0.1f)] public float reloadSeconds = 2.2f;

        [Header("Magazine")]
        [Min(1)] public int magazineSize = 25;

        [Header("Accuracy")]
        [Min(0f)] public float hipSpreadDegrees = 1.1f;
        [Min(0f)] public float movingSpreadBonus = 2.2f;
        [Min(0f)] public float crouchSpreadMultiplier = 0.75f;
    }
}
