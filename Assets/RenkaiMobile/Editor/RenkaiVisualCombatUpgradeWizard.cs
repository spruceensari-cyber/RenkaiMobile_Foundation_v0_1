using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Renkai.Combat;
using RenkaiMobile.Combat;
using RenkaiMobile.UI;
using RenkaiMobile.Input;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiVisualCombatUpgradeWizard
    {
        [MenuItem("Renkai Mobile/Upgrade Visuals & Combat Feel")]
        public static void Upgrade()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Play Mode'u kapat ve tekrar çalıştır.", "OK");
                return;
            }

            var player = GameObject.Find("Player");
            if (player == null)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Player bulunamadı. Önce First Playable Scene kur.", "OK");
                return;
            }

            var legacyDebug = player.GetComponent<EditorFpsDebugInput>();
            if (legacyDebug != null) Object.DestroyImmediate(legacyDebug);

            CreateMaterials();
            ThemeArena();
            BuildWeaponViewModel(player);
            UpgradeTargets();
            AddAtmosphere();
            AddAmmoHud();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Renkai Mobile", "Visual & Combat Feel upgrade tamamlandı.", "OK");
        }

        private static void CreateMaterials()
        {
            EnsureFolder("Assets/Renkai", "Art");
            EnsureFolder("Assets/Renkai/Art", "Materials");
            CreateMaterial("Mat_ArenaFloor", new Color(0.035f, 0.055f, 0.08f), 0.15f, 0.72f);
            CreateMaterial("Mat_ArenaWall", new Color(0.09f, 0.11f, 0.16f), 0.1f, 0.6f);
            CreateMaterial("Mat_Cyan", new Color(0.04f, 0.58f, 0.9f), 0.2f, 0.5f);
            CreateMaterial("Mat_Magenta", new Color(0.62f, 0.08f, 0.75f), 0.18f, 0.5f);
            CreateMaterial("Mat_WeaponDark", new Color(0.035f, 0.04f, 0.055f), 0.55f, 0.72f);
            CreateMaterial("Mat_WeaponAccent", new Color(0.06f, 0.62f, 1f), 0.35f, 0.62f);
        }

        private static Material CreateMaterial(string name, Color color, float metallic, float smoothness)
        {
            string path = "Assets/Renkai/Art/Materials/" + name + ".mat";
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

        private static void ThemeArena()
        {
            var floorMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Renkai/Art/Materials/Mat_ArenaFloor.mat");
            var wallMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Renkai/Art/Materials/Mat_ArenaWall.mat");
            var cyan = AssetDatabase.LoadAssetAtPath<Material>("Assets/Renkai/Art/Materials/Mat_Cyan.mat");
            var magenta = AssetDatabase.LoadAssetAtPath<Material>("Assets/Renkai/Art/Materials/Mat_Magenta.mat");

            SetRendererMaterial("ArenaFloor", floorMat);
            SetRendererMaterial("Cover_A", wallMat);
            SetRendererMaterial("Cover_B", wallMat);
            SetRendererMaterial("Cover_C", wallMat);
            SetRendererMaterial("BackWall", wallMat);

            var old = GameObject.Find("RenkaiVisualSet");
            if (old != null) Object.DestroyImmediate(old);
            var root = new GameObject("RenkaiVisualSet");

            for (int i = -4; i <= 4; i++)
                CreateStrip(root.transform, new Vector3(i * 4f, 0.012f, 10f), new Vector3(0.08f, 0.02f, 60f), i % 2 == 0 ? cyan : magenta);

            CreateStrip(root.transform, new Vector3(0f, 0.015f, 2f), new Vector3(32f, 0.02f, 0.08f), cyan);
            CreateStrip(root.transform, new Vector3(0f, 0.015f, 30f), new Vector3(32f, 0.02f, 0.08f), magenta);

            for (int side = -1; side <= 1; side += 2)
            {
                for (int i = 0; i < 5; i++)
                {
                    var pillar = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    pillar.name = "CyberPillar";
                    pillar.transform.SetParent(root.transform);
                    pillar.transform.position = new Vector3(side * 16f, 2f, i * 8f);
                    pillar.transform.localScale = new Vector3(0.8f, 4f, 0.8f);
                    pillar.GetComponent<Renderer>().sharedMaterial = wallMat;
                    CreateStrip(pillar.transform, new Vector3(0f, 0f, -0.42f), new Vector3(0.18f, 3.2f, 0.03f), side < 0 ? cyan : magenta);
                }
            }
        }

        private static void BuildWeaponViewModel(GameObject player)
        {
            Camera cam = player.GetComponentInChildren<Camera>();
            if (cam == null) return;
            Transform old = cam.transform.Find("RenkaiRifle_ViewModel");
            if (old != null) Object.DestroyImmediate(old.gameObject);

            var root = new GameObject("RenkaiRifle_ViewModel");
            root.transform.SetParent(cam.transform, false);
            root.transform.localPosition = new Vector3(0.32f, -0.28f, 0.62f);
            root.transform.localRotation = Quaternion.Euler(2f, -4f, 0f);

            var dark = AssetDatabase.LoadAssetAtPath<Material>("Assets/Renkai/Art/Materials/Mat_WeaponDark.mat");
            var accent = AssetDatabase.LoadAssetAtPath<Material>("Assets/Renkai/Art/Materials/Mat_WeaponAccent.mat");

            CreatePart(root.transform, "Body", new Vector3(0f, 0f, 0f), new Vector3(0.18f, 0.16f, 0.72f), dark);
            CreatePart(root.transform, "Barrel", new Vector3(0f, 0.02f, 0.48f), new Vector3(0.07f, 0.07f, 0.42f), dark);
            CreatePart(root.transform, "Accent", new Vector3(0.06f, 0.04f, 0.05f), new Vector3(0.035f, 0.04f, 0.42f), accent);
            CreatePart(root.transform, "Grip", new Vector3(0f, -0.16f, -0.04f), new Vector3(0.09f, 0.24f, 0.13f), dark);
            CreatePart(root.transform, "Sight", new Vector3(0f, 0.12f, 0.12f), new Vector3(0.08f, 0.06f, 0.15f), accent);

            root.AddComponent<WeaponSway>();
            root.AddComponent<WeaponKickback>();

            var weapon = player.GetComponentInChildren<HitscanWeapon>();
            if (weapon != null && weapon.GetComponent<WeaponFeelBridge>() == null)
                weapon.gameObject.AddComponent<WeaponFeelBridge>();
        }

        private static void UpgradeTargets()
        {
            var healthObjects = Object.FindObjectsByType<Health>(FindObjectsSortMode.None);
            foreach (var health in healthObjects)
            {
                if (health.GetComponent<TargetDummyFeedback>() == null)
                    health.gameObject.AddComponent<TargetDummyFeedback>();
            }
        }

        private static void AddAmmoHud()
        {
            var canvas = Object.FindFirstObjectByType<Canvas>();
            var weapon = Object.FindFirstObjectByType<HitscanWeapon>();
            if (canvas == null || weapon == null) return;

            var old = GameObject.Find("AmmoPanel");
            if (old != null) Object.DestroyImmediate(old);

            var panel = new GameObject("AmmoPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(AmmoHud));
            panel.transform.SetParent(canvas.transform, false);
            var image = panel.GetComponent<Image>();
            image.color = new Color(0.02f, 0.035f, 0.07f, 0.78f);
            var rt = panel.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(1f, 0f);
            rt.pivot = new Vector2(1f, 0f);
            rt.anchoredPosition = new Vector2(-36f, 32f);
            rt.sizeDelta = new Vector2(240f, 82f);

            var textGo = new GameObject("AmmoText", typeof(RectTransform), typeof(Text));
            textGo.transform.SetParent(panel.transform, false);
            var text = textGo.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 34;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(0.65f, 0.9f, 1f);
            text.text = "25 / 25";
            var textRt = text.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;

            var hud = panel.GetComponent<AmmoHud>();
            var so = new SerializedObject(hud);
            so.FindProperty("weapon").objectReferenceValue = weapon;
            so.FindProperty("ammoText").objectReferenceValue = text;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void AddAtmosphere()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.08f, 0.12f, 0.2f);
            RenderSettings.ambientEquatorColor = new Color(0.04f, 0.05f, 0.09f);
            RenderSettings.ambientGroundColor = new Color(0.015f, 0.02f, 0.035f);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = new Color(0.035f, 0.055f, 0.09f);
            RenderSettings.fogDensity = 0.007f;

            var light = Object.FindFirstObjectByType<Light>();
            if (light != null)
            {
                light.color = new Color(0.78f, 0.86f, 1f);
                light.intensity = 1.05f;
                light.shadows = LightShadows.Soft;
            }
        }

        private static void CreatePart(Transform parent, string name, Vector3 localPos, Vector3 scale, Material mat)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = mat;
            var col = go.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col);
        }

        private static void CreateStrip(Transform parent, Vector3 position, Vector3 scale, Material mat)
        {
            var strip = GameObject.CreatePrimitive(PrimitiveType.Cube);
            strip.name = "EnergyStrip";
            strip.transform.SetParent(parent, false);
            strip.transform.position = position;
            strip.transform.localScale = scale;
            strip.GetComponent<Renderer>().sharedMaterial = mat;
            var col = strip.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col);
        }

        private static void SetRendererMaterial(string objectName, Material mat)
        {
            var go = GameObject.Find(objectName);
            var r = go != null ? go.GetComponent<Renderer>() : null;
            if (r != null) r.sharedMaterial = mat;
        }

        private static void EnsureFolder(string parent, string child)
        {
            string full = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(full)) AssetDatabase.CreateFolder(parent, child);
        }
    }
}
