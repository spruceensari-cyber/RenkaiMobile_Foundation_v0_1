using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using RenkaiMobile.Map;
using RenkaiMobile.Presentation;
using RenkaiMobile.Rounds;
using RenkaiMobile.Spectate;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiVerticalSlicePresentationWizard
    {
        [MenuItem("Renkai Mobile/Setup Vertical Slice Presentation")]
        public static void Build()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Play Mode'u kapat ve tekrar çalıştır.", "OK");
                return;
            }

            GameObject services = GameObject.Find("RenkaiMobile_PresentationServices") ?? new GameObject("RenkaiMobile_PresentationServices");
            Ensure<MobileMatchStateController>(services);
            Ensure<TeamLineupPresenter>(services);
            Ensure<ScoreboardPresenter>(services);
            Ensure<DeathRecapPresenter>(services);
            Ensure<RoundTransitionPresenter>(services);
            Ensure<LocalDeathSpectateBridge>(services);

            TeamSpectateController spectate = Object.FindFirstObjectByType<TeamSpectateController>();
            if (spectate == null)
            {
                GameObject go = new GameObject("TeamSpectateController");
                spectate = go.AddComponent<TeamSpectateController>();
            }

            EnsureKagamiAtmosphere();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Renkai Mobile", "Vertical slice presentation services kuruldu: match states, lineup, scoreboard, death recap, spectate gating, round transitions ve Kagami atmosphere controller hazır.", "OK");
        }

        private static T Ensure<T>(GameObject root) where T : Component
        {
            T component = root.GetComponent<T>();
            return component != null ? component : root.AddComponent<T>();
        }

        private static void EnsureKagamiAtmosphere()
        {
            GameObject atmosphereRoot = GameObject.Find("Kagami_Atmosphere") ?? new GameObject("Kagami_Atmosphere");
            KagamiAtmosphereController controller = atmosphereRoot.GetComponent<KagamiAtmosphereController>();
            if (controller == null) atmosphereRoot.AddComponent<KagamiAtmosphereController>();

            if (atmosphereRoot.transform.Find("Rain") == null)
            {
                GameObject rain = new GameObject("Rain");
                rain.transform.SetParent(atmosphereRoot.transform, false);
                ParticleSystem ps = rain.AddComponent<ParticleSystem>();
                var main = ps.main;
                main.startSpeed = 18f;
                main.startLifetime = 2.5f;
                main.startSize = 0.035f;
                main.maxParticles = 1800;
                var shape = ps.shape;
                shape.shapeType = ParticleSystemShapeType.Box;
                shape.scale = new Vector3(40f, 1f, 50f);
                rain.transform.position = new Vector3(0f, 18f, 12f);
                rain.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            }

            if (atmosphereRoot.transform.Find("SyntheticBlossoms") == null)
            {
                GameObject blossoms = new GameObject("SyntheticBlossoms");
                blossoms.transform.SetParent(atmosphereRoot.transform, false);
                ParticleSystem ps = blossoms.AddComponent<ParticleSystem>();
                var main = ps.main;
                main.startSpeed = 1.5f;
                main.startLifetime = 7f;
                main.startSize = 0.12f;
                main.maxParticles = 500;
                var shape = ps.shape;
                shape.shapeType = ParticleSystemShapeType.Box;
                shape.scale = new Vector3(35f, 4f, 45f);
                blossoms.transform.position = new Vector3(0f, 7f, 14f);
            }
        }
    }
}
