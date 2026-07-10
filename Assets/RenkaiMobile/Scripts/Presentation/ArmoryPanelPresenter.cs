using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.Loadout;

namespace RenkaiMobile.Presentation
{
    public sealed class ArmoryPanelPresenter : MonoBehaviour
    {
        [SerializeField] private MobileLoadoutProfile loadout;
        [SerializeField] private Text primaryText;
        [SerializeField] private Text secondaryText;
        [SerializeField] private Text meleeText;
        [SerializeField] private Text charmText;
        [SerializeField] private Text reticleText;
        [SerializeField] private CanvasGroup group;

        public void Show()
        {
            Refresh();
            if (group != null) group.alpha = 1f;
        }

        public void Hide()
        {
            if (group != null) group.alpha = 0f;
        }

        public void Refresh()
        {
            if (loadout == null) return;
            if (primaryText != null) primaryText.text = "PRIMARY // " + loadout.primaryWeaponId.ToUpperInvariant() + "\nSKIN // " + loadout.primarySkinId.ToUpperInvariant();
            if (secondaryText != null) secondaryText.text = "SECONDARY // " + loadout.secondaryWeaponId.ToUpperInvariant() + "\nSKIN // " + loadout.secondarySkinId.ToUpperInvariant();
            if (meleeText != null) meleeText.text = "MELEE // " + loadout.meleeWeaponId.ToUpperInvariant() + "\nSKIN // " + loadout.meleeSkinId.ToUpperInvariant();
            if (charmText != null) charmText.text = "CHARM // " + loadout.weaponCharmId.ToUpperInvariant();
            if (reticleText != null) reticleText.text = "RETICLE // " + loadout.reticleStyleId.ToUpperInvariant();
        }
    }
}
