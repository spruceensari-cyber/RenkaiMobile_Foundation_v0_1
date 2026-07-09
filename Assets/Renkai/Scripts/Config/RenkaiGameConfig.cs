using UnityEngine;

namespace Renkai.Config
{
    [CreateAssetMenu(menuName = "Renkai/Config/Game Config", fileName = "RenkaiGameConfig")]
    public sealed class RenkaiGameConfig : ScriptableObject
    {
        [Header("Frame Rate")]
        public int lowTierFps = 30;
        public int defaultFps = 60;
        public int highTierFps = 120;

        [Header("Camera")]
        [Range(60f, 110f)] public float defaultFov = 86f;

        [Header("Identity")]
        public string gameName = "Renkai";
        public string objectiveName = "Spirit Core";
    }
}
