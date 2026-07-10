using System.Collections;
using UnityEngine;

namespace RenkaiMobile.Abilities
{
    public sealed class KurohaAbilityKit : MonoBehaviour
    {
        [SerializeField] private AbilityRuntimeController runtime;
        [SerializeField] private float veilSeconds = 4f;
        [SerializeField] private float phaseStepDistance = 4.5f;
        [SerializeField] private Renderer[] bodyRenderers;

        private void Awake()
        {
            if (runtime == null) runtime = GetComponent<AbilityRuntimeController>();
        }

        public void UsePhaseStep()
        {
            if (runtime == null || !runtime.TryUseAbility(0)) return;
            transform.position += transform.forward * phaseStepDistance;
        }

        public void UseVeil()
        {
            if (runtime == null || !runtime.TryUseAbility(1)) return;
            StartCoroutine(VeilRoutine());
        }

        public void UseNullEcho()
        {
            if (runtime == null || !runtime.TryUseAbility(2)) return;
            GameObject echo = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            echo.name = "Kuroha_NullEcho";
            echo.transform.position = transform.position + transform.forward * 2f;
            echo.transform.rotation = transform.rotation;
            Collider c = echo.GetComponent<Collider>();
            if (c != null) Destroy(c);
            Destroy(echo, 4f);
        }

        private IEnumerator VeilRoutine()
        {
            SetVisible(false);
            yield return new WaitForSeconds(veilSeconds);
            SetVisible(true);
        }

        private void SetVisible(bool visible)
        {
            if (bodyRenderers == null || bodyRenderers.Length == 0)
                bodyRenderers = GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in bodyRenderers)
                if (renderer != null) renderer.enabled = visible;
        }
    }
}
