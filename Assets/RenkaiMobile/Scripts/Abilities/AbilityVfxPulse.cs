using System.Collections;
using UnityEngine;

namespace RenkaiMobile.Abilities
{
    [RequireComponent(typeof(AbilityRuntimeController))]
    public sealed class AbilityVfxPulse : MonoBehaviour
    {
        [SerializeField] private float lifetime = 0.55f;
        [SerializeField] private float startScale = 0.2f;
        [SerializeField] private float endScale = 3.5f;
        [SerializeField] private Color[] slotColors =
        {
            new Color(0.1f, 0.75f, 1f, 0.8f),
            new Color(0.65f, 0.15f, 1f, 0.8f),
            new Color(1f, 0.2f, 0.55f, 0.85f)
        };

        private AbilityRuntimeController controller;

        private void Awake()
        {
            controller = GetComponent<AbilityRuntimeController>();
        }

        private void OnEnable()
        {
            if (controller != null) controller.AbilityUsed += OnAbilityUsed;
        }

        private void OnDisable()
        {
            if (controller != null) controller.AbilityUsed -= OnAbilityUsed;
        }

        private void OnAbilityUsed(int slot)
        {
            GameObject pulse = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pulse.name = "AbilityPulse_" + slot;
            pulse.transform.position = transform.position + Vector3.up;
            pulse.transform.localScale = Vector3.one * startScale;
            Collider collider = pulse.GetComponent<Collider>();
            if (collider != null) Destroy(collider);

            Renderer renderer = pulse.GetComponent<Renderer>();
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Unlit/Color");
            Material mat = new Material(shader);
            Color color = slotColors != null && slot < slotColors.Length ? slotColors[slot] : Color.cyan;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            else if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
            renderer.material = mat;

            StartCoroutine(AnimatePulse(pulse, mat));
        }

        private IEnumerator AnimatePulse(GameObject pulse, Material material)
        {
            float t = 0f;
            while (pulse != null && t < lifetime)
            {
                t += Time.deltaTime;
                float p = Mathf.Clamp01(t / lifetime);
                float scale = Mathf.Lerp(startScale, endScale, p);
                pulse.transform.localScale = Vector3.one * scale;
                yield return null;
            }

            if (pulse != null) Destroy(pulse);
            if (material != null) Destroy(material);
        }
    }
}
