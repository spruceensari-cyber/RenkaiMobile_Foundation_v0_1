using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using RenkaiMobile.Combat;
using RenkaiMobile.VFX;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiMobileCombatRulesAndVfxWizard
    {
        private const string ProfilePath = "Assets/RenkaiMobile/Data/VFX/WeaponVfxPoolProfile_Default.asset";
        private const string PrefabFolder = "Assets/RenkaiMobile/Generated/VFX";

        [MenuItem("Renkai Mobile/Setup Team-Safe Combat & Pooled VFX")]
        public static void Build()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Play Mode'u kapat ve tekrar çalıştır.", "OK");
                return;
            }

            GameObject services = GameObject.Find("RenkaiMobile_Services") ?? new GameObject("RenkaiMobile_Services");
            if (services.GetComponent<MobileDamagePolicy>() == null) services.AddComponent<MobileDamagePolicy>();
            if (services.GetComponent<MobileVfxPool>() == null) services.AddComponent<MobileVfxPool>();

            MobileWeaponVfxPoolProfile profile = EnsureProfile();
            MobileWeaponVfxController[] controllers = Object.FindObjectsByType<MobileWeaponVfxController>(FindObjectsSortMode.None);
            foreach (MobileWeaponVfxController controller in controllers)
            {
                SerializedObject so = new SerializedObject(controller);
                so.FindProperty("pool").objectReferenceValue = services.GetComponent<MobileVfxPool>();
                so.FindProperty("poolProfile").objectReferenceValue = profile;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Renkai Mobile", "Team-safe damage policy ve pooled combat VFX servisi kuruldu.", "OK");
        }

        private static MobileWeaponVfxPoolProfile EnsureProfile()
        {
            EnsureFolder("Assets/RenkaiMobile/Data");
            EnsureFolder("Assets/RenkaiMobile/Data/VFX");
            EnsureFolder("Assets/RenkaiMobile/Generated");
            EnsureFolder(PrefabFolder);

            MobileWeaponVfxPoolProfile profile = AssetDatabase.LoadAssetAtPath<MobileWeaponVfxPoolProfile>(ProfilePath);
            if (profile == null)
            {
                profile = ScriptableObject.CreateInstance<MobileWeaponVfxPoolProfile>();
                AssetDatabase.CreateAsset(profile, ProfilePath);
            }

            profile.muzzlePrefab = EnsureSpherePrefab("MuzzleFlash", 0.12f);
            profile.impactPrefab = EnsureSpherePrefab("ImpactFracture", 0.1f);
            profile.headshotPrefab = EnsureSpherePrefab("HeadshotFracture", 0.18f);
            profile.tracerPrefab = EnsureTracerPrefab();
            EditorUtility.SetDirty(profile);
            return profile;
        }

        private static GameObject EnsureSpherePrefab(string name, float scale)
        {
            string path = PrefabFolder + "/" + name + ".prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null) return prefab;

            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            temp.name = name;
            temp.transform.localScale = Vector3.one * scale;
            Object.DestroyImmediate(temp.GetComponent<Collider>());
            prefab = PrefabUtility.SaveAsPrefabAsset(temp, path);
            Object.DestroyImmediate(temp);
            return prefab;
        }

        private static GameObject EnsureTracerPrefab()
        {
            string path = PrefabFolder + "/Tracer.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null) return prefab;

            GameObject temp = new GameObject("Tracer");
            LineRenderer line = temp.AddComponent<LineRenderer>();
            line.positionCount = 2;
            line.startWidth = 0.025f;
            line.endWidth = 0.008f;
            line.shadowCastingMode = ShadowCastingMode.Off;
            line.receiveShadows = false;
            prefab = PrefabUtility.SaveAsPrefabAsset(temp, path);
            Object.DestroyImmediate(temp);
            return prefab;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            int slash = path.LastIndexOf('/');
            string parent = path.Substring(0, slash);
            string name = path.Substring(slash + 1);
            if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
