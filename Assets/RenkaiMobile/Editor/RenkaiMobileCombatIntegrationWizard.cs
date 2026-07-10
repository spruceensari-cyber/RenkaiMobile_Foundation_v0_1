using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using RenkaiMobile.Abilities;
using RenkaiMobile.Combat;
using RenkaiMobile.Objective;
using RenkaiMobile.UI;
using RenkaiMobile.VFX;
using RenkaiMobile.Weapons;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiMobileCombatIntegrationWizard
    {
        private const string WeaponProfilePath = "Assets/RenkaiMobile/Data/Weapons/WeaponProfile_Kitsune.asset";

        [MenuItem("Renkai Mobile/Integrate Mobile Combat Stack")]
        public static void Build()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Play Mode'u kapat ve tekrar çalıştır.", "OK");
                return;
            }

            GameObject player = GameObject.Find("Player");
            if (player == null)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Player bulunamadı.", "OK");
                return;
            }

            MobileWeaponProfile profile = EnsureWeaponProfile();
            Transform weaponRoot = FindWeaponRoot(player.transform);
            if (weaponRoot == null)
            {
                GameObject root = new GameObject("MobileWeaponRoot");
                Camera cam = player.GetComponentInChildren<Camera>(true);
                root.transform.SetParent(cam != null ? cam.transform : player.transform, false);
                root.transform.localPosition = new Vector3(0.18f, -0.16f, 0.42f);
                weaponRoot = root.transform;
            }

            MobileRifleController rifle = weaponRoot.GetComponent<MobileRifleController>();
            if (rifle == null) rifle = weaponRoot.gameObject.AddComponent<MobileRifleController>();
            SerializedObject rifleSo = new SerializedObject(rifle);
            rifleSo.FindProperty("profile").objectReferenceValue = profile;
            Camera aimCamera = player.GetComponentInChildren<Camera>(true);
            rifleSo.FindProperty("aimCamera").objectReferenceValue = aimCamera;
            rifleSo.ApplyModifiedPropertiesWithoutUndo();

            if (player.GetComponent<MobileWeaponShotSignal>() == null) player.AddComponent<MobileWeaponShotSignal>();
            if (weaponRoot.GetComponent<MobileWeaponVfxController>() == null) weaponRoot.gameObject.AddComponent<MobileWeaponVfxController>();
            if (player.GetComponent<WeaponInventoryController>() == null) player.AddComponent<WeaponInventoryController>();
            if (player.GetComponent<AbilityRuntimeController>() == null) player.AddComponent<AbilityRuntimeController>();
            if (player.GetComponent<RaikaAbilityKit>() == null) player.AddComponent<RaikaAbilityKit>();
            if (player.GetComponent<AgentAbilityDispatcher>() == null) player.AddComponent<AgentAbilityDispatcher>();
            if (player.GetComponent<ZodiacCarrier>() == null) player.AddComponent<ZodiacCarrier>();

            AttachMobileButtons(rifle, player.GetComponent<AgentAbilityDispatcher>(), player.GetComponent<ZodiacCarrier>());

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Renkai Mobile", "Mobile combat stack bağlandı: Kitsune rifle, shot signal, weapon VFX, inventory, Raika ability dispatcher, Zodiac carrier ve touch buttons bağlandı.", "OK");
        }

        private static MobileWeaponProfile EnsureWeaponProfile()
        {
            MobileWeaponProfile profile = AssetDatabase.LoadAssetAtPath<MobileWeaponProfile>(WeaponProfilePath);
            if (profile != null) return profile;

            if (!AssetDatabase.IsValidFolder("Assets/RenkaiMobile/Data"))
                AssetDatabase.CreateFolder("Assets/RenkaiMobile", "Data");
            if (!AssetDatabase.IsValidFolder("Assets/RenkaiMobile/Data/Weapons"))
                AssetDatabase.CreateFolder("Assets/RenkaiMobile/Data", "Weapons");

            profile = ScriptableObject.CreateInstance<MobileWeaponProfile>();
            AssetDatabase.CreateAsset(profile, WeaponProfilePath);
            return profile;
        }

        private static void AttachMobileButtons(MobileRifleController rifle, AgentAbilityDispatcher dispatcher, ZodiacCarrier carrier)
        {
            GameObject[] all = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (GameObject go in all)
            {
                if (go == null) continue;
                string n = go.name.ToUpperInvariant();

                if (n.Contains("FIRE"))
                {
                    MobileRifleFireButton button = go.GetComponent<MobileRifleFireButton>();
                    if (button == null) button = go.AddComponent<MobileRifleFireButton>();
                    SerializedObject so = new SerializedObject(button);
                    so.FindProperty("rifle").objectReferenceValue = rifle;
                    so.ApplyModifiedPropertiesWithoutUndo();
                }
                else if (n.Contains("ABILITY_Q")) BindAbilityButton(go, dispatcher, 0);
                else if (n.Contains("ABILITY_E")) BindAbilityButton(go, dispatcher, 1);
                else if (n.Contains("ABILITY_X")) BindAbilityButton(go, dispatcher, 2);
                else if (n.Contains("INTERACT"))
                {
                    MobileZodiacInteractButton button = go.GetComponent<MobileZodiacInteractButton>();
                    if (button == null) button = go.AddComponent<MobileZodiacInteractButton>();
                    SerializedObject so = new SerializedObject(button);
                    so.FindProperty("carrier").objectReferenceValue = carrier;
                    so.ApplyModifiedPropertiesWithoutUndo();
                }
            }
        }

        private static void BindAbilityButton(GameObject go, AgentAbilityDispatcher dispatcher, int slot)
        {
            MobileAgentAbilityButton button = go.GetComponent<MobileAgentAbilityButton>();
            if (button == null) button = go.AddComponent<MobileAgentAbilityButton>();
            button.Configure(dispatcher, slot);
        }

        private static Transform FindWeaponRoot(Transform root)
        {
            Transform[] all = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in all)
            {
                string n = t.name.ToLowerInvariant();
                if (n.Contains("weaponroot") || n.Contains("viewmodel") || n.Contains("mobileweaponroot"))
                    return t;
            }
            return null;
        }
    }
}
