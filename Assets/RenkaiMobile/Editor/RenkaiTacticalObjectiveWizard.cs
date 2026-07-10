using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Renkai.Rounds;
using RenkaiMobile.Objective;
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

            var roundManager = Object.FindFirstObjectByType<RoundManager>();
            if (roundManager == null)
            {
                var root = GameObject.Find("Renkai_5v5_Match") ?? new GameObject("Renkai_5v5_Match");
                roundManager = root.AddComponent<RoundManager>();
            }

            var objectiveRoot = GameObject.Find("SpiritCore_Objective") ?? new GameObject("SpiritCore_Objective");
            var objective = objectiveRoot.GetComponent<SpiritCoreRoundObjective>() ?? objectiveRoot.AddComponent<SpiritCoreRoundObjective>();
            Bind(objective, "roundManager", roundManager);

            CreateSite("Site_A", new Vector3(-10f, 0.05f, 10f), "A");
            CreateSite("Site_B", new Vector3(10f, 0.05f, 18f), "B");
            CreateRoundHud(roundManager, objective);

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            EditorUtility.DisplayDialog("Renkai Mobile", "A/B site, Spirit Core objective ve tactical round HUD eklendi.", "OK");
        }

        private static void CreateSite(string name, Vector3 position, string id)
        {
            var old = GameObject.Find(name);
            if (old != null) Object.DestroyImmediate(old);

            var site = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            site.name = name;
            site.transform.position = position;
            site.transform.localScale = new Vector3(4f, 0.05f, 4f);
            var collider = site.GetComponent<Collider>();
            collider.isTrigger = true;
            var zone = site.AddComponent<SpiritCoreSiteZone>();
            zone.Configure(id);
        }

        private static void CreateRoundHud(RoundManager roundManager, SpiritCoreRoundObjective objective)
        {
            var old = GameObject.Find("TacticalRoundHUD");
            if (old != null) Object.DestroyImmediate(old);

            var canvasGo = new GameObject("TacticalRoundHUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(TacticalRoundHud));
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            Text score = CreateText(canvasGo.transform, "Score", "0  -  0", new Vector2(0f, -42f), 42, TextAnchor.UpperCenter);
            Text phase = CreateText(canvasGo.transform, "Phase", "BUY PHASE", new Vector2(0f, -92f), 26, TextAnchor.UpperCenter);
            Text timer = CreateText(canvasGo.transform, "Timer", "00:20", new Vector2(0f, -132f), 34, TextAnchor.UpperCenter);
            Text objectiveText = CreateText(canvasGo.transform, "ObjectiveStatus", "", new Vector2(0f, -180f), 28, TextAnchor.UpperCenter);

            var hud = canvasGo.GetComponent<TacticalRoundHud>();
            var so = new SerializedObject(hud);
            so.FindProperty("roundManager").objectReferenceValue = roundManager;
            so.FindProperty("objective").objectReferenceValue = objective;
            so.FindProperty("scoreText").objectReferenceValue = score;
            so.FindProperty("phaseText").objectReferenceValue = phase;
            so.FindProperty("timerText").objectReferenceValue = timer;
            so.FindProperty("objectiveText").objectReferenceValue = objectiveText;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Text CreateText(Transform parent, string name, string content, Vector2 pos, int size, TextAnchor alignment)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var text = go.GetComponent<Text>();
            text.text = content;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.alignment = alignment;
            text.color = Color.white;
            var rt = text.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(700f, 60f);
            return text;
        }

        private static void Bind(Object target, string propertyName, Object value)
        {
            var so = new SerializedObject(target);
            so.FindProperty(propertyName).objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
