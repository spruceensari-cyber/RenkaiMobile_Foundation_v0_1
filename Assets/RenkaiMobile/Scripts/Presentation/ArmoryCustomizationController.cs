using System;
using UnityEngine;
using RenkaiMobile.Loadout;

namespace RenkaiMobile.Presentation
{
    public sealed class ArmoryCustomizationController : MonoBehaviour
    {
        [SerializeField] private MobileLoadoutProfile loadout;
        [SerializeField] private ArmorySkinBinding primaryBinding;
        [SerializeField] private ArmorySkinBinding secondaryBinding;
        [SerializeField] private ArmorySkinBinding meleeBinding;

        public event Action LoadoutChanged;

        public void ApplyCurrentLoadout()
        {
            if (loadout == null) return;
            primaryBinding?.Apply(loadout.primarySkinId);
            secondaryBinding?.Apply(loadout.secondarySkinId);
            meleeBinding?.Apply(loadout.meleeSkinId);
            LoadoutChanged?.Invoke();
        }

        public void SetPrimarySkin(string skinId)
        {
            if (loadout == null || string.IsNullOrWhiteSpace(skinId)) return;
            loadout.primarySkinId = skinId;
            primaryBinding?.Apply(skinId);
            LoadoutChanged?.Invoke();
        }

        public void SetSecondarySkin(string skinId)
        {
            if (loadout == null || string.IsNullOrWhiteSpace(skinId)) return;
            loadout.secondarySkinId = skinId;
            secondaryBinding?.Apply(skinId);
            LoadoutChanged?.Invoke();
        }

        public void SetMeleeSkin(string skinId)
        {
            if (loadout == null || string.IsNullOrWhiteSpace(skinId)) return;
            loadout.meleeSkinId = skinId;
            meleeBinding?.Apply(skinId);
            LoadoutChanged?.Invoke();
        }

        public void SetCharm(string charmId)
        {
            if (loadout == null || string.IsNullOrWhiteSpace(charmId)) return;
            loadout.weaponCharmId = charmId;
            LoadoutChanged?.Invoke();
        }

        public void SetReticleStyle(string reticleStyleId)
        {
            if (loadout == null || string.IsNullOrWhiteSpace(reticleStyleId)) return;
            loadout.reticleStyleId = reticleStyleId;
            LoadoutChanged?.Invoke();
        }
    }
}
