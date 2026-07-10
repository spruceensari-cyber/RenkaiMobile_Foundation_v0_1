using UnityEngine;

namespace RenkaiMobile.Weapons
{
    [CreateAssetMenu(menuName = "Renkai Mobile/Weapons/Pistol Profile", fileName = "PistolProfile_")]
    public sealed class MobilePistolProfile : ScriptableObject
    {
        public string weaponId = "zero_pulse";
        public string displayName = "Zero Pulse";
        public float bodyDamage = 30f;
        public float headDamage = 90f;
        public float fireInterval = 0.18f;
        public float range = 55f;
        public int magazineSize = 12;
        public float reloadSeconds = 1.45f;
        public float hipSpreadDegrees = 0.5f;
        public float adsSpreadMultiplier = 0.35f;
    }
}
