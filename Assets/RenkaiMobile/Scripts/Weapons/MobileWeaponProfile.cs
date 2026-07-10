using UnityEngine;

namespace RenkaiMobile.Weapons
{
    [CreateAssetMenu(menuName = "Renkai Mobile/Weapons/Weapon Profile", fileName = "WeaponProfile_")]
    public sealed class MobileWeaponProfile : ScriptableObject
    {
        public string weaponId = "kitsune_ar";
        public string displayName = "Kitsune AR";
        public float bodyDamage = 34f;
        public float headDamage = 102f;
        public float fireInterval = 0.1f;
        public float range = 85f;
        public int magazineSize = 25;
        public float reloadSeconds = 2.2f;
        public float hipSpreadDegrees = 0.7f;
        public float adsSpreadMultiplier = 0.45f;
        public bool automatic = true;
    }
}
