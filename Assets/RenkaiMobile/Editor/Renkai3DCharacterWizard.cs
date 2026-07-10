using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Renkai.Combat;
using RenkaiMobile.Characters;

namespace RenkaiMobile.EditorTools
{
    public static class Renkai3DCharacterWizard
    {
        private const string MatRoot = "Assets/Renkai/Art/Materials";

        [MenuItem("Renkai Mobile/Add 3D Character Layer")]
        public static void Build()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Play Mode'u kapat ve tekrar çalıştır.", "OK");
                return;
            }

            var targets = Object.FindObjectsByType<Health>(FindObjectsSortMode.None);
            if (targets.Length == 0)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Sahnede Health component'li hedef bulunamadı.", "OK");
                return;
            }

            CreateCharacterMaterials();
            int index = 0;
            foreach (var health in targets)
            {
                if (!health.gameObject.name.StartsWith("TargetDummy_")) continue;
                UpgradeTarget(health.gameObject, index++);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Renkai Mobile", "3D karakter katmanı eklendi: Raika, Akari ve Kuroha varyasyonları hedeflere uygulandı.", "OK");
        }

        private static void UpgradeTarget(GameObject target, int index)
        {
            var primitiveRenderer = target.GetComponent<Renderer>();
            if (primitiveRenderer != null) primitiveRenderer.enabled = false;
            var capsule = target.GetComponent<CapsuleCollider>();
            if (capsule != null) capsule.enabled = false;

            Transform old = target.transform.Find("AgentVisual");
            if (old != null) Object.DestroyImmediate(old.gameObject);

            string[] names = { "Raika", "Akari", "Kuroha" };
            AgentRole[] roles = { AgentRole.Duelist, AgentRole.Initiator, AgentRole.Controller };
            Color[] accents = {
                new Color(0.1f, 0.7f, 1f),
                new Color(1f, 0.18f, 0.08f),
                new Color(0.62f, 0.15f, 0.95f)
            };

            int variant = index % 3;
            var identity = target.GetComponent<RenkaiCharacterIdentity>() ?? target.AddComponent<RenkaiCharacterIdentity>();
            identity.Configure(names[variant].ToLowerInvariant(), names[variant], roles[variant], accents[variant]);

            var visual = new GameObject("AgentVisual");
            visual.transform.SetParent(target.transform, false);

            var baseMat = AssetDatabase.LoadAssetAtPath<Material>(MatRoot + "/Mat_AgentBase.mat");
            var accentMat = AssetDatabase.LoadAssetAtPath<Material>(MatRoot + "/Mat_AgentAccent_" + names[variant] + ".mat");
            var skinMat = AssetDatabase.LoadAssetAtPath<Material>(MatRoot + "/Mat_AgentSkin.mat");

            Transform pelvis = Part(visual.transform, "Pelvis", new Vector3(0f, 0.95f, 0f), new Vector3(0.38f, 0.28f, 0.26f), baseMat);
            Transform torso = Part(visual.transform, "Torso", new Vector3(0f, 1.35f, 0f), new Vector3(0.52f, 0.62f, 0.3f), baseMat);
            Part(torso, "ChestAccent", new Vector3(0f, 0.08f, -0.165f), new Vector3(0.34f, 0.09f, 0.02f), accentMat);

            Transform head = Sphere(visual.transform, "Head", new Vector3(0f, 1.92f, 0f), new Vector3(0.28f, 0.32f, 0.28f), skinMat);
            if (head.GetComponent<HeadshotZone>() == null) head.gameObject.AddComponent<HeadshotZone>();

            Transform hair = Sphere(visual.transform, "Hair", new Vector3(0f, 2.05f, 0.02f), new Vector3(0.33f, 0.22f, 0.3f), accentMat);
            hair.localScale = new Vector3(0.33f, 0.22f, 0.3f);

            Transform leftArm = Limb(visual.transform, "LeftArm", new Vector3(-0.38f, 1.38f, 0f), new Vector3(0.14f, 0.55f, 0.14f), baseMat);
            Transform rightArm = Limb(visual.transform, "RightArm", new Vector3(0.38f, 1.38f, 0f), new Vector3(0.14f, 0.55f, 0.14f), baseMat);
            Transform leftLeg = Limb(visual.transform, "LeftLeg", new Vector3(-0.14f, 0.48f, 0f), new Vector3(0.18f, 0.78f, 0.18f), baseMat);
            Transform rightLeg = Limb(visual.transform, "RightLeg", new Vector3(0.14f, 0.48f, 0f), new Vector3(0.18f, 0.78f, 0.18f), baseMat);

            Part(visual.transform, "EnergyCore", new Vector3(0f, 1.45f, -0.19f), new Vector3(0.12f, 0.12f, 0.03f), accentMat);

            var motion = target.GetComponent<ProceduralAgentMotion>() ?? target.AddComponent<ProceduralAgentMotion>();
            motion.Configure(torso, leftArm, rightArm, leftLeg, rightLeg);
            if (target.GetComponent<SimpleAgentPatrol>() == null) target.AddComponent<SimpleAgentPatrol>();
        }

        private static void CreateCharacterMaterials()
        {
            EnsureFolder("Assets/Renkai", "Art");
            EnsureFolder("Assets/Renkai/Art", "Materials");
            CreateMaterial("Mat_AgentBase", new Color(0.035f, 0.045f, 0.07f), 0.3f, 0.55f);
            CreateMaterial("Mat_AgentSkin", new Color(0.82f, 0.62f, 0.5f), 0.02f, 0.32f);
            CreateMaterial("Mat_AgentAccent_Raika", new Color(0.08f, 0.68f, 1f), 0.15f, 0.6f);
            CreateMaterial("Mat_AgentAccent_Akari", new Color(1f, 0.14f, 0.035f), 0.12f, 0.58f);
            CreateMaterial("Mat_AgentAccent_Kuroha", new Color(0.6f, 0.08f, 0.92f), 0.15f, 0.62f);
        }

        private static Material CreateMaterial(string name, Color color, float metallic, float smoothness)
        {
            string path = MatRoot + "/" + name + ".mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) return existing;
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            var mat = new Material(shader) { name = name };
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color); else mat.color = color;
            if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }

        private static Transform Part(Transform parent, string name, Vector3 localPos, Vector3 scale, Material mat)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = mat;
            Object.DestroyImmediate(go.GetComponent<Collider>());
            return go.transform;
        }

        private static Transform Sphere(Transform parent, string name, Vector3 localPos, Vector3 scale, Material mat)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = mat;
            return go.transform;
        }

        private static Transform Limb(Transform parent, string name, Vector3 localPos, Vector3 scale, Material mat)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = mat;
            Object.DestroyImmediate(go.GetComponent<Collider>());
            return go.transform;
        }

        private static void EnsureFolder(string parent, string child)
        {
            string full = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(full)) AssetDatabase.CreateFolder(parent, child);
        }
    }
}
