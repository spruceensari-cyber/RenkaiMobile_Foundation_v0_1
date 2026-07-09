using UnityEngine;
using Renkai.Config;

namespace Renkai.Core
{
    public sealed class RenkaiBootstrap : MonoBehaviour
    {
        [SerializeField] private RenkaiGameConfig config;
        [SerializeField] private Camera gameplayCamera;

        private void Awake()
        {
            Application.targetFrameRate = config != null ? config.defaultFps : 60;

            if (gameplayCamera != null && config != null)
                gameplayCamera.fieldOfView = config.defaultFov;

            QualitySettings.vSyncCount = 0;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}
