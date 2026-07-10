using UnityEngine;

namespace RenkaiMobile.Loadout
{
    [CreateAssetMenu(menuName = "Renkai Mobile/Loadout/Profile", fileName = "Loadout_")]
    public sealed class MobileLoadoutProfile : ScriptableObject
    {
        public string loadoutId = "default";
        public string primaryWeaponId = "kitsune_ar";
        public string secondaryWeaponId = "zero_pulse";
        public string meleeWeaponId = "hikari_blade";
        public string primarySkinId = "kitsune_origin";
        public string secondarySkinId = "zero_origin";
        public string meleeSkinId = "hikari_origin";
        public string weaponCharmId = "none";
        public string reticleStyleId = "renkai_ring";
    }
}
