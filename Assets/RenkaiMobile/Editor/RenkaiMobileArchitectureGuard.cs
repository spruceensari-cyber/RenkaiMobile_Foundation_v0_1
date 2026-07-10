using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiMobileArchitectureGuard
    {
        private const string Root = "Assets/RenkaiMobile";
        private const string ForbiddenUsing = "using Renkai.";

        [MenuItem("Renkai Mobile/Validate Mobile Isolation")]
        public static void Validate()
        {
            var violations = new List<string>();
            if (!Directory.Exists(Root))
            {
                EditorUtility.DisplayDialog("Renkai Mobile Isolation", "Assets/RenkaiMobile bulunamadı.", "OK");
                return;
            }

            string[] files = Directory.GetFiles(Root, "*.cs", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string normalized = file.Replace('\\', '/');
                string text = File.ReadAllText(file);
                if (text.Contains(ForbiddenUsing))
                    violations.Add(normalized);
            }

            if (violations.Count == 0)
            {
                Debug.Log("RENKAI MOBILE ISOLATION PASSED: legacy Renkai namespace dependency bulunmadı.");
                EditorUtility.DisplayDialog("Renkai Mobile Isolation", "PASSED\nLegacy Renkai namespace dependency bulunmadı.", "OK");
                return;
            }

            string message = "FAILED\n\nLegacy Renkai dependency bulunan dosyalar:\n- " + string.Join("\n- ", violations);
            Debug.LogError(message);
            EditorUtility.DisplayDialog("Renkai Mobile Isolation", message, "OK");
        }
    }
}
