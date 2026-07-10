using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using RenkaiMobile.Map;
using RenkaiMobile.Bots;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiKagamiDistrictWizard
    {
        [MenuItem("Renkai Mobile/Build Kagami District Future Layer")]
        public static void Build()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Play Mode'u kapat ve tekrar çalıştır.", "OK");
                return;
            }

            GameObject old = GameObject.Find("Kagami_District_FutureLayer");
            if (old != null) Object.DestroyImmediate(old);

            GameObject root = new GameObject("Kagami_District_FutureLayer");
            Transform architecture = NewChild(root.transform, "Architecture");
            Transform energy = NewChild(root.transform, "EnergyPaths");
            Transform holograms = NewChild(root.transform, "Holograms");
            Transform cover = NewChild(root.transform, "CoverPoints");

            BuildArchitecture(architecture);
            BuildEnergyPaths(energy);
            BuildHolograms(holograms);
            BuildCoverPoints(cover);
            EnsureCoverRegistry(root);

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            EditorUtility.DisplayDialog("Renkai Mobile", "Kagami District future layer kuruldu: cyber-shrine landmarks, energy paths, elevated structures, holograms ve structured cover points eklendi.", "OK");
        }

        private static void BuildArchitecture(Transform parent)
        {
            CreateBlock(parent, "Central_Shrine_Gate", new Vector3(0f, 2.2f, 8f), new Vector3(8f, 4.4f, 1f));
            CreateBlock(parent, "Left_Megastructure", new Vector3(-16f, 4f, 12f), new Vector3(5f, 8f, 14f));
            CreateBlock(parent, "Right_Megastructure", new Vector3(16f, 5f, 17f), new Vector3(5f, 10f, 12f));
            CreateBlock(parent, "Elevated_Transit", new Vector3(0f, 6f, 24f), new Vector3(28f, 1f, 2.2f));
            CreateBlock(parent, "ZoneA_Frame", new Vector3(-10f, 1.5f, 10f), new Vector3(7f, 3f, 0.7f));
            CreateBlock(parent, "ZoneB_Frame", new Vector3(10f, 1.5f, 18f), new Vector3(7f, 3f, 0.7f));

            for (int i = 0; i < 8; i++)
            {
                float side = i % 2 == 0 ? -1f : 1f;
                float z = -6f + i * 5f;
                CreateBlock(parent, "Skyline_Pillar_" + i, new Vector3(side * (22f + (i % 3) * 3f), 7f + i % 4, z), new Vector3(3f, 14f + i % 4 * 2f, 3f));
            }
        }

        private static void BuildEnergyPaths(Transform parent)
        {
            for (int i = 0; i < 6; i++)
            {
                GameObject strip = GameObject.CreatePrimitive(PrimitiveType.Cube);
                strip.name = "Energy_Path_" + i;
                strip.transform.SetParent(parent, false);
                strip.transform.position = new Vector3((i - 2.5f) * 4f, 0.06f, 4f + i * 4f);
                strip.transform.localScale = new Vector3(0.18f, 0.04f, 8f);
                Object.DestroyImmediate(strip.GetComponent<Collider>());
            }
        }

        private static void BuildHolograms(Transform parent)
        {
            Vector3[] positions =
            {
                new Vector3(-8f, 3.8f, 4f),
                new Vector3(8f, 4.5f, 12f),
                new Vector3(-14f, 5.5f, 22f),
                new Vector3(14f, 6f, 28f)
            };

            for (int i = 0; i < positions.Length; i++)
            {
                GameObject holo = GameObject.CreatePrimitive(i % 2 == 0 ? PrimitiveType.Cylinder : PrimitiveType.Sphere);
                holo.name = "Kagami_Hologram_" + i;
                holo.transform.SetParent(parent, false);
                holo.transform.position = positions[i];
                holo.transform.localScale = Vector3.one * (0.7f + i * 0.15f);
                Object.DestroyImmediate(holo.GetComponent<Collider>());
                holo.AddComponent<HologramSignAnimator>();
            }
        }

        private static void BuildCoverPoints(Transform parent)
        {
            Vector3[] points =
            {
                new Vector3(-6f, 0f, 3f), new Vector3(6f, 0f, 4f),
                new Vector3(-12f, 0f, 12f), new Vector3(-6f, 0f, 14f),
                new Vector3(6f, 0f, 16f), new Vector3(12f, 0f, 20f),
                new Vector3(-8f, 0f, 25f), new Vector3(8f, 0f, 27f)
            };

            for (int i = 0; i < points.Length; i++)
            {
                GameObject marker = new GameObject("CoverPoint_Kagami_" + i);
                marker.transform.SetParent(parent, false);
                marker.transform.position = points[i];
                marker.transform.rotation = Quaternion.Euler(0f, i % 2 == 0 ? 45f : -45f, 0f);
                marker.AddComponent<CoverPoint>();
            }
        }

        private static void EnsureCoverRegistry(GameObject root)
        {
            CoverRegistry registry = Object.FindFirstObjectByType<CoverRegistry>();
            if (registry == null) root.AddComponent<CoverRegistry>();
        }

        private static void CreateBlock(Transform parent, string name, Vector3 position, Vector3 scale)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.position = position;
            go.transform.localScale = scale;
        }

        private static Transform NewChild(Transform parent, string name)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go.transform;
        }
    }
}
