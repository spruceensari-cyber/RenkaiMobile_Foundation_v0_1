using System;
using UnityEngine;

namespace RenkaiMobile.Agents
{
    public sealed class AgentSelectionController : MonoBehaviour
    {
        [SerializeField] private AgentDefinition[] agents = Array.Empty<AgentDefinition>();
        [SerializeField] private int selectedIndex;

        public event Action<AgentDefinition, int> SelectionChanged;
        public event Action<AgentDefinition> SelectionLocked;

        public AgentDefinition SelectedAgent => agents != null && agents.Length > 0
            ? agents[Mathf.Clamp(selectedIndex, 0, agents.Length - 1)]
            : null;

        public bool Locked { get; private set; }

        private void Start()
        {
            NotifySelection();
        }

        public void SelectIndex(int index)
        {
            if (Locked || agents == null || agents.Length == 0) return;
            selectedIndex = Mathf.Clamp(index, 0, agents.Length - 1);
            NotifySelection();
        }

        public void SelectNext()
        {
            if (Locked || agents == null || agents.Length == 0) return;
            selectedIndex = (selectedIndex + 1) % agents.Length;
            NotifySelection();
        }

        public void SelectPrevious()
        {
            if (Locked || agents == null || agents.Length == 0) return;
            selectedIndex = (selectedIndex - 1 + agents.Length) % agents.Length;
            NotifySelection();
        }

        public void LockSelection()
        {
            if (Locked || SelectedAgent == null) return;
            Locked = true;
            SelectionLocked?.Invoke(SelectedAgent);
        }

        public void UnlockSelection()
        {
            Locked = false;
        }

        private void NotifySelection()
        {
            AgentDefinition selected = SelectedAgent;
            if (selected != null) SelectionChanged?.Invoke(selected, selectedIndex);
        }
    }
}
