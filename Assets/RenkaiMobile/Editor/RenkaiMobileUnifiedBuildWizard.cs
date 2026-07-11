using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using RenkaiMobile.Core;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiMobileUnifiedBuildWizard
    {
        private const string MenuPath = "Renkai Mobile/00 BUILD RENKAI MOBILE - UNIFIED ONE CLICK";
        private const string CatalogPath = "Assets/RenkaiMobile/Data/Core/RenkaiMobileIdentityCatalog.asset";

        private static readonly string[] GeneratedRootsToRebuild =
        {
            "Player",
            "Renkai_5v5_Match",
            "Zodiac_Objective",
            "Zodiac_Zone_A",
            "Zodiac_Zone_B",
            "Kagami_CompleteMap",
            "RenkaiMobile_Services",
            "RenkaiMobile_FullHUD",
            "Renkai_Presentation",
            "EventSystem"
        };

        private static readonly string[] LegacyPrototypeRoots =
        {
            "Kagami_District_FutureLayer",
            "Kagami_Playable_Layout",
            "GameplayLoopHUD",
            "MobileHUD",
            "ZodiacObjectiveHUD",
            "SpiritCore_Objective",
            "SpiritCoreSite_A",
            "SpiritCoreSite_B",
            "TacticalObjectiveRoot",
            "Renkai_TacticalObjective",
            "Renkai_VisualUpgrade",
            "Renkai_3DCharacters",
            "CompetitiveMatchLayer",
            "CombatPolish"
        };

        [MenuItem(MenuPath, false, 0)]
        public static void BuildUnifiedGame()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Play Mode'u kapat ve unified build komutunu tekrar çalıştır.", "OK");
                return;
            }

            var removed = new List<string>();

            try
            {
                EditorUtility.DisplayProgressBar("Renkai Mobile Unified Build", "Cleaning generated mobile prototypes", 0.08f);
                RemoveRoots(GeneratedRootsToRebuild, removed);
                RemoveRoots(LegacyPrototypeRoots, removed);

                EditorUtility.DisplayProgressBar("Renkai Mobile Unified Build", "Building complete mobile adaptation", 0.22f);
                RenkaiOneClickFullBuildWizard.BuildCompleteGame();

                EditorUtility.DisplayProgressBar("Renkai Mobile Unified Build", "Applying unified Renkai identity", 0.86f);
                RenkaiMobileIdentityCatalog catalog = EnsureIdentityCatalog();
                AttachManifest(catalog);
                NormalizeCanonicalHierarchy();

                Scene scene = SceneManager.GetActiveScene();
                EditorSceneManager.MarkSceneDirty(scene);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                if (!string.IsNullOrEmpty(scene.path))
                    EditorSceneManager.SaveScene(scene);

                string cleanup = removed.Count > 0
                    ? "\n\nRemoved old generated scene roots:\n- " + string.Join("\n- ", removed)
                    : "\n\nNo old generated prototype roots were present.";

                EditorUtility.DisplayDialog(
                    "RENKAI MOBILE UNIFIED BUILD",
                    "COMPLETE.\n\nThe scene now contains one canonical Renkai Mobile build:\n" +
                    "• shared Renkai agent identity\n" +
                    "• shared weapon identity\n" +
                    "• Kagami District mobile adaptation\n" +
                    "• Zodiac objective language\n" +
                    "• mobile-simplified controls and pacing\n" +
                    "• one 5v5 match stack\n" +
                    "• one HUD stack\n" +
                    "• one presentation stack\n\n" +
                    "Assets/Renkai was not modified by this builder." + cleanup,
                    "OK");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static void RemoveRoots(string[] names, List<string> removed)
        {
            foreach (string name in names)
            {
                GameObject go = GameObject.Find(name);
                if (go == null) continue;
                removed.Add(name);
                Object.DestroyImmediate(go);
            }
        }

        private static RenkaiMobileIdentityCatalog EnsureIdentityCatalog()
        {
            RenkaiMobileIdentityCatalog catalog = AssetDatabase.LoadAssetAtPath<RenkaiMobileIdentityCatalog>(CatalogPath);
            if (catalog != null) return catalog;

            EnsureFolder("Assets/RenkaiMobile/Data");
            EnsureFolder("Assets/RenkaiMobile/Data/Core");
            catalog = ScriptableObject.CreateInstance<RenkaiMobileIdentityCatalog>();
            AssetDatabase.CreateAsset(catalog, CatalogPath);
            EditorUtility.SetDirty(catalog);
            return catalog;
        }

        private static void AttachManifest(RenkaiMobileIdentityCatalog catalog)
        {
            GameObject root = GameObject.Find("RenkaiMobile_Game");
            if (root == null) root = new GameObject("RenkaiMobile_Game");

            RenkaiMobileBuildManifest manifest = root.GetComponent<RenkaiMobileBuildManifest>();
            if (manifest == null) manifest = root.AddComponent<RenkaiMobileBuildManifest>();
            manifest.Configure(catalog);

            Adopt(root.transform, "Renkai_5v5_Match");
            Adopt(root.transform, "Zodiac_Objective");
            Adopt(root.transform, "Kagami_CompleteMap");
            Adopt(root.transform, "RenkaiMobile_Services");
            Adopt(root.transform, "RenkaiMobile_FullHUD");
            Adopt(root.transform, "Renkai_Presentation");
        }

        private static void NormalizeCanonicalHierarchy()
        {
            GameObject root = GameObject.Find("RenkaiMobile_Game");
            if (root == null) return;

            Transform player = GameObject.Find("Player")?.transform;
            if (player != null) player.SetParent(root.transform, true);
        }

        private static void Adopt(Transform root, string objectName)
        {
            GameObject go = GameObject.Find(objectName);
            if (go != null && go.transform.parent != root)
                go.transform.SetParent(root, true);
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            int slash = path.LastIndexOf('/');
            if (slash <= 0) return;
            string parent = path.Substring(0, slash);
            string name = path.Substring(slash + 1);
            if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
