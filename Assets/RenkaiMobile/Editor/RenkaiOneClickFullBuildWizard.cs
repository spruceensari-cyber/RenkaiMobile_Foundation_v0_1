using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiOneClickFullBuildWizard
    {
        private const string MenuPath = "Renkai Mobile/00 BUILD COMPLETE GAME - ONE CLICK";
        private static readonly List<string> MissingTypes = new List<string>();
        private static readonly List<string> BuildNotes = new List<string>();

        [MenuItem(MenuPath, false, 1)]
        public static void BuildCompleteGame()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Play Mode'u kapat ve FULL BUILD komutunu tekrar çalıştır.", "OK");
                return;
            }

            MissingTypes.Clear();
            BuildNotes.Clear();

            try
            {
                Step(0.03f, "Preparing scene");
                EnsureCameraAndLighting();

                Step(0.10f, "Building match core");
                GameObject matchRoot = EnsureObject("Renkai_5v5_Match");
                Component roundManager = EnsureComponent(matchRoot, "RenkaiMobile.Rounds.MobileRoundManager");
                Component roundDirector = EnsureComponent(matchRoot, "RenkaiMobile.Teams.FiveVFiveRoundDirector");
                Bind(roundDirector, "roundManager", roundManager);

                Step(0.18f, "Building player and teams");
                GameObject player = BuildPlayer(roundManager);
                BuildBots();

                Step(0.30f, "Building Zodiac objective");
                Component zodiacObjective = BuildZodiac(roundManager);

                Step(0.40f, "Building Kagami District");
                BuildKagamiDistrict();

                Step(0.54f, "Installing game services");
                GameObject services = BuildServices();

                Step(0.64f, "Installing weapons and abilities");
                BuildPlayerCombat(player, zodiacObjective, services);

                Step(0.76f, "Building mobile HUD and controls");
                BuildHud(roundManager, zodiacObjective);

                Step(0.86f, "Building presentation shells");
                BuildPresentationShells();

                Step(0.93f, "Refreshing registries and identities");
                ConfigureRosterIdentities();
                InvokeNoArgs(FindSceneComponent("RenkaiMobile.Spawning.MobileSpawnRegistry"), "Refresh");
                InvokeNoArgs(FindSceneComponent("RenkaiMobile.Bots.CoverRegistry"), "Refresh");
                InvokeNoArgs(FindSceneComponent("RenkaiMobile.Map.KagamiRouteGraph"), "RebuildLookup");

                Step(0.98f, "Finalizing scene");
                Scene scene = SceneManager.GetActiveScene();
                EditorSceneManager.MarkSceneDirty(scene);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                if (!string.IsNullOrEmpty(scene.path))
                    EditorSceneManager.SaveScene(scene);
                else
                    BuildNotes.Add("Scene is Untitled: build completed without opening a Save As window. Save the scene manually later when you choose its location.");

                Debug.Log(BuildReport());
                EditorUtility.DisplayDialog("RENKAI MOBILE FULL BUILD", BuildReport(), "OK");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                EditorUtility.DisplayDialog("RENKAI MOBILE FULL BUILD", "Build stopped because of an exception.\n\n" + ex.Message + "\n\nCheck Console for details.", "OK");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static void Step(float progress, string label)
        {
            EditorUtility.DisplayProgressBar("Renkai Mobile Full Build", label, progress);
        }

        private static void EnsureCameraAndLighting()
        {
            GameObject cameraGo = GameObject.Find("Main Camera");
            if (cameraGo == null)
            {
                cameraGo = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
                cameraGo.tag = "MainCamera";
            }

            Camera camera = cameraGo.GetComponent<Camera>();
            if (camera == null) camera = cameraGo.AddComponent<Camera>();
            camera.fieldOfView = 72f;
            camera.nearClipPlane = 0.03f;
            camera.farClipPlane = 500f;

            GameObject lightGo = GameObject.Find("Directional Light");
            if (lightGo == null) lightGo = new GameObject("Directional Light", typeof(Light));
            Light light = lightGo.GetComponent<Light>();
            if (light == null) light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.15f;
            light.color = new Color(0.72f, 0.82f, 1f);
            lightGo.transform.rotation = Quaternion.Euler(42f, -32f, 0f);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.09f, 0.12f, 0.2f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.035f, 0.055f, 0.09f);
            RenderSettings.fogDensity = 0.007f;
        }

        private static GameObject BuildPlayer(Component roundManager)
        {
            GameObject player = GameObject.Find("Player");
            if (player == null)
            {
                player = new GameObject("Player");
                player.transform.position = new Vector3(0f, 1.1f, -20f);
            }

            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller == null) controller = player.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.34f;
            controller.center = new Vector3(0f, 0.9f, 0f);

            EnsureComponent(player, "RenkaiMobile.Movement.MobilePlayerMotor");
            EnsureComponent(player, "RenkaiMobile.Movement.MobileLookController");
            Component team = EnsureComponent(player, "RenkaiMobile.Core.MobileTeamMember");
            SetTeam(team, "Attackers");
            EnsureComponent(player, "RenkaiMobile.Combat.MobileHealth");
            Component roster = EnsureComponent(player, "RenkaiMobile.Teams.FiveVFiveRosterAgent");
            ConfigureRoster(roster, 0, true);
            EnsureComponent(player, "RenkaiMobile.Combat.MobileCombatantIdentity");

            Transform cameraRoot = EnsureChild(player.transform, "PlayerCameraRoot");
            cameraRoot.localPosition = new Vector3(0f, 1.62f, 0f);
            GameObject mainCamera = GameObject.Find("Main Camera");
            mainCamera.transform.SetParent(cameraRoot, false);
            mainCamera.transform.localPosition = Vector3.zero;
            mainCamera.transform.localRotation = Quaternion.identity;

            return player;
        }

        private static void BuildBots()
        {
            GameObject matchRoot = EnsureObject("Renkai_5v5_Match");
            Transform attackersRoot = EnsureChild(matchRoot.transform, "Attackers_Team");
            Transform defendersRoot = EnsureChild(matchRoot.transform, "Defenders_Team");

            for (int i = 1; i <= 4; i++)
            {
                Vector3 position = new Vector3(-4.5f + i * 1.8f, 1f, -20f + (i % 2) * 1.2f);
                BuildBot("Attackers_Bot_" + i, attackersRoot, "Attackers", i, position, Quaternion.identity);
            }

            for (int i = 0; i < 5; i++)
            {
                Vector3 position = new Vector3(-4f + i * 2f, 1f, 39f);
                BuildBot("Defenders_Bot_" + (i + 1), defendersRoot, "Defenders", i, position, Quaternion.Euler(0f, 180f, 0f));
            }
        }

        private static void BuildBot(string name, Transform parent, string teamName, int slot, Vector3 position, Quaternion rotation)
        {
            GameObject bot = GameObject.Find(name);
            if (bot == null) bot = new GameObject(name);
            bot.transform.SetParent(parent, true);
            bot.transform.SetPositionAndRotation(position, rotation);

            Component team = EnsureComponent(bot, "RenkaiMobile.Core.MobileTeamMember");
            SetTeam(team, teamName);
            EnsureComponent(bot, "RenkaiMobile.Combat.MobileHealth");
            Component roster = EnsureComponent(bot, "RenkaiMobile.Teams.FiveVFiveRosterAgent");
            ConfigureRoster(roster, slot, false);
            EnsureComponent(bot, "RenkaiMobile.Combat.MobileCombatantIdentity");

            EnsureComponent(bot, "RenkaiMobile.Bots.BotNavigationAgent");
            EnsureComponent(bot, "RenkaiMobile.Bots.BotNavigationIntentController");
            EnsureComponent(bot, "RenkaiMobile.Bots.BotPerceptionMemory");
            EnsureComponent(bot, "RenkaiMobile.Bots.BotPerception");
            EnsureComponent(bot, "RenkaiMobile.Bots.BotInvestigateSoundState");
            EnsureComponent(bot, "RenkaiMobile.Bots.KagamiTacticalRouteBrain");
            EnsureComponent(bot, "RenkaiMobile.Bots.SiteRotationBot");
            EnsureComponent(bot, "RenkaiMobile.Bots.CoverSeekingBot");
            EnsureComponent(bot, "RenkaiMobile.Bots.TacticalBotBrain");
            EnsureComponent(bot, "RenkaiMobile.Audio.ProceduralFootsteps");
            if (bot.GetComponent<AudioSource>() == null) bot.AddComponent<AudioSource>();

            CapsuleCollider capsule = bot.GetComponent<CapsuleCollider>();
            if (capsule == null) capsule = bot.AddComponent<CapsuleCollider>();
            capsule.center = new Vector3(0f, 0.9f, 0f);
            capsule.height = 1.8f;
            capsule.radius = 0.35f;

            Transform visual = bot.transform.Find("Visual");
            if (visual == null)
            {
                GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                body.name = "Visual";
                body.transform.SetParent(bot.transform, false);
                body.transform.localPosition = new Vector3(0f, 0.9f, 0f);
                body.transform.localScale = new Vector3(0.65f, 0.9f, 0.65f);
                UnityEngine.Object.DestroyImmediate(body.GetComponent<Collider>());
                ApplyMaterial(body.GetComponent<Renderer>(), teamName == "Attackers" ? EnsureMaterial("Team_Cyan", new Color(0.04f, 0.55f, 0.9f)) : EnsureMaterial("Team_Magenta", new Color(0.75f, 0.08f, 0.45f)));
            }
        }

        private static Component BuildZodiac(Component roundManager)
        {
            GameObject objectiveRoot = EnsureObject("Zodiac_Objective");
            Component objective = EnsureComponent(objectiveRoot, "RenkaiMobile.Objective.ZodiacObjective");
            Bind(objective, "roundManager", roundManager);

            CreateZodiacZone("Zodiac_Zone_A", "A", new Vector3(-12f, 0.08f, 18f));
            CreateZodiacZone("Zodiac_Zone_B", "B", new Vector3(12f, 0.08f, 22f));
            return objective;
        }

        private static void CreateZodiacZone(string name, string id, Vector3 position)
        {
            GameObject zone = GameObject.Find(name);
            if (zone == null)
            {
                zone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                zone.name = name;
            }
            zone.transform.position = position;
            zone.transform.localScale = new Vector3(4.5f, 0.05f, 4.5f);
            Collider collider = zone.GetComponent<Collider>();
            if (collider != null) collider.isTrigger = true;
            Component component = EnsureComponent(zone, "RenkaiMobile.Objective.ZodiacZone");
            Invoke(component, "Configure", id);
            ApplyMaterial(zone.GetComponent<Renderer>(), EnsureMaterial(id == "A" ? "Zone_A_Cyan" : "Zone_B_Purple", id == "A" ? new Color(0.02f, 0.65f, 1f) : new Color(0.5f, 0.08f, 0.9f)));
        }

        private static void BuildKagamiDistrict()
        {
            GameObject old = GameObject.Find("Kagami_CompleteMap");
            if (old != null) UnityEngine.Object.DestroyImmediate(old);

            GameObject root = new GameObject("Kagami_CompleteMap");
            Material dark = EnsureMaterial("Kagami_Dark", new Color(0.025f, 0.04f, 0.08f));
            Material steel = EnsureMaterial("Kagami_Steel", new Color(0.11f, 0.16f, 0.24f));
            Material cyan = EnsureMaterial("Kagami_Cyan", new Color(0.02f, 0.7f, 1f));
            Material purple = EnsureMaterial("Kagami_Purple", new Color(0.55f, 0.08f, 0.95f));

            CreateBlock(root.transform, "Ground", new Vector3(0f, -0.25f, 10f), new Vector3(46f, 0.5f, 70f), dark);
            CreateBlock(root.transform, "MidLane", new Vector3(0f, 0f, 8f), new Vector3(8f, 0.25f, 38f), steel);
            CreateBlock(root.transform, "LeftLane", new Vector3(-13f, 0f, 10f), new Vector3(7f, 0.25f, 42f), steel);
            CreateBlock(root.transform, "RightLane", new Vector3(13f, 0f, 12f), new Vector3(7f, 0.25f, 44f), steel);
            CreateBlock(root.transform, "DefenderConnector", new Vector3(0f, 0f, 31f), new Vector3(28f, 0.25f, 5f), steel);

            CreateBlock(root.transform, "CyberShrineGate", new Vector3(0f, 3.2f, 7f), new Vector3(10f, 0.8f, 1f), purple);
            CreateBlock(root.transform, "ShrinePillarL", new Vector3(-4.5f, 2f, 7f), new Vector3(0.8f, 4f, 0.8f), purple);
            CreateBlock(root.transform, "ShrinePillarR", new Vector3(4.5f, 2f, 7f), new Vector3(0.8f, 4f, 0.8f), cyan);
            CreateBlock(root.transform, "ElevatedTransit", new Vector3(0f, 7f, 27f), new Vector3(34f, 0.7f, 2.2f), cyan);

            for (int i = 0; i < 10; i++)
            {
                float side = i % 2 == 0 ? -1f : 1f;
                float z = -10f + i * 6f;
                float height = 12f + (i % 4) * 4f;
                CreateBlock(root.transform, "MegaTower_" + i, new Vector3(side * (23f + (i % 3) * 3f), height * 0.5f, z), new Vector3(5f, height, 5f), i % 3 == 0 ? purple : dark);
            }

            for (int i = 0; i < 14; i++)
            {
                Vector3 p = new Vector3((i % 2 == 0 ? -1f : 1f) * (5f + (i % 3) * 4f), 0.75f, -6f + i * 3.2f);
                GameObject cover = CreateBlock(root.transform, "Cover_" + i, p, new Vector3(2.4f, 1.5f, 1.2f), steel);
                GameObject point = new GameObject("CoverPoint_" + i);
                point.transform.SetParent(root.transform, false);
                point.transform.position = p + Vector3.back * 1.4f;
                EnsureComponent(point, "RenkaiMobile.Bots.CoverPoint");
            }

            BuildRouteGraph(root.transform);
            BuildSpawnPoints(root.transform);
            EnsureComponent(root, "RenkaiMobile.Bots.CoverRegistry");
            EnsureComponent(root, "RenkaiMobile.Map.KagamiAtmosphereController");
        }

        private static void BuildRouteGraph(Transform parent)
        {
            string[] names = { "AttackerSpawn", "AMain", "AShort", "AZodiacZone", "Mid", "MidConnector", "BMain", "BShort", "BZodiacZone", "DefenderConnector", "DefenderSpawn" };
            Vector3[] positions =
            {
                new Vector3(0f,0.1f,-20f), new Vector3(-13f,0.1f,7f), new Vector3(-7f,0.1f,14f), new Vector3(-12f,0.1f,18f),
                new Vector3(0f,0.1f,8f), new Vector3(0f,0.1f,24f), new Vector3(13f,0.1f,10f), new Vector3(7f,0.1f,17f),
                new Vector3(12f,0.1f,22f), new Vector3(0f,0.1f,31f), new Vector3(0f,0.1f,39f)
            };

            GameObject graphGo = new GameObject("KagamiRouteGraph");
            graphGo.transform.SetParent(parent, false);
            Component graph = EnsureComponent(graphGo, "RenkaiMobile.Map.KagamiRouteGraph");

            SerializedObject so = graph != null ? new SerializedObject(graph) : null;
            SerializedProperty nodes = so?.FindProperty("nodes");
            if (nodes == null) return;
            nodes.arraySize = names.Length;
            for (int i = 0; i < names.Length; i++)
            {
                GameObject anchor = new GameObject(names[i]);
                anchor.transform.SetParent(graphGo.transform, false);
                anchor.transform.position = positions[i];

                SerializedProperty element = nodes.GetArrayElementAtIndex(i);
                SerializedProperty id = element.FindPropertyRelative("id");
                SerializedProperty anchorProp = element.FindPropertyRelative("anchor");
                if (id != null) id.enumValueIndex = i;
                if (anchorProp != null) anchorProp.objectReferenceValue = anchor.transform;
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildSpawnPoints(Transform parent)
        {
            Transform root = EnsureChild(parent, "SpawnPoints");
            for (int i = 0; i < 5; i++)
            {
                BuildSpawnPoint(root, "Attackers_Spawn_" + i, "Attackers", i, new Vector3(-4f + i * 2f, 0.1f, -20f), Quaternion.identity);
                BuildSpawnPoint(root, "Defenders_Spawn_" + i, "Defenders", i, new Vector3(-4f + i * 2f, 0.1f, 39f), Quaternion.Euler(0f, 180f, 0f));
            }
        }

        private static void BuildSpawnPoint(Transform parent, string name, string teamName, int slot, Vector3 position, Quaternion rotation)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.SetPositionAndRotation(position, rotation);
            Component component = EnsureComponent(go, "RenkaiMobile.Spawning.MobileSpawnPoint");
            if (component == null) return;
            Type enumType = FindType("RenkaiMobile.Core.MobileTeamId");
            if (enumType == null) return;
            object enumValue = Enum.Parse(enumType, teamName);
            MethodInfo method = component.GetType().GetMethod("Configure", BindingFlags.Instance | BindingFlags.Public);
            method?.Invoke(component, new[] { enumValue, (object)slot });
        }

        private static GameObject BuildServices()
        {
            GameObject services = EnsureObject("RenkaiMobile_Services");
            string[] types =
            {
                "RenkaiMobile.Combat.MobileDamagePolicy",
                "RenkaiMobile.Combat.MobileHitFeedbackBus",
                "RenkaiMobile.Combat.MobileHitFeedbackRelay",
                "RenkaiMobile.Events.MobileKillEventBus",
                "RenkaiMobile.Events.MobileKillEventRelay",
                "RenkaiMobile.Events.CompetitiveMomentTracker",
                "RenkaiMobile.Events.CompetitiveAnnouncementQueue",
                "RenkaiMobile.Events.CompetitiveAnnouncerBridge",
                "RenkaiMobile.Events.HighlightMomentTracker",
                "RenkaiMobile.Audio.MobileSoundEventBus",
                "RenkaiMobile.VFX.MobileVfxPool",
                "RenkaiMobile.Stats.MatchStatsTracker",
                "RenkaiMobile.Stats.MvpScoreCalculator",
                "RenkaiMobile.Rounds.MobileMatchStateController",
                "RenkaiMobile.UI.MobileTeamVisionService",
                "RenkaiMobile.Spawning.MobileSpawnRegistry",
                "RenkaiMobile.Spawning.MobileTeamSpawnController"
            };

            foreach (string type in types) EnsureComponent(services, type);
            return services;
        }

        private static void BuildPlayerCombat(GameObject player, Component zodiacObjective, GameObject services)
        {
            Component ads = EnsureComponent(player, "RenkaiMobile.Combat.MobileAdsController");
            Component shotSignal = EnsureComponent(player, "RenkaiMobile.Combat.MobileWeaponShotSignal");
            Component recoilPattern = EnsureComponent(player, "RenkaiMobile.Combat.DeterministicRecoilPattern");
            Component cameraStack = EnsureComponent(player, "RenkaiMobile.Combat.MobileCameraMotionStack");
            Component cameraImpulse = EnsureComponent(player, "RenkaiMobile.Combat.MobileCameraImpulseAdapter");
            Component recoilDriver = EnsureComponent(player, "RenkaiMobile.Combat.MobileRecoilDriver");
            EnsureComponent(player, "RenkaiMobile.Combat.MobileRecoilShotLink");
            Component inventory = EnsureComponent(player, "RenkaiMobile.Combat.WeaponInventoryController");

            Component abilityRuntime = EnsureComponent(player, "RenkaiMobile.Abilities.AbilityRuntimeController");
            Component raika = EnsureComponent(player, "RenkaiMobile.Abilities.RaikaAbilityKit");
            Component dispatcher = EnsureComponent(player, "RenkaiMobile.Abilities.AgentAbilityDispatcher");
            Component carrier = EnsureComponent(player, "RenkaiMobile.Objective.ZodiacCarrier");
            Bind(carrier, "objective", zodiacObjective);
            EnsureComponent(player, "RenkaiMobile.Audio.ProceduralFootsteps");
            if (player.GetComponent<AudioSource>() == null) player.AddComponent<AudioSource>();

            Camera aimCamera = player.GetComponentInChildren<Camera>(true);
            Transform weaponRoot = EnsureChild(aimCamera != null ? aimCamera.transform : player.transform, "MobileWeaponRoot");
            weaponRoot.localPosition = new Vector3(0.2f, -0.2f, 0.45f);

            GameObject primary = EnsureWeaponVisual(weaponRoot, "Kitsune_AR", new Vector3(0.12f, -0.08f, 0.28f), new Vector3(0.12f, 0.1f, 0.55f), EnsureMaterial("Weapon_Cyan", new Color(0.03f, 0.7f, 1f)));
            GameObject secondary = EnsureWeaponVisual(weaponRoot, "Zero_Pulse", new Vector3(0.1f, -0.1f, 0.22f), new Vector3(0.1f, 0.08f, 0.25f), EnsureMaterial("Weapon_Magenta", new Color(0.8f, 0.05f, 0.45f)));
            GameObject melee = EnsureWeaponVisual(weaponRoot, "Hikari_Blade", new Vector3(0.16f, -0.08f, 0.25f), new Vector3(0.04f, 0.04f, 0.65f), EnsureMaterial("Weapon_Purple", new Color(0.55f, 0.08f, 1f)));

            Component rifle = EnsureComponent(primary, "RenkaiMobile.Weapons.MobileRifleController");
            Component pistol = EnsureComponent(secondary, "RenkaiMobile.Weapons.MobilePistolController");
            Component blade = EnsureComponent(melee, "RenkaiMobile.Weapons.MobileMeleeController");
            EnsureComponent(primary, "RenkaiMobile.VFX.MobileWeaponVfxController");
            EnsureComponent(primary, "RenkaiMobile.VFX.HolographicWeaponSight");
            EnsureComponent(melee, "RenkaiMobile.VFX.HikariBladeVfxController");

            ScriptableObject rifleProfile = EnsureScriptable("RenkaiMobile.Weapons.MobileWeaponProfile", "Assets/RenkaiMobile/Data/Weapons/WeaponProfile_Kitsune.asset");
            ScriptableObject pistolProfile = EnsureScriptable("RenkaiMobile.Weapons.MobilePistolProfile", "Assets/RenkaiMobile/Data/Weapons/PistolProfile_ZeroPulse.asset");
            ScriptableObject meleeProfile = EnsureScriptable("RenkaiMobile.Weapons.MobileMeleeProfile", "Assets/RenkaiMobile/Data/Weapons/MeleeProfile_HikariBlade.asset");

            Bind(rifle, "profile", rifleProfile);
            Bind(rifle, "aimCamera", aimCamera);
            Bind(rifle, "adsController", ads);
            Bind(rifle, "shotSignal", shotSignal);
            Bind(rifle, "damagePolicy", FindSceneComponent("RenkaiMobile.Combat.MobileDamagePolicy"));

            Bind(pistol, "profile", pistolProfile);
            Bind(pistol, "aimCamera", aimCamera);
            Bind(pistol, "adsController", ads);
            Bind(pistol, "shotSignal", shotSignal);
            Bind(pistol, "damagePolicy", FindSceneComponent("RenkaiMobile.Combat.MobileDamagePolicy"));

            Bind(blade, "profile", meleeProfile);
            Bind(blade, "damagePolicy", FindSceneComponent("RenkaiMobile.Combat.MobileDamagePolicy"));

            Bind(recoilDriver, "pattern", recoilPattern);
            Bind(recoilDriver, "ads", ads);
            Bind(recoilDriver, "cameraImpulse", cameraImpulse);

            Bind(inventory, "primaryView", primary);
            Bind(inventory, "secondaryView", secondary);
            Bind(inventory, "meleeView", melee);

            Component weaponRouter = EnsureComponent(player, "RenkaiMobile.Weapons.MobileWeaponRouter");
            Bind(weaponRouter, "inventory", inventory);
            Bind(weaponRouter, "rifle", rifle);
            Bind(weaponRouter, "pistol", pistol);
            Bind(weaponRouter, "melee", blade);
            Bind(weaponRouter, "ads", ads);
            Bind(weaponRouter, "recoilDriver", recoilDriver);

            Component reloadState = EnsureComponent(player, "RenkaiMobile.Weapons.MobileReloadState");
            Bind(reloadState, "weaponRouter", weaponRouter);
            Bind(reloadState, "rifle", rifle);
            Bind(reloadState, "pistol", pistol);
            Bind(reloadState, "adsController", ads);

            Bind(dispatcher, "raika", raika);
            Bind(raika, "runtime", abilityRuntime);

            Component inputRouter = EnsureComponent(player, "RenkaiMobile.Input.MobileCombatInputRouter");
            Bind(inputRouter, "weaponRouter", weaponRouter);
            Bind(inputRouter, "adsController", ads);
            Bind(inputRouter, "abilityDispatcher", dispatcher);
            Bind(inputRouter, "zodiacCarrier", carrier);
        }

        private static GameObject EnsureWeaponVisual(Transform parent, string name, Vector3 localPosition, Vector3 scale, Material material)
        {
            Transform existing = parent.Find(name);
            GameObject go;
            if (existing != null) go = existing.gameObject;
            else
            {
                go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.name = name;
                go.transform.SetParent(parent, false);
            }
            go.transform.localPosition = localPosition;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = scale;
            Collider collider = go.GetComponent<Collider>();
            if (collider != null) UnityEngine.Object.DestroyImmediate(collider);
            ApplyMaterial(go.GetComponent<Renderer>(), material);
            return go;
        }

        private static void BuildHud(Component roundManager, Component zodiacObjective)
        {
            GameObject old = GameObject.Find("RenkaiMobile_FullHUD");
            if (old != null) UnityEngine.Object.DestroyImmediate(old);

            GameObject canvasGo = new GameObject("RenkaiMobile_FullHUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            EnsureEventSystem();

            GameObject topPanel = CreateUiPanel(canvasGo.transform, "TopHolographicPanel", new Vector2(0.5f, 1f), new Vector2(0f, -18f), new Vector2(780f, 150f), new Color(0.02f, 0.04f, 0.09f, 0.72f));
            EnsureComponent(topPanel, "RenkaiMobile.UI.HolographicHudPanel");
            Text phase = CreateUiText(topPanel.transform, "Phase", "PHASE // BUY", new Vector2(-220f, 38f), new Vector2(300f, 45f), 24);
            Text timer = CreateUiText(topPanel.transform, "Timer", "00:20", new Vector2(0f, 35f), new Vector2(180f, 55f), 36);
            Text score = CreateUiText(topPanel.transform, "Score", "0 // 0", new Vector2(220f, 38f), new Vector2(260f, 45f), 28);
            Text objective = CreateUiText(topPanel.transform, "Objective", "ZODIAC // DORMANT", new Vector2(0f, -28f), new Vector2(600f, 40f), 22);

            Component matchHud = EnsureComponent(canvasGo, "RenkaiMobile.UI.MobileMatchHud");
            Bind(matchHud, "roundManager", roundManager);
            Bind(matchHud, "objective", zodiacObjective);
            Bind(matchHud, "phaseText", phase);
            Bind(matchHud, "timerText", timer);
            Bind(matchHud, "scoreText", score);
            Bind(matchHud, "objectiveText", objective);

            GameObject interaction = CreateUiPanel(canvasGo.transform, "ZodiacInteraction", new Vector2(0.5f, 0.5f), new Vector2(0f, -90f), new Vector2(280f, 160f), new Color(0.02f, 0.03f, 0.08f, 0.58f));
            CanvasGroup interactionGroup = interaction.AddComponent<CanvasGroup>();
            Text actionText = CreateUiText(interaction.transform, "ActionText", "", new Vector2(0f, 42f), new Vector2(260f, 40f), 20);
            GameObject ringGo = new GameObject("ProgressRing", typeof(RectTransform), typeof(Image));
            ringGo.transform.SetParent(interaction.transform, false);
            Image ring = ringGo.GetComponent<Image>();
            ring.type = Image.Type.Filled;
            ring.fillMethod = Image.FillMethod.Radial360;
            ring.fillAmount = 0f;
            ring.color = new Color(0.05f, 0.85f, 1f, 0.9f);
            ring.rectTransform.sizeDelta = new Vector2(86f, 86f);
            ring.rectTransform.anchoredPosition = new Vector2(0f, -25f);

            Component interactionPresenter = EnsureComponent(interaction, "RenkaiMobile.Objective.ZodiacInteractionPresenter");
            Bind(interactionPresenter, "objective", zodiacObjective);
            Bind(interactionPresenter, "progressRing", ring);
            Bind(interactionPresenter, "actionText", actionText);
            Bind(interactionPresenter, "group", interactionGroup);

            BuildCombatButtons(canvasGo.transform);
        }

        private static void BuildCombatButtons(Transform canvas)
        {
            Component router = FindSceneComponent("RenkaiMobile.Input.MobileCombatInputRouter");
            CreateActionButton(canvas, "FIRE", "FIRE", new Vector2(1f, 0f), new Vector2(-150f, 170f), new Vector2(150f, 150f), 0, router);
            CreateActionButton(canvas, "ADS", "ADS", new Vector2(1f, 0f), new Vector2(-320f, 230f), new Vector2(110f, 110f), 1, router);
            CreateActionButton(canvas, "RELOAD", "R", new Vector2(1f, 0f), new Vector2(-300f, 90f), new Vector2(95f, 95f), 2, router);
            CreateActionButton(canvas, "PRIMARY", "1", new Vector2(0.5f, 0f), new Vector2(-110f, 85f), new Vector2(78f, 78f), 3, router);
            CreateActionButton(canvas, "SECONDARY", "2", new Vector2(0.5f, 0f), new Vector2(0f, 85f), new Vector2(78f, 78f), 4, router);
            CreateActionButton(canvas, "MELEE", "3", new Vector2(0.5f, 0f), new Vector2(110f, 85f), new Vector2(78f, 78f), 5, router);
            CreateActionButton(canvas, "ABILITY_Q", "Q", new Vector2(1f, 0f), new Vector2(-480f, 110f), new Vector2(92f, 92f), 6, router);
            CreateActionButton(canvas, "ABILITY_E", "E", new Vector2(1f, 0f), new Vector2(-450f, 225f), new Vector2(92f, 92f), 7, router);
            CreateActionButton(canvas, "ABILITY_X", "X", new Vector2(1f, 0f), new Vector2(-390f, 335f), new Vector2(105f, 105f), 8, router);
            CreateActionButton(canvas, "INTERACT", "ZODIAC", new Vector2(0.5f, 0.5f), new Vector2(0f, -250f), new Vector2(160f, 72f), 9, router);

            GameObject movePad = CreateUiPanel(canvas, "MOVE_PAD", new Vector2(0f, 0f), new Vector2(170f, 170f), new Vector2(250f, 250f), new Color(0.04f, 0.18f, 0.28f, 0.28f));
            CreateUiText(movePad.transform, "Label", "MOVE", Vector2.zero, new Vector2(180f, 50f), 22);
        }

        private static void CreateActionButton(Transform parent, string name, string label, Vector2 anchor, Vector2 position, Vector2 size, int actionIndex, Component router)
        {
            GameObject go = CreateUiPanel(parent, name, anchor, position, size, new Color(0.04f, 0.16f, 0.28f, 0.78f));
            CreateUiText(go.transform, "Label", label, Vector2.zero, size, Mathf.RoundToInt(Mathf.Clamp(size.y * 0.24f, 18f, 32f)));
            Component actionButton = EnsureComponent(go, "RenkaiMobile.UI.MobileCombatActionButton");
            Bind(actionButton, "inputRouter", router);
            SetEnum(actionButton, "action", actionIndex);
        }

        private static void BuildPresentationShells()
        {
            GameObject root = EnsureObject("Renkai_Presentation");
            EnsureComponent(root, "RenkaiMobile.Presentation.MatchLobbyController");
            EnsureComponent(root, "RenkaiMobile.Presentation.AgentSelectionController");
            EnsureComponent(root, "RenkaiMobile.Presentation.MatchPresentationDirector");
            EnsureComponent(root, "RenkaiMobile.Presentation.MainMenuWorldSceneController");

            string[] panels = { "MainMenuWorld", "MatchLobby", "AgentSelection", "TeamLineup", "Armory", "Scoreboard", "DeathRecap", "SpectatorHUD", "RoundTransition", "MVP_EndMatch" };
            foreach (string panelName in panels)
            {
                Transform panel = EnsureChild(root.transform, panelName);
                panel.gameObject.SetActive(false);
            }
        }

        private static void ConfigureRosterIdentities()
        {
            Type rosterType = FindType("RenkaiMobile.Teams.FiveVFiveRosterAgent");
            if (rosterType == null) return;
            UnityEngine.Object[] objects = Resources.FindObjectsOfTypeAll(rosterType);
            foreach (UnityEngine.Object obj in objects)
            {
                Component roster = obj as Component;
                if (!IsSceneComponent(roster)) continue;
                Component identity = EnsureComponent(roster.gameObject, "RenkaiMobile.Combat.MobileCombatantIdentity");
                string team = GetPropertyString(roster, "Team");
                int slot = GetPropertyInt(roster, "SlotIndex");
                bool playerControlled = GetPropertyBool(roster, "PlayerControlled");
                string id = team.ToLowerInvariant() + "_" + slot;
                string displayName = playerControlled ? "ENSARI" : team.ToUpperInvariant() + " " + (slot + 1);
                string agentId = slot % 3 == 0 ? "raika" : slot % 3 == 1 ? "akari" : "kuroha";
                Invoke(identity, "Configure", id, displayName, agentId, playerControlled);
            }
        }

        private static GameObject CreateBlock(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.position = position;
            go.transform.localScale = scale;
            ApplyMaterial(go.GetComponent<Renderer>(), material);
            return go;
        }

        private static Material EnsureMaterial(string name, Color color)
        {
            EnsureFolder("Assets/RenkaiMobile/Generated/Materials");
            string path = "Assets/RenkaiMobile/Generated/Materials/" + name + ".mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) shader = Shader.Find("Standard");
                material = new Material(shader) { name = name };
                AssetDatabase.CreateAsset(material, path);
            }
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            if (material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * 0.65f);
            }
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void ApplyMaterial(Renderer renderer, Material material)
        {
            if (renderer != null && material != null) renderer.sharedMaterial = material;
        }

        private static ScriptableObject EnsureScriptable(string typeName, string path)
        {
            Type type = FindType(typeName);
            if (type == null)
            {
                AddMissing(typeName);
                return null;
            }

            ScriptableObject asset = AssetDatabase.LoadAssetAtPath(path, type) as ScriptableObject;
            if (asset != null) return asset;
            EnsureFolder(path.Substring(0, path.LastIndexOf('/')));
            asset = ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            int slash = path.LastIndexOf('/');
            if (slash <= 0) return;
            string parent = path.Substring(0, slash);
            string name = path.Substring(slash + 1);
            if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }

        private static GameObject CreateUiPanel(Transform parent, string name, Vector2 anchor, Vector2 position, Vector2 size, Color color)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = anchor;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = position;
            rt.sizeDelta = size;
            go.GetComponent<Image>().color = color;
            return go;
        }

        private static Text CreateUiText(Transform parent, string name, string value, Vector2 position, Vector2 size, int fontSize)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            Text text = go.GetComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            RectTransform rt = text.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = position;
            rt.sizeDelta = size;
            return text;
        }

        private static void EnsureEventSystem()
        {
            EventSystem existing = UnityEngine.Object.FindFirstObjectByType<EventSystem>();
            if (existing != null) return;
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        private static GameObject EnsureObject(string name)
        {
            return GameObject.Find(name) ?? new GameObject(name);
        }

        private static Transform EnsureChild(Transform parent, string name)
        {
            Transform child = parent.Find(name);
            if (child != null) return child;
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go.transform;
        }

        private static Component EnsureComponent(GameObject go, string typeName)
        {
            if (go == null) return null;
            Type type = FindType(typeName);
            if (type == null)
            {
                AddMissing(typeName);
                return null;
            }
            Component component = go.GetComponent(type);
            return component != null ? component : go.AddComponent(type);
        }

        private static Type FindType(string fullName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(fullName, false);
                if (type != null) return type;
            }
            return null;
        }

        private static Component FindSceneComponent(string typeName)
        {
            Type type = FindType(typeName);
            if (type == null) return null;
            foreach (UnityEngine.Object obj in Resources.FindObjectsOfTypeAll(type))
            {
                Component component = obj as Component;
                if (IsSceneComponent(component)) return component;
            }
            return null;
        }

        private static bool IsSceneComponent(Component component)
        {
            return component != null && component.gameObject.scene.IsValid() && component.gameObject.scene.isLoaded;
        }

        private static void SetTeam(Component component, string teamName)
        {
            if (component == null) return;
            Type enumType = FindType("RenkaiMobile.Core.MobileTeamId");
            if (enumType == null) return;
            object value = Enum.Parse(enumType, teamName);
            MethodInfo method = component.GetType().GetMethod("SetTeam", BindingFlags.Instance | BindingFlags.Public);
            method?.Invoke(component, new[] { value });
        }

        private static void ConfigureRoster(Component component, int slot, bool playerControlled)
        {
            if (component == null) return;
            Invoke(component, "Configure", slot, playerControlled);
            InvokeNoArgs(component, "CaptureSpawn");
        }

        private static void Bind(UnityEngine.Object target, string propertyName, UnityEngine.Object value)
        {
            if (target == null || value == null) return;
            SerializedObject so = new SerializedObject(target);
            SerializedProperty property = so.FindProperty(propertyName);
            if (property == null) return;
            property.objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetEnum(UnityEngine.Object target, string propertyName, int enumIndex)
        {
            if (target == null) return;
            SerializedObject so = new SerializedObject(target);
            SerializedProperty property = so.FindProperty(propertyName);
            if (property == null) return;
            property.enumValueIndex = enumIndex;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void InvokeNoArgs(Component component, string methodName)
        {
            if (component == null) return;
            MethodInfo method = component.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
            method?.Invoke(component, null);
        }

        private static void Invoke(Component component, string methodName, params object[] args)
        {
            if (component == null) return;
            MethodInfo method = component.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
            method?.Invoke(component, args);
        }

        private static string GetPropertyString(Component component, string propertyName)
        {
            object value = component?.GetType().GetProperty(propertyName)?.GetValue(component);
            return value != null ? value.ToString() : "None";
        }

        private static int GetPropertyInt(Component component, string propertyName)
        {
            object value = component?.GetType().GetProperty(propertyName)?.GetValue(component);
            return value is int result ? result : 0;
        }

        private static bool GetPropertyBool(Component component, string propertyName)
        {
            object value = component?.GetType().GetProperty(propertyName)?.GetValue(component);
            return value is bool result && result;
        }

        private static void AddMissing(string typeName)
        {
            if (!MissingTypes.Contains(typeName)) MissingTypes.Add(typeName);
        }

        private static string BuildReport()
        {
            string report = "FULL BUILD COMPLETE\n\n" +
                            "Created/updated automatically:\n" +
                            "• Player + mobile movement/look\n" +
                            "• 5v5 roster and bot stack\n" +
                            "• Zodiac objective + A/B zones\n" +
                            "• Kagami District playable map\n" +
                            "• Spawn points, covers and route graph\n" +
                            "• Combat/event/stats/VFX services\n" +
                            "• Kitsune AR + Zero Pulse + Hikari Blade\n" +
                            "• ADS, recoil, abilities and Zodiac interaction\n" +
                            "• Premium match HUD + touch combat buttons\n" +
                            "• Presentation shell roots\n\n" +
                            "No file picker was opened.";

            if (MissingTypes.Count > 0)
                report += "\n\nMissing optional script types:\n- " + string.Join("\n- ", MissingTypes);
            if (BuildNotes.Count > 0)
                report += "\n\nNotes:\n- " + string.Join("\n- ", BuildNotes);
            return report;
        }
    }
}
