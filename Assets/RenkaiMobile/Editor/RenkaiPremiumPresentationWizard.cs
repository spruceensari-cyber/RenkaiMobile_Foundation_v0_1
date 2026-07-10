using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using RenkaiMobile.Agents;
using RenkaiMobile.Stats;
using RenkaiMobile.UI;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiPremiumPresentationWizard
    {
        private const string AgentFolder = "Assets/RenkaiMobile/Data/Agents";

        [MenuItem("Renkai Mobile/Setup Premium Presentation Layer")]
        public static void Build()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Play Mode'u kapat ve tekrar çalıştır.", "OK");
                return;
            }

            EnsureFolder("Assets/RenkaiMobile/Data");
            EnsureFolder(AgentFolder);

            EnsureAgent("Raika", "raika", "Duelist", "Velocity becomes a weapon.",
                "Velocity Surge", "Arc Pulse", "Overdrive",
                new Color(0.1f, 0.75f, 1f, 1f), new Color(0.75f, 0.2f, 1f, 1f));

            EnsureAgent("Akari", "akari", "Initiator", "Plasma pressure breaks the line.",
                "Plasma Burst", "Ignition Field", "Solar Break",
                new Color(0.95f, 0.18f, 0.12f, 1f), new Color(0.35f, 0.04f, 0.05f, 1f));

            EnsureAgent("Kuroha", "kuroha", "Controller", "The void wins before the shot.",
                "Phase Step", "Veil", "Null Echo",
                new Color(0.55f, 0.2f, 0.9f, 1f), new Color(0.05f, 0.08f, 0.18f, 1f));

            GameObject services = GameObject.Find("RenkaiMobile_Presentation") ?? new GameObject("RenkaiMobile_Presentation");
            if (services.GetComponent<MvpScoreCalculator>() == null) services.AddComponent<MvpScoreCalculator>();
            if (services.GetComponent<MatchStatsTracker>() == null) services.AddComponent<MatchStatsTracker>();
            if (services.GetComponent<MatchPresentationDirector>() == null) services.AddComponent<MatchPresentationDirector>();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Renkai Mobile", "Premium presentation layer hazır: Raika/Akari/Kuroha data assets, match stats, MVP calculation ve presentation director eklendi.", "OK");
        }

        private static AgentDefinition EnsureAgent(string displayName, string id, string role, string tagline,
            string q, string e, string x, Color primary, Color secondary)
        {
            string path = AgentFolder + "/Agent_" + displayName + ".asset";
            AgentDefinition agent = AssetDatabase.LoadAssetAtPath<AgentDefinition>(path);
            if (agent == null)
            {
                agent = ScriptableObject.CreateInstance<AgentDefinition>();
                AssetDatabase.CreateAsset(agent, path);
            }

            agent.agentId = id;
            agent.displayName = displayName;
            agent.roleLabel = role;
            agent.tagline = tagline;
            agent.abilityOneName = q;
            agent.abilityTwoName = e;
            agent.signatureName = x;
            agent.primaryColor = primary;
            agent.secondaryColor = secondary;
            EditorUtility.SetDirty(agent);
            return agent;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            int slash = path.LastIndexOf('/');
            string parent = path.Substring(0, slash);
            string name = path.Substring(slash + 1);
            if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
