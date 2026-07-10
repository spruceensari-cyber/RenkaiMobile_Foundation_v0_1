using UnityEngine;

namespace RenkaiMobile.Characters
{
    public sealed class CharacterModelSlot : MonoBehaviour
    {
        [SerializeField] private Transform modelRoot;
        [SerializeField] private Animator animator;

        public Transform ModelRoot => modelRoot;
        public Animator Animator => animator;

        public void Bind(Transform root, Animator modelAnimator)
        {
            modelRoot = root;
            animator = modelAnimator;
        }

        public void ReplaceModel(GameObject modelPrefab)
        {
            if (modelPrefab == null) return;
            if (modelRoot != null) Destroy(modelRoot.gameObject);

            GameObject instance = Instantiate(modelPrefab, transform);
            instance.name = modelPrefab.name + "_Runtime";
            modelRoot = instance.transform;
            animator = instance.GetComponentInChildren<Animator>();
        }
    }
}
