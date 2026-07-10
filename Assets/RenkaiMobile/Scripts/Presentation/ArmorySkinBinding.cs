using UnityEngine;

namespace RenkaiMobile.Presentation
{
    [System.Serializable]
    public sealed class WeaponSkinMaterialSet
    {
        public string skinId;
        public Material[] materials;
    }

    public sealed class ArmorySkinBinding : MonoBehaviour
    {
        [SerializeField] private Renderer[] targetRenderers;
        [SerializeField] private WeaponSkinMaterialSet[] skins;
        [SerializeField] private string defaultSkinId = "default";

        public string ActiveSkinId { get; private set; }

        private void Start()
        {
            Apply(defaultSkinId);
        }

        public bool Apply(string skinId)
        {
            if (skins == null || targetRenderers == null) return false;
            foreach (WeaponSkinMaterialSet skin in skins)
            {
                if (skin == null || skin.skinId != skinId || skin.materials == null) continue;
                int count = Mathf.Min(targetRenderers.Length, skin.materials.Length);
                for (int i = 0; i < count; i++)
                {
                    if (targetRenderers[i] != null && skin.materials[i] != null)
                        targetRenderers[i].sharedMaterial = skin.materials[i];
                }
                ActiveSkinId = skinId;
                return true;
            }
            return false;
        }
    }
}
