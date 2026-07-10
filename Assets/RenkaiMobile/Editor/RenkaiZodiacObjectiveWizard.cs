using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using RenkaiMobile.Objective;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiZodiacObjectiveWizard
    {
        [MenuItem("Renkai Mobile/Add Zodiac Objective Layer")]
        public static void Build()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Play Mode'u kapat ve tekrar çalıştır.", "OK");
                return;
            }

            GameObject root = GameObject.Find("Zodiac_Objective") ?? new GameObject("Zodiac_Objective");
            if (root.GetComponent<ZodiacObjective>() == null) root.AddComponent<ZodiacObjective>();

            CreateZone("Zodiac_Zone_A", "A", new Vector3(-10f, 0.05f, 10f));
            CreateZone("Zodiac_Zone_B", "B", new Vector3(10f, 0.05f, 18f));

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            EditorUtility.DisplayDialog("Renkai Mobile", "Zodiac objective layer eklendi: Zone A, Zone B, Activate/Disrupt/Collapse state sistemi hazır.", "OK");
        }

        private static void CreateZone(string name, string id, Vector3 position)
        {
            GameObject old = GameObject.Find(name);
            if (old != null) Object.DestroyImmediate(old);

            GameObject zone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            zone.name = name;
            zone.transform.position = position;
            zone.transform.localScale = new Vector3(4.5f, 0.05f, 4.5f);
            Collider collider = zone.GetComponent<Collider>();
            collider.isTrigger = true;
            ZodiacZone zodiacZone = zone.AddComponent<ZodiacZone>();
            zodiacZone.Configure(id);
        }
    }
}
