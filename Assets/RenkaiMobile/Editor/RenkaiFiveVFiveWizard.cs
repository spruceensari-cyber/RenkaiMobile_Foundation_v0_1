using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Renkai.Core;
using Renkai.Combat;
using Renkai.Rounds;
using RenkaiMobile.Bots;
using RenkaiMobile.Rounds;
using RenkaiMobile.Teams;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiFiveVFiveWizard
    {
        [MenuItem("Renkai Mobile/Build 5v5 Tactical Match")]
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
                EditorUtility.DisplayDialog("Renkai Mobile", "Player bulunamadı. Önce First Playable Scene oluştur.", "OK");
                return;
            }

            RemoveOldMatchActors();
            ConfigurePlayer(player);
            BuildTeam("Attackers", TeamId.Attackers, new Vector3(0f, 1f, -12f), Vector3.forward, 4, -1f);
            BuildTeam("Defenders", TeamId.Defenders, new Vector3(0f, 1f, 26f), Vector3.back, 5, 1f);
            EnsureRoundSystems();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            Selection.activeGameObject = GameObject.Find("Renkai_5v5_Match");
            EditorUtility.DisplayDialog("Renkai Mobile", "5v5 tactical match kuruldu: oyuncu + 4 ally bot, 5 defender bot.", "OK");
        }

        private static void ConfigurePlayer(GameObject player)
        {
            var team = player.GetComponent<TeamMember>() ?? player.AddComponent<TeamMember>();
            team.SetTeam(TeamId.Attackers);
            if (player.GetComponent<Health>() == null) player.AddComponent<Health>();
            var roster = player.GetComponent<FiveVFiveRosterAgent>() ?? player.AddComponent<FiveVFiveRosterAgent>();
            roster.Configure(0, true);
            roster.CaptureSpawn();
        }

        private static void BuildTeam(string label, TeamId team, Vector3 origin, Vector3 forward, int count, float lateralSign)
        {
            GameObject root = new GameObject(label + "_Team");
            root.transform.SetParent(GameObject.Find("Renkai_5v5_Match").transform);

            for (int i = 0; i < count; i++)
            {
                float x = (i - (count - 1) * 0.5f) * 2.2f;
                Vector3 pos = origin + new Vector3(x * lateralSign, 0f, (i % 2) * 1.4f);
                GameObject bot = CreateBot(label + "_Bot_" + (i + 1), team, pos, Quaternion.LookRotation(forward));
                bot.transform.SetParent(root.transform);
                var roster = bot.GetComponent<FiveVFiveRosterAgent>();
                roster.Configure(i + 1, false);
                roster.CaptureSpawn();
            }
        }

        private static GameObject CreateBot(string name, TeamId teamId, Vector3 position, Quaternion rotation)
        {
            GameObject bot = new GameObject(name);
            bot.transform.SetPositionAndRotation(position, rotation);
            var team = bot.AddComponent<TeamMember>();
            team.SetTeam(teamId);
            bot.AddComponent<Health>();
            bot.AddComponent<FiveVFiveRosterAgent>();
            bot.AddComponent<TacticalBotBrain>();

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Body";
            body.transform.SetParent(bot.transform, false);
            body.transform.localPosition = new Vector3(0f, 1f, 0f);
            body.transform.localScale = new Vector3(0.75f, 1f, 0.75f);
            Object.DestroyImmediate(body.GetComponent<Collider>());

            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "Head";
            head.transform.SetParent(bot.transform, false);
            head.transform.localPosition = new Vector3(0f, 2.15f, 0f);
            head.transform.localScale = Vector3.one * 0.32f;
            head.AddComponent<HeadshotZone>();

            var capsule = bot.AddComponent<CapsuleCollider>();
            capsule.center = new Vector3(0f, 1.15f, 0f);
            capsule.height = 2.3f;
            capsule.radius = 0.42f;
            return bot;
        }

        private static void EnsureRoundSystems()
        {
            GameObject root = GameObject.Find("Renkai_5v5_Match");
            if (root == null) root = new GameObject("Renkai_5v5_Match");

            var round = root.GetComponent<RoundManager>() ?? root.AddComponent<RoundManager>();
            var director = root.GetComponent<FiveVFiveRoundDirector>() ?? root.AddComponent<FiveVFiveRoundDirector>();
            var so = new SerializedObject(director);
            so.FindProperty("roundManager").objectReferenceValue = round;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void RemoveOldMatchActors()
        {
            var oldRoot = GameObject.Find("Renkai_5v5_Match");
            if (oldRoot != null) Object.DestroyImmediate(oldRoot);

            var attackers = GameObject.Find("Attackers_Team");
            if (attackers != null) Object.DestroyImmediate(attackers);
            var defenders = GameObject.Find("Defenders_Team");
            if (defenders != null) Object.DestroyImmediate(defenders);

            GameObject root = new GameObject("Renkai_5v5_Match");
        }
    }
}
