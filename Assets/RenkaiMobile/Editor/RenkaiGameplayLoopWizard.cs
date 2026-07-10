using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Renkai.Core;
using Renkai.Rounds;
using RenkaiMobile.Abilities;
using RenkaiMobile.Audio;
using RenkaiMobile.Bots;
using RenkaiMobile.Economy;
using RenkaiMobile.Objective;
using RenkaiMobile.UI;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiGameplayLoopWizard
    {
        [MenuItem("Renkai Mobile/Add Gameplay Loop Integration")]
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

            RoundManager round = Object.FindFirstObjectByType<RoundManager>();
            if (round == null)
            {
                GameObject root = GameObject.Find("Renkai_5v5_Match") ?? new GameObject("Renkai_5v5_Match");
                round = root.AddComponent<RoundManager>();
            }

            SetupPlayer(player, round);
            SetupBots();
            SetupEconomy(round);
            SetupUi(player, round);

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            EditorUtility.DisplayDialog("Renkai Mobile", "Gameplay loop integration eklendi: economy rewards, buy menu, minimap, footsteps, Spirit Core carrier, bot site rotation ve ability runtime.", "OK");
        }

        private static void SetupPlayer(GameObject player, RoundManager round)
        {
            var wallet = player.GetComponent<CreditWallet>() ?? player.AddComponent<CreditWallet>();
            var buy = player.GetComponent<BuyPhaseController>() ?? player.AddComponent<BuyPhaseController>();
            Bind(buy, "roundManager", round);
            Bind(buy, "wallet", wallet);

            if (player.GetComponent<SpiritCoreCarrier>() == null) player.AddComponent<SpiritCoreCarrier>();
            if (player.GetComponent<AbilityRuntimeController>() == null) player.AddComponent<AbilityRuntimeController>();

            if (player.GetComponent<AudioSource>() == null) player.AddComponent<AudioSource>();
            if (player.GetComponent<ProceduralFootsteps>() == null) player.AddComponent<ProceduralFootsteps>();
        }

        private static void SetupBots()
        {
            TeamMember[] members = Object.FindObjectsByType<TeamMember>(FindObjectsSortMode.None);
            foreach (TeamMember member in members)
            {
                if (member == null || member.gameObject.name == "Player") continue;
                if (member.GetComponent<SiteRotationBot>() == null)
                    member.gameObject.AddComponent<SiteRotationBot>();
            }
        }

        private static void SetupEconomy(RoundManager round)
        {
            GameObject root = GameObject.Find("Renkai_5v5_Match") ?? new GameObject("Renkai_5v5_Match");
            var rewards = root.GetComponent<RoundEconomyRewards>() ?? root.AddComponent<RoundEconomyRewards>();
            Bind(rewards, "roundManager", round);
        }

        private static void SetupUi(GameObject player, RoundManager round)
        {
            GameObject old = GameObject.Find("GameplayLoopHUD");
            if (old != null) Object.DestroyImmediate(old);

            var canvasGo = new GameObject("GameplayLoopHUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            GameObject buyPanel = new GameObject("BuyMenuPanel", typeof(RectTransform), typeof(Image));
            buyPanel.transform.SetParent(canvasGo.transform, false);
            RectTransform buyRt = buyPanel.GetComponent<RectTransform>();
            buyRt.anchorMin = buyRt.anchorMax = new Vector2(0.5f, 0.5f);
            buyRt.sizeDelta = new Vector2(900f, 520f);
            buyPanel.GetComponent<Image>().color = new Color(0.035f, 0.045f, 0.08f, 0.94f);

            Text title = CreateText(buyPanel.transform, "Title", "RENKAI ARMORY", new Vector2(0f, 210f), new Vector2(700f, 60f), 34);
            Text credits = CreateText(buyPanel.transform, "Credits", "800 CR", new Vector2(0f, 155f), new Vector2(500f, 50f), 28);
            Text loadout = CreateText(buyPanel.transform, "Loadout", "RIFLE: --   SHIELD: --   ABILITY: 0", new Vector2(0f, 105f), new Vector2(800f, 45f), 22);

            BuyMenuController menu = canvasGo.AddComponent<BuyMenuController>();
            Bind(menu, "roundManager", round);
            Bind(menu, "buyController", player.GetComponent<BuyPhaseController>());
            Bind(menu, "wallet", player.GetComponent<CreditWallet>());
            Bind(menu, "panel", buyPanel);
            Bind(menu, "creditsText", credits);
            Bind(menu, "loadoutText", loadout);

            GameObject mapBg = new GameObject("Minimap", typeof(RectTransform), typeof(Image), typeof(SimpleMinimapController));
            mapBg.transform.SetParent(canvasGo.transform, false);
            RectTransform mapRt = mapBg.GetComponent<RectTransform>();
            mapRt.anchorMin = mapRt.anchorMax = new Vector2(0f, 1f);
            mapRt.pivot = new Vector2(0f, 1f);
            mapRt.anchoredPosition = new Vector2(24f, -24f);
            mapRt.sizeDelta = new Vector2(260f, 260f);
            mapBg.GetComponent<Image>().color = new Color(0.02f, 0.03f, 0.055f, 0.82f);
            var minimap = mapBg.GetComponent<SimpleMinimapController>();
            Bind(minimap, "mapRoot", mapRt);
        }

        private static Text CreateText(Transform parent, string name, string value, Vector2 position, Vector2 size, int fontSize)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            Text text = go.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.alignment = TextAnchor.MiddleCenter;
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
            SerializedObject so = new SerializedObject(target);
            SerializedProperty property = so.FindProperty(propertyName);
            if (property != null) property.objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
