using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.Abilities;
using RenkaiMobile.Bots;
using RenkaiMobile.Combat;
using RenkaiMobile.Objective;
using RenkaiMobile.UI;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiCombatPolishWizard
    {
        [MenuItem("Renkai Mobile/Add Combat Polish & Mobile Abilities")]
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

            var ability = player.GetComponent<AbilityRuntimeController>() ?? player.AddComponent<AbilityRuntimeController>();
            if (player.GetComponent<AbilityVfxPulse>() == null) player.AddComponent<AbilityVfxPulse>();
            if (player.GetComponent<WeaponInventoryController>() == null) player.AddComponent<WeaponInventoryController>();
            if (player.GetComponent<DeterministicRecoilPattern>() == null) player.AddComponent<DeterministicRecoilPattern>();
            if (player.GetComponent<PlantAnimationBridge>() == null) player.AddComponent<PlantAnimationBridge>();

            AddCoverBots();
            BuildBuyButtons();
            BuildAbilityButtons(ability);

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            EditorUtility.DisplayDialog("Renkai Mobile", "Combat polish eklendi: buy buttons, weapon inventory, recoil pattern, cover bots, plant bridge, ability VFX ve mobile ability buttons.", "OK");
        }

        private static void AddCoverBots()
        {
            var teams = Object.FindObjectsByType<Renkai.Core.TeamMember>(FindObjectsSortMode.None);
            foreach (var member in teams)
            {
                if (member == null || member.gameObject.name == "Player") continue;
                if (member.GetComponent<CoverSeekingBot>() == null)
                    member.gameObject.AddComponent<CoverSeekingBot>();
            }
        }

        private static void BuildBuyButtons()
        {
            BuyMenuController menu = Object.FindFirstObjectByType<BuyMenuController>();
            if (menu == null) return;

            Transform panel = menu.transform.Find("BuyMenuPanel");
            if (panel == null)
            {
                GameObject panelGo = GameObject.Find("BuyMenuPanel");
                if (panelGo != null) panel = panelGo.transform;
            }
            if (panel == null) return;

            RemoveIfExists(panel, "Buy_Rifle");
            RemoveIfExists(panel, "Buy_Shield");
            RemoveIfExists(panel, "Buy_Ability");

            CreateBuyButton(panel, "Buy_Rifle", "KITSUNE AR\n2900 CR", new Vector2(-260f, -30f), menu, BuyMenuAction.Rifle);
            CreateBuyButton(panel, "Buy_Shield", "SHIELD\n1000 CR", new Vector2(0f, -30f), menu, BuyMenuAction.Shield);
            CreateBuyButton(panel, "Buy_Ability", "ABILITY CHARGE\n400 CR", new Vector2(260f, -30f), menu, BuyMenuAction.AbilityCharge);
        }

        private static void BuildAbilityButtons(AbilityRuntimeController controller)
        {
            GameObject hud = GameObject.Find("MobileHUD");
            if (hud == null) return;

            Transform root = hud.transform;
            RemoveIfExists(root, "Ability_Q");
            RemoveIfExists(root, "Ability_E");
            RemoveIfExists(root, "Ability_X");

            CreateAbilityButton(root, "Ability_Q", "Q", new Vector2(-340f, 170f), controller, 0);
            CreateAbilityButton(root, "Ability_E", "E", new Vector2(-230f, 245f), controller, 1);
            CreateAbilityButton(root, "Ability_X", "X", new Vector2(-110f, 300f), controller, 2);
        }

        private static void CreateBuyButton(Transform parent, string name, string label, Vector2 pos, BuyMenuController menu, BuyMenuAction action)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(BuyMenuActionButton));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(220f, 120f);
            go.GetComponent<Image>().color = new Color(0.08f, 0.14f, 0.25f, 0.95f);
            go.GetComponent<BuyMenuActionButton>().Configure(menu, action);

            GameObject textGo = new GameObject("Label", typeof(RectTransform), typeof(Text));
            textGo.transform.SetParent(go.transform, false);
            Text text = textGo.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 22;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.text = label;
            RectTransform tr = text.rectTransform;
            tr.anchorMin = Vector2.zero;
            tr.anchorMax = Vector2.one;
            tr.offsetMin = tr.offsetMax = Vector2.zero;
        }

        private static void CreateAbilityButton(Transform parent, string name, string label, Vector2 pos, AbilityRuntimeController controller, int slot)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(MobileAbilityButton));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(1f, 0f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(92f, 92f);
            go.GetComponent<Image>().color = new Color(0.12f, 0.12f, 0.24f, 0.85f);
            go.GetComponent<MobileAbilityButton>().Configure(controller, slot);

            GameObject textGo = new GameObject("Label", typeof(RectTransform), typeof(Text));
            textGo.transform.SetParent(go.transform, false);
            Text text = textGo.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 28;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.text = label;
            RectTransform tr = text.rectTransform;
            tr.anchorMin = Vector2.zero;
            tr.anchorMax = Vector2.one;
            tr.offsetMin = tr.offsetMax = Vector2.zero;
        }

        private static void RemoveIfExists(Transform parent, string name)
        {
            Transform old = parent.Find(name);
            if (old != null) Object.DestroyImmediate(old.gameObject);
        }
    }
}
