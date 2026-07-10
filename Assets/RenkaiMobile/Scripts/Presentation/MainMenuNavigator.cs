using System;
using UnityEngine;

namespace RenkaiMobile.Presentation
{
    public enum MainMenuPanelId
    {
        Home,
        Play,
        Agents,
        Armory,
        Career,
        Settings
    }

    [Serializable]
    public sealed class MainMenuPanelBinding
    {
        public MainMenuPanelId id;
        public CanvasGroup group;
        public GameObject root;
    }

    public sealed class MainMenuNavigator : MonoBehaviour
    {
        [SerializeField] private MainMenuPanelBinding[] panels = Array.Empty<MainMenuPanelBinding>();
        [SerializeField] private MainMenuPanelId defaultPanel = MainMenuPanelId.Home;
        [SerializeField] private float fadeSpeed = 8f;

        public MainMenuPanelId CurrentPanel { get; private set; }
        public event Action<MainMenuPanelId> PanelChanged;

        private void Start()
        {
            Open(defaultPanel);
        }

        private void Update()
        {
            foreach (MainMenuPanelBinding panel in panels)
            {
                if (panel == null || panel.group == null) continue;
                float target = panel.id == CurrentPanel ? 1f : 0f;
                panel.group.alpha = Mathf.MoveTowards(panel.group.alpha, target, fadeSpeed * Time.unscaledDeltaTime);
                panel.group.interactable = target > 0.5f;
                panel.group.blocksRaycasts = target > 0.5f;
            }
        }

        public void Open(MainMenuPanelId id)
        {
            CurrentPanel = id;
            foreach (MainMenuPanelBinding panel in panels)
            {
                if (panel == null || panel.root == null) continue;
                panel.root.SetActive(true);
            }
            PanelChanged?.Invoke(id);
        }

        public void OpenHome() => Open(MainMenuPanelId.Home);
        public void OpenPlay() => Open(MainMenuPanelId.Play);
        public void OpenAgents() => Open(MainMenuPanelId.Agents);
        public void OpenArmory() => Open(MainMenuPanelId.Armory);
        public void OpenCareer() => Open(MainMenuPanelId.Career);
        public void OpenSettings() => Open(MainMenuPanelId.Settings);
    }
}
