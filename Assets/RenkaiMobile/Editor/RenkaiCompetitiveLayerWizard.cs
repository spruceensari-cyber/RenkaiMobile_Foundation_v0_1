using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.Economy;
using RenkaiMobile.UI;
using RenkaiMobile.Spectate;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiCompetitiveLayerWizard
    {
        [MenuItem("Renkai Mobile/Add Competitive Match Layer")]
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

            var wallet = player.GetComponent<CreditWallet>() ?? player.AddComponent<CreditWallet>();
            var buy = player.GetComponent<BuyPhaseController>() ?? player.AddComponent<BuyPhaseController>();
            Bind(buy, "wallet", wallet);

            CreateCompetitiveHud();

            if (player.GetComponent<TeamSpectateController>() == null)
                player.AddComponent<TeamSpectateController>();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            EditorUtility.DisplayDialog("Renkai Mobile", "Competitive layer eklendi: economy, buy controller, team status HUD, kill feed ve spectate temeli.", "OK");
        }

        private static void CreateCompetitiveHud()
        {
            GameObject old = GameObject.Find("CompetitiveMatchHUD");
            if (old != null) Object.DestroyImmediate(old);

            var canvasGo = new GameObject("CompetitiveMatchHUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            var teamRoot = new GameObject("TeamStatus", typeof(RectTransform), typeof(TeamStatusHud));
            teamRoot.transform.SetParent(canvasGo.transform, false);
            RectTransform teamRt = teamRoot.GetComponent<RectTransform>();
            teamRt.anchorMin = teamRt.anchorMax = new Vector2(0.5f, 1f);
            teamRt.pivot = new Vector2(0.5f, 1f);
            teamRt.anchoredPosition = new Vector2(0f, -18f);
            teamRt.sizeDelta = new Vector2(1500f, 52f);

            Text attackers = CreateText(teamRoot.transform, "Attackers", "ATTACKERS  ●●●●●", new Vector2(-390f, 0f), new Vector2(620f, 52f), TextAnchor.MiddleRight, 24);
            Text defenders = CreateText(teamRoot.transform, "Defenders", "●●●●●  DEFENDERS", new Vector2(390f, 0f), new Vector2(620f, 52f), TextAnchor.MiddleLeft, 24);

            var teamHud = teamRoot.GetComponent<TeamStatusHud>();
            var teamSo = new SerializedObject(teamHud);
            teamSo.FindProperty("attackersText").objectReferenceValue = attackers;
            teamSo.FindProperty("defendersText").objectReferenceValue = defenders;
            teamSo.ApplyModifiedPropertiesWithoutUndo();

            var feedRootGo = new GameObject("KillFeedRoot", typeof(RectTransform), typeof(VerticalLayoutGroup));
            feedRootGo.transform.SetParent(canvasGo.transform, false);
            RectTransform feedRt = feedRootGo.GetComponent<RectTransform>();
            feedRt.anchorMin = feedRt.anchorMax = new Vector2(1f, 1f);
            feedRt.pivot = new Vector2(1f, 1f);
            feedRt.anchoredPosition = new Vector2(-28f, -110f);
            feedRt.sizeDelta = new Vector2(560f, 300f);
            VerticalLayoutGroup layout = feedRootGo.GetComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.UpperRight;
            layout.spacing = 4f;

            var killFeed = canvasGo.AddComponent<KillFeedController>();
            var killSo = new SerializedObject(killFeed);
            killSo.FindProperty("feedRoot").objectReferenceValue = feedRt;
            killSo.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Text CreateText(Transform parent, string name, string value, Vector2 position, Vector2 size, TextAnchor anchor, int fontSize)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            Text text = go.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.alignment = anchor;
            text.color = Color.white;
            text.text = value;
            RectTransform rt = text.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = position;
            rt.sizeDelta = size;
            return text;
        }

        private static void Bind(Object target, string propertyName, Object value)
        {
            var so = new SerializedObject(target);
            var property = so.FindProperty(propertyName);
            if (property != null) property.objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
