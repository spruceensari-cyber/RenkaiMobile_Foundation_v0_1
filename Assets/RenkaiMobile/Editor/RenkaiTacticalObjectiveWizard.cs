using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.Objective;
using RenkaiMobile.Rounds;
using RenkaiMobile.UI;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiTacticalObjectiveWizard
    {
        [MenuItem("Renkai Mobile/Add Tactical Objective Layer")]
        public static void Build()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Play Mode'u kapat ve tekrar çalıştır.", "OK");
                return;
            }

            MobileRoundManager roundManager = Object.FindFirstObjectByType<MobileRoundManager>();
            if (roundManager == null)
            {
                GameObject root = GameObject.Find("Renkai_5v5_Match") ?? new GameObject("Renkai_5v5_Match");
                roundManager = root.AddComponent<MobileRoundManager>();
            }

            GameObject objectiveRoot = GameObject.Find("Zodiac_Objective") ?? new GameObject("Zodiac_Objective");
            ZodiacObjective objective = objectiveRoot.GetComponent<ZodiacObjective>() ?? objectiveRoot.AddComponent<ZodiacObjective>();
            Bind(objective, "roundManager", roundManager);

            CreateZone("Zodiac_Zone_A", new Vector3(-10f, 0.05f, 10f), "A");
            CreateZone("Zodiac_Zone_B", new Vector3(10f, 0.05f, 18f), "B");
            CreateObjectiveHud(objective);

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            EditorUtility.DisplayDialog("Renkai Mobile", "Zodiac Zone A/B, mobile objective ve objective HUD eklendi.", "OK");
        }

        private static void CreateZone(string name, Vector3 position, string id)
        {
            GameObject old = GameObject.Find(name);
            if (old != null) Object.DestroyImmediate(old);

            GameObject zoneObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            zoneObject.name = name;
            zoneObject.transform.position = position;
            zoneObject.transform.localScale = new Vector3(4f, 0.05f, 4f);
            Collider collider = zoneObject.GetComponent<Collider>();
            collider.isTrigger = true;
            ZodiacZone zone = zoneObject.AddComponent<ZodiacZone>();
            zone.Configure(id);
        }

        private static void CreateObjectiveHud(ZodiacObjective objective)
        {
            GameObject old = GameObject.Find("ZodiacObjectiveHUD");
            if (old != null) Object.DestroyImmediate(old);

            GameObject canvasGo = new GameObject("ZodiacObjectiveHUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(ZodiacObjectiveHud));
            Canvas canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            Text state = CreateText(canvasGo.transform, "State", "ZODIAC DORMANT", new Vector2(0f, -86f), 30);
            Text timer = CreateText(canvasGo.transform, "CollapseTimer", string.Empty, new Vector2(0f, -128f), 38);

            GameObject progressGo = new GameObject("InteractionProgress", typeof(RectTransform), typeof(Image));
            progressGo.transform.SetParent(canvasGo.transform, false);
            Image progress = progressGo.GetComponent<Image>();
            progress.type = Image.Type.Filled;
            progress.fillMethod = Image.FillMethod.Radial360;
            RectTransform progressRt = progress.rectTransform;
            progressRt.anchorMin = progressRt.anchorMax = new Vector2(0.5f, 0.5f);
            progressRt.sizeDelta = new Vector2(120f, 120f);

            ZodiacObjectiveHud hud = canvasGo.GetComponent<ZodiacObjectiveHud>();
            SerializedObject so = new SerializedObject(hud);
            SetObject(so, "objective", objective);
            SetObject(so, "stateText", state);
            SetObject(so, "timerText", timer);
            SetObject(so, "progressFill", progress);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Text CreateText(Transform parent, string name, string content, Vector2 pos, int size)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            Text text = go.GetComponent<Text>();
            text.text = content;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.alignment = TextAnchor.UpperCenter;
            text.color = Color.white;

            RectTransform rt = text.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(760f, 60f);
            return text;
        }

        private static void Bind(Object target, string propertyName, Object value)
        {
            SerializedObject so = new SerializedObject(target);
            SetObject(so, propertyName, value);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetObject(SerializedObject so, string propertyName, Object value)
        {
            SerializedProperty property = so.FindProperty(propertyName);
            if (property != null) property.objectReferenceValue = value;
        }
    }
}
