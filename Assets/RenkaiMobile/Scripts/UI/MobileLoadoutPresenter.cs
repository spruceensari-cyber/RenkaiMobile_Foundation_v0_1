using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.Loadout;

namespace RenkaiMobile.UI
{
    public sealed class MobileLoadoutPresenter : MonoBehaviour
    {
        [SerializeField] private MobileLoadoutProfile profile;
        [SerializeField] private Text primaryText;
        [SerializeField] private Text secondaryText;
        [SerializeField] private Text meleeText;
        [SerializeField] private Text reticleText;
        [SerializeField] private Text charmText;
        [SerializeField] private CanvasGroup panelGroup;

        public void Refresh()
        {
            if (profile == null) return;
            if (primaryText != null) primaryText.text = "PRIMARY // " + profile.primaryWeaponId.ToUpperInvariant();
            if (secondaryText != null) secondaryText.text = "SECONDARY // " + profile.secondaryWeaponId.ToUpperInvariant();
            if (meleeText != null) meleeText.text = "MELEE // " + profile.meleeWeaponId.ToUpperInvariant();
            if (reticleText != null) reticleText.text = "RETICLE // " + profile.reticleStyleId.ToUpperInvariant();
            if (charmText != null) charmText.text = "CHARM // " + profile.weaponCharmId.ToUpperInvariant();
        }

        public void Show()
        {
            Refresh();
            if (panelGroup != null)
            {
                panelGroup.alpha = 1f;
                panelGroup.interactable = true;
                panelGroup.blocksRaycasts = true;
            }
        }

        public void Hide()
        {
            if (panelGroup == null) return;
            panelGroup.alpha = 0f;
            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;
        }
    }
}
