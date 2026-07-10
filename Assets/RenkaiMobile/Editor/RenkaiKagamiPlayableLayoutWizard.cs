using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using RenkaiMobile.Bots;
using RenkaiMobile.Objective;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiKagamiPlayableLayoutWizard
    {
        [MenuItem("Renkai Mobile/Build Kagami Playable Layout")]
        public static void Build()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Play Mode'u kapat ve tekrar çalıştır.", "OK");
                return;
            }

            GameObject old = GameObject.Find("Kagami_Playable_Layout");
            if (old != null) Object.DestroyImmediate(old);

            GameObject root = new GameObject("Kagami_Playable_Layout");
            Transform lanes = NewChild(root.transform, "Lanes");
            Transform sites = NewChild(root.transform, "Sites");
            Transform covers = NewChild(root.transform, "CoverGeometry");
            Transform spawns = NewChild(root.transform, "SpawnAreas");

            BuildLanes(lanes);
            BuildSites(sites);
            BuildCoverGeometry(covers);
            BuildSpawnAreas(spawns);

            CoverRegistry registry = Object.FindFirstObjectByType<CoverRegistry>();
            if (registry == null) registry = root.AddComponent<CoverRegistry>();
            registry.Refresh();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            EditorUtility.DisplayDialog("Renkai Mobile", "Kagami District oynanabilir 5v5 layout eklendi: mid, iki ana lane, hızlı defender connector, iki Zodiac Zone çevresi ve cover geometrisi hazır.", "OK");
        }

        private static void BuildLanes(Transform parent)
        {
            CreateFloor(parent, "Attacker_Staging", new Vector3(0f, 0f, -18f), new Vector3(18f, 0.3f, 10f));
            CreateFloor(parent, "Mid_Lane", new Vector3(0f, 0f, 6f), new Vector3(8f, 0.3f, 34f));
            CreateFloor(parent, "Left_Lane", new Vector3(-13f, 0f, 8f), new Vector3(7f, 0.3f, 36f));
            CreateFloor(parent, "Right_Lane", new Vector3(13f, 0f, 10f), new Vector3(7f, 0.3f, 38f));
            CreateFloor(parent, "Defender_Connector", new Vector3(0f, 0f, 30f), new Vector3(28f, 0.3f, 5f));
            CreateFloor(parent, "Defender_Staging", new Vector3(0f, 0f, 38f), new Vector3(18f, 0.3f, 10f));

            CreateWall(parent, "Mid_LeftWall", new Vector3(-4.5f, 1.8f, 6f), new Vector3(1f, 3.6f, 30f));
            CreateWall(parent, "Mid_RightWall", new Vector3(4.5f, 1.8f, 6f), new Vector3(1f, 3.6f, 30f));
            CreateWall(parent, "Left_OuterWall", new Vector3(-17f, 2f, 8f), new Vector3(1f, 4f, 36f));
            CreateWall(parent, "Right_OuterWall", new Vector3(17f, 2f, 10f), new Vector3(1f, 4f, 38f));
        }

        private static void BuildSites(Transform parent)
        {
            CreateSiteRoom(parent, "Zodiac_A_Room", new Vector3(-12f, 0f, 18f), new Vector3(10f, 0.3f, 12f));
            CreateSiteRoom(parent, "Zodiac_B_Room", new Vector3(12f, 0f, 22f), new Vector3(10f, 0.3f, 12f));

            CreateWall(parent, "A_BackWall", new Vector3(-12f, 2f, 24f), new Vector3(10f, 4f, 1f));
            CreateWall(parent, "B_BackWall", new Vector3(12f, 2f, 28f), new Vector3(10f, 4f, 1f));
            CreateWall(parent, "A_ShortWall", new Vector3(-7f, 1.5f, 18f), new Vector3(1f, 3f, 8f));
            CreateWall(parent, "B_ShortWall", new Vector3(7f, 1.5f, 22f), new Vector3(1f, 3f, 8f));

            ZodiacZone zoneA = EnsureZone("Zodiac_Zone_A", "A", new Vector3(-12f, 0.08f, 18f));
            ZodiacZone zoneB = EnsureZone("Zodiac_Zone_B", "B", new Vector3(12f, 0.08f, 22f));
            zoneA.transform.SetParent(parent, true);
            zoneB.transform.SetParent(parent, true);
        }

        private static void BuildCoverGeometry(Transform parent)
        {
            Vector3[] positions =
            {
                new Vector3(-2.5f, 0.75f, -4f), new Vector3(2.5f, 0.75f, 2f),
                new Vector3(-12f, 0.75f, 6f), new Vector3(-11f, 0.75f, 16f),
                new Vector3(-14f, 0.75f, 21f), new Vector3(12f, 0.75f, 10f),
                new Vector3(11f, 0.75f, 21f), new Vector3(14f, 0.75f, 25f),
                new Vector3(-3f, 0.75f, 26f), new Vector3(3f, 0.75f, 29f)
            };

            for (int i = 0; i < positions.Length; i++)
            {
                GameObject cover = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cover.name = "Kagami_Cover_" + i;
                cover.transform.SetParent(parent, false);
                cover.transform.position = positions[i];
                cover.transform.localScale = new Vector3(i % 3 == 0 ? 3.2f : 2.2f, 1.5f, 1.2f);

                GameObject point = new GameObject("CoverPoint_" + i);
                point.transform.SetParent(parent, false);
                point.transform.position = positions[i] + new Vector3(0f, 0f, i % 2 == 0 ? -1.4f : 1.4f);
                point.transform.rotation = Quaternion.Euler(0f, i % 2 == 0 ? 0f : 180f, 0f);
                point.AddComponent<CoverPoint>();
            }
        }

        private static void BuildSpawnAreas(Transform parent)
        {
            CreateMarker(parent, "Attackers_Spawn_Center", new Vector3(0f, 0.1f, -20f));
            CreateMarker(parent, "Defenders_Spawn_Center", new Vector3(0f, 0.1f, 39f));

            for (int i = 0; i < 5; i++)
            {
                CreateMarker(parent, "Attackers_Spawn_" + i, new Vector3(-4f + i * 2f, 0.1f, -20f));
                CreateMarker(parent, "Defenders_Spawn_" + i, new Vector3(-4f + i * 2f, 0.1f, 39f));
            }
        }

        private static ZodiacZone EnsureZone(string name, string id, Vector3 position)
        {
            GameObject zone = GameObject.Find(name);
            if (zone == null)
            {
                zone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                zone.name = name;
                zone.transform.localScale = new Vector3(4.5f, 0.05f, 4.5f);
                Collider c = zone.GetComponent<Collider>();
                c.isTrigger = true;
            }
            zone.transform.position = position;
            ZodiacZone component = zone.GetComponent<ZodiacZone>() ?? zone.AddComponent<ZodiacZone>();
            component.Configure(id);
            return component;
        }

        private static void CreateFloor(Transform parent, string name, Vector3 position, Vector3 scale)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.position = position;
            go.transform.localScale = scale;
        }

        private static void CreateSiteRoom(Transform parent, string name, Vector3 position, Vector3 scale)
        {
            CreateFloor(parent, name, position, scale);
        }

        private static void CreateWall(Transform parent, string name, Vector3 position, Vector3 scale)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.position = position;
            go.transform.localScale = scale;
        }

        private static void CreateMarker(Transform parent, string name, Vector3 position)
        {
            GameObject marker = new GameObject(name);
            marker.transform.SetParent(parent, false);
            marker.transform.position = position;
        }

        private static Transform NewChild(Transform parent, string name)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go.transform;
        }
    }
}
