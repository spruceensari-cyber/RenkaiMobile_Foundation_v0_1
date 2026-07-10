using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using RenkaiMobile.VFX;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiHolographicSightWizard
    {
        [MenuItem("Renkai Mobile/Add Holographic Weapon Sight")]
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

            Transform parent = FindWeaponRoot(player.transform);
            if (parent == null)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Weapon/Viewmodel root bulunamadı. Önce weapon viewmodel kurulumunu çalıştır.", "OK");
                return;
            }

            Transform old = parent.Find("Renkai_HoloSight");
            if (old != null) Object.DestroyImmediate(old.gameObject);

            GameObject root = new GameObject("Renkai_HoloSight");
            root.transform.SetParent(parent, false);
            root.transform.localPosition = new Vector3(0f, 0.04f, 0.34f);
            root.transform.localRotation = Quaternion.identity;

            Transform outer = CreateRing(root.transform, "OuterRing", 0.075f, 0.006f);
            Transform middle = CreateRing(root.transform, "MiddleRing", 0.052f, 0.004f);
            Transform inner = CreateReticle(root.transform);

            HolographicWeaponSight sight = root.AddComponent<HolographicWeaponSight>();
            SerializedObject so = new SerializedObject(sight);
            so.FindProperty("outerRing").objectReferenceValue = outer;
            so.FindProperty("middleRing").objectReferenceValue = middle;
            so.FindProperty("innerReticle").objectReferenceValue = inner;
            so.FindProperty("glowRenderers").arraySize = 3;
            so.FindProperty("glowRenderers").GetArrayElementAtIndex(0).objectReferenceValue = outer.GetComponent<Renderer>();
            so.FindProperty("glowRenderers").GetArrayElementAtIndex(1).objectReferenceValue = middle.GetComponent<Renderer>();
            so.FindProperty("glowRenderers").GetArrayElementAtIndex(2).objectReferenceValue = inner.GetComponent<Renderer>();
            so.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            EditorUtility.DisplayDialog("Renkai Mobile", "Katmanlı holografik weapon sight eklendi.", "OK");
        }

        private static Transform FindWeaponRoot(Transform root)
        {
            Transform[] all = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in all)
            {
                string n = t.name.ToLowerInvariant();
                if (n.Contains("viewmodel") || n.Contains("weaponroot") || n.Contains("weapon_view")) return t;
            }
            Camera cam = root.GetComponentInChildren<Camera>(true);
            return cam != null ? cam.transform : null;
        }

        private static Transform CreateRing(Transform parent, string name, float radius, float thickness)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            go.transform.localScale = new Vector3(radius, thickness, radius);
            Object.DestroyImmediate(go.GetComponent<Collider>());
            return go.transform;
        }

        private static Transform CreateReticle(Transform parent)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = "InnerReticle";
            go.transform.SetParent(parent, false);
            go.transform.localPosition = new Vector3(0f, 0f, 0.006f);
            go.transform.localScale = Vector3.one * 0.028f;
            Object.DestroyImmediate(go.GetComponent<Collider>());
            return go.transform;
        }
    }
}
