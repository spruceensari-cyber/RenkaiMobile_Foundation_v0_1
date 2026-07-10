using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using RenkaiMobile.Audio;
using RenkaiMobile.Bots;
using RenkaiMobile.Combat;
using RenkaiMobile.Events;
using RenkaiMobile.Rounds;
using RenkaiMobile.Spawning;
using RenkaiMobile.Stats;
using RenkaiMobile.Teams;
using RenkaiMobile.UI;
using RenkaiMobile.VFX;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiVerticalSliceSceneIntegrator
    {
        [MenuItem("Renkai Mobile/Integrate Vertical Slice Scene")]
        public static void Integrate()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Play Mode'u kapat ve tekrar çalıştır.", "OK");
                return;
            }

            GameObject services = GameObject.Find("RenkaiMobile_Services") ?? new GameObject("RenkaiMobile_Services");
            Ensure<MobileDamagePolicy>(services);
            Ensure<MobileHitFeedbackBus>(services);
            Ensure<MobileKillEventBus>(services);
            Ensure<MobileKillEventRelay>(services);
            Ensure<CompetitiveMomentTracker>(services);
            Ensure<CompetitiveAnnouncementQueue>(services);
            Ensure<HighlightMomentTracker>(services);
            Ensure<MobileSoundEventBus>(services);
            Ensure<MobileVfxPool>(services);
            Ensure<MatchStatsTracker>(services);
            Ensure<MvpScoreCalculator>(services);
            Ensure<MobileMatchStateController>(services);
            Ensure<MobileTeamVisionService>(services);

            EnsureSpawnServices(services);
            IntegrateRoster();
            IntegrateBots();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog(
                "Renkai Mobile",
                "Vertical slice scene integration tamamlandı. Core combat, events, stats, sound, VFX, vision, spawn ve bot navigation servisleri sahneye bağlandı.",
                "OK");
        }

        private static void IntegrateRoster()
        {
            FiveVFiveRosterAgent[] roster = Object.FindObjectsByType<FiveVFiveRosterAgent>(FindObjectsSortMode.None);
            foreach (FiveVFiveRosterAgent agent in roster)
            {
                if (agent == null) continue;

                MobileCombatantIdentity identity = agent.GetComponent<MobileCombatantIdentity>();
                if (identity == null) identity = agent.gameObject.AddComponent<MobileCombatantIdentity>();

                string id = agent.Team.ToString().ToLowerInvariant() + "_" + agent.SlotIndex;
                string shownName = agent.PlayerControlled ? "ENSARI" : agent.Team + " " + (agent.SlotIndex + 1);
                string agentId = agent.SlotIndex % 3 == 0 ? "raika" : agent.SlotIndex % 3 == 1 ? "akari" : "kuroha";
                identity.Configure(id, shownName, agentId, agent.PlayerControlled);

                if (agent.GetComponent<MobileVisibilityState>() == null)
                    agent.gameObject.AddComponent<MobileVisibilityState>();
            }
        }

        private static void IntegrateBots()
        {
            FiveVFiveRosterAgent[] roster = Object.FindObjectsByType<FiveVFiveRosterAgent>(FindObjectsSortMode.None);
            foreach (FiveVFiveRosterAgent agent in roster)
            {
                if (agent == null || agent.PlayerControlled) continue;
                Ensure<BotNavigationAgent>(agent.gameObject);
                Ensure<BotNavigationIntentController>(agent.gameObject);
                Ensure<BotPerceptionMemory>(agent.gameObject);
                Ensure<BotPerception>(agent.gameObject);
                Ensure<BotInvestigateSoundState>(agent.gameObject);
                Ensure<KagamiTacticalRouteBrain>(agent.gameObject);
            }
        }

        private static void EnsureSpawnServices(GameObject services)
        {
            MobileSpawnRegistry registry = Object.FindFirstObjectByType<MobileSpawnRegistry>();
            if (registry == null) registry = Ensure<MobileSpawnRegistry>(services);

            MobileTeamSpawnController controller = Object.FindFirstObjectByType<MobileTeamSpawnController>();
            if (controller == null) Ensure<MobileTeamSpawnController>(services);

            registry.Refresh();
        }

        private static T Ensure<T>(GameObject root) where T : Component
        {
            T component = root.GetComponent<T>();
            return component != null ? component : root.AddComponent<T>();
        }
    }
}
