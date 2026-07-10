using UnityEngine;

namespace RenkaiMobile.Objective
{
    public sealed class PlantAnimationBridge : MonoBehaviour
    {
        [SerializeField] private SpiritCoreRoundObjective objective;
        [SerializeField] private Animator animator;
        [SerializeField] private string plantedTrigger = "PlantComplete";
        [SerializeField] private string defusedTrigger = "DefuseComplete";
        [SerializeField] private string detonatedTrigger = "Detonate";

        private void Awake()
        {
            if (objective == null) objective = FindFirstObjectByType<SpiritCoreRoundObjective>();
            if (animator == null) animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            if (objective == null) return;
            objective.Planted += OnPlanted;
            objective.Defused += OnDefused;
            objective.Detonated += OnDetonated;
        }

        private void OnDisable()
        {
            if (objective == null) return;
            objective.Planted -= OnPlanted;
            objective.Defused -= OnDefused;
            objective.Detonated -= OnDetonated;
        }

        private void OnPlanted()
        {
            if (animator != null && !string.IsNullOrEmpty(plantedTrigger)) animator.SetTrigger(plantedTrigger);
        }

        private void OnDefused()
        {
            if (animator != null && !string.IsNullOrEmpty(defusedTrigger)) animator.SetTrigger(defusedTrigger);
        }

        private void OnDetonated()
        {
            if (animator != null && !string.IsNullOrEmpty(detonatedTrigger)) animator.SetTrigger(detonatedTrigger);
        }
    }
}
