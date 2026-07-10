using UnityEngine;

namespace RenkaiMobile.Presentation
{
    public sealed class AgentPoseDriver : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string introTrigger = "Intro";
        [SerializeField] private string mvpTrigger = "MVP";
        [SerializeField] private string defeatTrigger = "Defeat";
        [SerializeField] private string selectedBool = "Selected";

        private int introHash;
        private int mvpHash;
        private int defeatHash;
        private int selectedHash;

        private void Awake()
        {
            if (animator == null) animator = GetComponentInChildren<Animator>();
            introHash = Animator.StringToHash(introTrigger);
            mvpHash = Animator.StringToHash(mvpTrigger);
            defeatHash = Animator.StringToHash(defeatTrigger);
            selectedHash = Animator.StringToHash(selectedBool);
        }

        public void PlayIntro()
        {
            if (animator != null) animator.SetTrigger(introHash);
        }

        public void PlayMvp()
        {
            if (animator != null) animator.SetTrigger(mvpHash);
        }

        public void PlayDefeat()
        {
            if (animator != null) animator.SetTrigger(defeatHash);
        }

        public void SetSelected(bool selected)
        {
            if (animator != null) animator.SetBool(selectedHash, selected);
        }
    }
}
