using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using RenkaiMobile.UI;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiHolographicHudWizard
    {
        [MenuItem("Renkai Mobile/Polish Holographic HUD")]
        public static void Build()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Play Mode'u kapat ve tekrar çalıştır.", "OK");
                return;
            }

            Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            foreach (Canvas canvas in canvases)
            {
                if (canvas == null) continue;
                RectTransform[] rects = canvas.GetComponentsInChildren<RectTransform>(true);
                foreach (RectTransform rect in rects)
                {
                    if (rect == null || rect == canvas.transform) continue;
                    string n = rect.name.ToLowerInvariant();
                    bool important = n.Contains("hud") || n.Contains("panel") || n.Contains("teamstatus") || n.Contains("minimap") || n.Contains("killfeed");
                    if (!important) continue;
                    if (rect.GetComponent<CanvasGroup>() == null) rect.gameObject.AddComponent<CanvasGroup>();
                    if (rect.GetComponent<HolographicHudPanel>() == null) rect.gameObject.AddComponent<HolographicHudPanel>();
                }
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            EditorUtility.DisplayDialog("Renkai Mobile", "Holographic HUD motion/pulse polish uygulandı.", "OK");
        }
    }
}
