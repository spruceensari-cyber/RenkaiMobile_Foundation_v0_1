using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Renkai.Movement;
using Renkai.Combat;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiFirstPlayableWizard
    {
        [MenuItem("Renkai Mobile/Build First Playable Scene")]
        public static void BuildFirstPlayableScene()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Play Mode'u kapat ve komutu tekrar çalıştır.", "OK");
                return;
            }

            if (!EditorUtility.DisplayDialog("Renkai Mobile", "Create a new First Playable test scene? Unsaved scene changes may be lost.", "Create", "Cancel"))
                return;

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SceneManager.SetActiveScene(scene);

            CreateLighting();
            CreateArena();
            CreatePlayer();
            CreateTargets();

            const string folder = "Assets/Renkai/Scenes/Maps";
            if (!AssetDatabase.IsValidFolder("Assets/Renkai/Scenes"))
                AssetDatabase.CreateFolder("Assets/Renkai", "Scenes");
            if (!AssetDatabase.IsValidFolder(folder))
                AssetDatabase.CreateFolder("Assets/Renkai/Scenes", "Maps");

            string path = folder + "/Renkai_TestRange.unity";
            EditorSceneManager.SaveScene(scene, path);
            AssetDatabase.SaveAssets();
            Selection.activeGameObject = GameObject.Find("Player");
            EditorUtility.DisplayDialog("Renkai Mobile", "First Playable scene created at:\n" + path, "OK");
        }

        private static void CreateLighting()
        {
            var lightGo = new GameObject("Directional Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.15f;
            lightGo.transform.rotation = Quaternion.Euler(45f, -35f, 0f);
        }

        private static void CreateArena()
        {
            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "ArenaFloor";
            floor.transform.localScale = new Vector3(8f, 1f, 8f);

            CreateBlock("Cover_A", new Vector3(0f, 1f, 12f), new Vector3(8f, 2f, 1f));
            CreateBlock("Cover_B", new Vector3(-10f, 1f, 22f), new Vector3(4f, 2f, 1f));
            CreateBlock("Cover_C", new Vector3(10f, 1f, 22f), new Vector3(4f, 2f, 1f));
            CreateBlock("BackWall", new Vector3(0f, 2f, 34f), new Vector3(20f, 4f, 1f));
        }

        private static void CreateBlock(string name, Vector3 position, Vector3 scale)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.position = position;
            go.transform.localScale = scale;
        }

        private static void CreatePlayer()
        {
            var player = new GameObject("Player");
            player.transform.position = new Vector3(0f, 1f, -8f);
            player.AddComponent<CharacterController>();
            player.AddComponent<MobileFpsMotor>();

            var pitch = new GameObject("PitchRoot");
            pitch.transform.SetParent(player.transform);
            pitch.transform.localPosition = new Vector3(0f, 1.6f, 0f);

            var cameraGo = new GameObject("Main Camera");
            cameraGo.tag = "MainCamera";
            cameraGo.transform.SetParent(pitch.transform);
            cameraGo.transform.localPosition = Vector3.zero;
            cameraGo.transform.localRotation = Quaternion.identity;
            cameraGo.AddComponent<Camera>();
            cameraGo.AddComponent<AudioListener>();
        }

        private static void CreateTargets()
        {
            for (int i = 0; i < 5; i++)
            {
                var target = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                target.name = "TargetDummy_" + (i + 1);
                target.transform.position = new Vector3(-8f + i * 4f, 1f, 24f);
                target.AddComponent<Health>();

                var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                head.name = "HeadshotZone";
                head.transform.SetParent(target.transform, false);
                head.transform.localPosition = new Vector3(0f, 0.78f, 0f);
                head.transform.localScale = Vector3.one * 0.45f;
                head.AddComponent<HeadshotZone>();
            }
        }
    }
}
