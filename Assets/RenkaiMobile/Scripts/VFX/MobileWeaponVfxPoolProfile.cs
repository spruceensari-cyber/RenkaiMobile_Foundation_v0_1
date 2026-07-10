using UnityEngine;

namespace RenkaiMobile.VFX
{
    [CreateAssetMenu(menuName = "Renkai Mobile/VFX/Weapon VFX Pool Profile", fileName = "WeaponVfxPoolProfile_")]
    public sealed class MobileWeaponVfxPoolProfile : ScriptableObject
    {
        public GameObject muzzlePrefab;
        public GameObject tracerPrefab;
        public GameObject impactPrefab;
        public GameObject headshotPrefab;
        public int muzzlePrewarm = 8;
        public int tracerPrewarm = 12;
        public int impactPrewarm = 16;
        public int headshotPrewarm = 6;
    }
}
