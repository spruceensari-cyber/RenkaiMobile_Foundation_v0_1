using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Renkai.Combat;
using Renkai.Input;
using Renkai.Movement;
using RenkaiMobile.Combat;
using RenkaiMobile.Input;
using RenkaiMobile.UI;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiMobileHudWizard
    {
        [MenuItem("Renkai Mobile/Add Mobile Combat HUD To Current Scene")]
        public static void BuildHud()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Play Mode'u kapat ve HUD kurulumunu tekrar çalıştır.", "OK");
                return;
            }

            var player = GameObject.Find("Player");
            if (player == null)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Player bulunamadı. Önce Build First Playable Scene çalıştır.", "OK");
                return;
            }

            var motor = player.GetComponent<MobileFpsMotor>();
            var pitchRoot = player.transform.Find("PitchRoot");
            var camera = pitchRoot != null ? pitchRoot.GetComponentInChildren<Camera>() : Camera.main;
            if (motor == null || pitchRoot == null || camera == null)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Player kamera veya motor yapısı eksik.", "OK");
                return;
            }

            RemoveOldHud();
            EnsureEventSystem();

            var weapon = CreateWeapon(player, camera, motor);
            var ads = player.GetComponent<AdsController>() ?? player.AddComponent<AdsController>();
            SetObjectReference(ads, "gameplayCamera", camera);

            var router = player.GetComponent<MobileInputRouter>() ?? player.AddComponent<MobileInputRouter>();
            if (player.GetComponent<EditorFpsDebugInput>() == null)
                player.AddComponent<EditorFpsDebugInput>();
            if (weapon.GetComponent<WeaponFeedbackController>() == null)
                weapon.gameObject.AddComponent<WeaponFeedbackController>();

            var canvasGo = new GameObject("MobileHUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            var joystick = CreateJoystick(canvasGo.transform);
            var lookArea = CreateLookArea(canvasGo.transform);
            CreateActionButton(canvasGo.transform, "FIRE", new Vector2(-120f, 220f), new Vector2(220f, 220f), MobileActionButton.ActionType.Fire, router, ads);
            CreateActionButton(canvasGo.transform, "ADS", new Vector2(-350f, 280f), new Vector2(150f, 150f), MobileActionButton.ActionType.Ads, router, ads);
            CreateActionButton(canvasGo.transform, "JUMP", new Vector2(-330f, 90f), new Vector2(140f, 140f), MobileActionButton.ActionType.Jump, router, ads);
            CreateActionButton(canvasGo.transform, "CROUCH", new Vector2(-510f, 100f), new Vector2(140f, 140f), MobileActionButton.ActionType.Crouch, router, ads);
            CreateActionButton(canvasGo.transform, "RELOAD", new Vector2(-515f, 270f), new Vector2(130f, 130f), MobileActionButton.ActionType.Reload, router, ads);
            CreateCrosshair(canvasGo.transform);

            var so = new SerializedObject(router);
            so.FindProperty("motor").objectReferenceValue = motor;
            so.FindProperty("movementJoystick").objectReferenceValue = joystick;
            so.FindProperty("lookArea").objectReferenceValue = lookArea;
            so.FindProperty("yawRoot").objectReferenceValue = player.transform;
            so.FindProperty("pitchRoot").objectReferenceValue = pitchRoot;
            so.FindProperty("weapon").objectReferenceValue = weapon;
            so.ApplyModifiedPropertiesWithoutUndo();

            var debug = player.GetComponent<EditorFpsDebugInput>();
            var debugSo = new SerializedObject(debug);
            debugSo.FindProperty("motor").objectReferenceValue = motor;
            debugSo.FindProperty("yawRoot").objectReferenceValue = player.transform;
            debugSo.FindProperty("pitchRoot").objectReferenceValue = pitchRoot;
            debugSo.FindProperty("weapon").objectReferenceValue = weapon;
            debugSo.FindProperty("ads").objectReferenceValue = ads;
            debugSo.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            Selection.activeGameObject = canvasGo;
            EditorUtility.DisplayDialog("Renkai Mobile", "Mobil HUD, Editor test kontrolleri ve combat feedback sahneye eklendi.", "OK");
        }

        private static HitscanWeapon CreateWeapon(GameObject player, Camera camera, MobileFpsMotor motor)
        {
            var old = player.GetComponentInChildren<HitscanWeapon>();
            if (old != null) return old;

            var weaponGo = new GameObject("StarterRifle_Runtime");
            weaponGo.transform.SetParent(camera.transform, false);
            weaponGo.transform.localPosition = new Vector3(0.22f, -0.22f, 0.55f);
            var weapon = weaponGo.AddComponent<HitscanWeapon>();

            const string dataRoot = "Assets/Renkai/Data";
            const string weaponFolder = "Assets/Renkai/Data/Weapons";
            EnsureFolder("Assets/Renkai", "Data");
            EnsureFolder(dataRoot, "Weapons");
            const string assetPath = weaponFolder + "/Weapon_StarterRifle.asset";
            var definition = AssetDatabase.LoadAssetAtPath<Renkai.Combat.WeaponDefinition>(assetPath);
            if (definition == null)
            {
                definition = ScriptableObject.CreateInstance<Renkai.Combat.WeaponDefinition>();
                definition.weaponId = "renkai_starter_rifle";
                definition.displayName = "Kitsune AR";
                definition.bodyDamage = 34f;
                definition.headDamage = 102f;
                definition.fireInterval = 0.1f;
                definition.reloadSeconds = 2.2f;
                definition.magazineSize = 25;
                definition.hipSpreadDegrees = 0.7f;
                definition.movingSpreadBonus = 1.8f;
                definition.crouchSpreadMultiplier = 0.72f;
                AssetDatabase.CreateAsset(definition, assetPath);
            }

            var so = new SerializedObject(weapon);
            so.FindProperty("definition").objectReferenceValue = definition;
            so.FindProperty("aimCamera").objectReferenceValue = camera;
            so.FindProperty("motor").objectReferenceValue = motor;
            so.ApplyModifiedPropertiesWithoutUndo();
            return weapon;
        }

        private static VirtualJoystick CreateJoystick(Transform parent)
        {
            var bg = CreateImage("MoveJoystick", parent, new Vector2(130f, 135f), new Vector2(280f, 280f), new Color(0.08f, 0.2f, 0.35f, 0.42f));
            SetAnchor(bg.rectTransform, new Vector2(0f, 0f), new Vector2(0f, 0f));
            var knob = CreateImage("Knob", bg.transform, Vector2.zero, new Vector2(120f, 120f), new Color(0.2f, 0.75f, 1f, 0.78f));
            var joystick = bg.gameObject.AddComponent<VirtualJoystick>();
            var so = new SerializedObject(joystick);
            so.FindProperty("background").objectReferenceValue = bg.rectTransform;
            so.FindProperty("knob").objectReferenceValue = knob.rectTransform;
            so.ApplyModifiedPropertiesWithoutUndo();
            return joystick;
        }

        private static TouchLookArea CreateLookArea(Transform parent)
        {
            var image = CreateImage("TouchLookArea", parent, Vector2.zero, Vector2.zero, new Color(0f, 0f, 0f, 0.001f));
            var rt = image.rectTransform;
            rt.anchorMin = new Vector2(0.38f, 0f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            return image.gameObject.AddComponent<TouchLookArea>();
        }

        private static void CreateActionButton(Transform parent, string label, Vector2 pos, Vector2 size, MobileActionButton.ActionType action, MobileInputRouter router, AdsController ads)
        {
            var image = CreateImage(label + "_Button", parent, pos, size, new Color(0.35f, 0.12f, 0.48f, 0.68f));
            SetAnchor(image.rectTransform, Vector2.one, Vector2.one);
            var bridge = image.gameObject.AddComponent<MobileActionButton>();
            bridge.Configure(router, ads, action);

            var textGo = new GameObject("Label", typeof(RectTransform), typeof(Text));
            textGo.transform.SetParent(image.transform, false);
            var text = textGo.GetComponent<Text>();
            text.text = label;
            text.alignment = TextAnchor.MiddleCenter;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 25;
            text.color = Color.white;
            var rt = text.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        private static void CreateCrosshair(Transform parent)
        {
            var root = new GameObject("Crosshair", typeof(RectTransform), typeof(CrosshairController));
            root.transform.SetParent(parent, false);
            var rootRt = root.GetComponent<RectTransform>();
            rootRt.anchorMin = rootRt.anchorMax = new Vector2(0.5f, 0.5f);
            rootRt.sizeDelta = new Vector2(100f, 100f);

            var arms = new RectTransform[4];
            for (int i = 0; i < arms.Length; i++)
            {
                var arm = CreateImage("Arm_" + i, root.transform, Vector2.zero, i < 2 ? new Vector2(14f, 3f) : new Vector2(3f, 14f), Color.white);
                arms[i] = arm.rectTransform;
            }

            var so = new SerializedObject(root.GetComponent<CrosshairController>());
            var prop = so.FindProperty("arms");
            prop.arraySize = 4;
            for (int i = 0; i < 4; i++) prop.GetArrayElementAtIndex(i).objectReferenceValue = arms[i];
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Image CreateImage(string name, Transform parent, Vector2 anchoredPosition, Vector2 size, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = true;
            var rt = image.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = size;
            return image;
        }

        private static void SetAnchor(RectTransform rt, Vector2 min, Vector2 max)
        {
            rt.anchorMin = min;
            rt.anchorMax = max;
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null) return;
            var go = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            go.transform.SetAsLastSibling();
        }

        private static void RemoveOldHud()
        {
            var old = GameObject.Find("MobileHUD");
            if (old != null) Object.DestroyImmediate(old);
        }

        private static void SetObjectReference(Object target, string propertyName, Object value)
        {
            var so = new SerializedObject(target);
            so.FindProperty(propertyName).objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void EnsureFolder(string parent, string child)
        {
            var full = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(full)) AssetDatabase.CreateFolder(parent, child);
        }
    }
}
