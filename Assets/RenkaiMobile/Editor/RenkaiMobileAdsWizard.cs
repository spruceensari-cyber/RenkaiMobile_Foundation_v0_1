using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using RenkaiMobile.Combat;
using RenkaiMobile.UI;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiMobileAdsWizard
    {
        private const string AimProfilePath = "Assets/RenkaiMobile/Data/Aim/AimProfile_Kitsune.asset";

        [MenuItem("Renkai Mobile/Add Futuristic ADS System")]
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

            Camera camera = player.GetComponentInChildren<Camera>(true);
            if (camera == null) camera = Camera.main;

            Transform viewmodel = FindViewmodel(camera != null ? camera.transform : player.transform);
            MobileAimProfile profile = EnsureAimProfile();

            MobileAdsController ads = player.GetComponent<MobileAdsController>();
            if (ads == null) ads = player.AddComponent<MobileAdsController>();

            SerializedObject adsSo = new SerializedObject(ads);
            adsSo.FindProperty("playerCamera").objectReferenceValue = camera;
            adsSo.FindProperty("weaponViewmodel").objectReferenceValue = viewmodel;
            adsSo.FindProperty("profile").objectReferenceValue = profile;
            adsSo.ApplyModifiedPropertiesWithoutUndo();

            CrosshairController crosshair = Object.FindFirstObjectByType<CrosshairController>();
            AdsCrosshairBridge bridge = player.GetComponent<AdsCrosshairBridge>();
            if (bridge == null) bridge = player.AddComponent<AdsCrosshairBridge>();
            SerializedObject bridgeSo = new SerializedObject(bridge);
            bridgeSo.FindProperty("adsController").objectReferenceValue = ads;
            bridgeSo.FindProperty("crosshair").objectReferenceValue = crosshair;
            bridgeSo.ApplyModifiedPropertiesWithoutUndo();

            AttachToExistingAdsButton(ads);

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Renkai Mobile", "Futuristic ADS sistemi bağlandı: FOV zoom, viewmodel transition, recoil/spread/sway multipliers ve crosshair contraction hazır.", "OK");
        }

        private static MobileAimProfile EnsureAimProfile()
        {
            MobileAimProfile profile = AssetDatabase.LoadAssetAtPath<MobileAimProfile>(AimProfilePath);
            if (profile != null) return profile;

            string folder = "Assets/RenkaiMobile/Data/Aim";
            if (!AssetDatabase.IsValidFolder("Assets/RenkaiMobile/Data"))
                AssetDatabase.CreateFolder("Assets/RenkaiMobile", "Data");
            if (!AssetDatabase.IsValidFolder(folder))
                AssetDatabase.CreateFolder("Assets/RenkaiMobile/Data", "Aim");

            profile = ScriptableObject.CreateInstance<MobileAimProfile>();
            AssetDatabase.CreateAsset(profile, AimProfilePath);
            return profile;
        }

        private static Transform FindViewmodel(Transform root)
        {
            if (root == null) return null;
            Transform[] all = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in all)
            {
                string n = t.name.ToLowerInvariant();
                if (n.Contains("viewmodel") || n.Contains("weaponroot") || n.Contains("weapon_view"))
                    return t;
            }
            return null;
        }

        private static void AttachToExistingAdsButton(MobileAdsController ads)
        {
            GameObject[] all = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (GameObject go in all)
            {
                if (go == null || !go.name.ToUpperInvariant().Contains("ADS")) continue;
                MobileAdsButton button = go.GetComponent<MobileAdsButton>();
                if (button == null) button = go.AddComponent<MobileAdsButton>();
                SerializedObject so = new SerializedObject(button);
                so.FindProperty("adsController").objectReferenceValue = ads;
                so.ApplyModifiedPropertiesWithoutUndo();
                break;
            }
        }
    }
}
