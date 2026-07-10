using System.Collections;
using UnityEngine;

namespace RenkaiMobile.Combat
{
    [RequireComponent(typeof(Renkai.Combat.Health))]
    public sealed class TargetDummyFeedback : MonoBehaviour
    {
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private float flashSeconds = 0.08f;
        [SerializeField] private float respawnDelay = 2f;
        [SerializeField] private Color flashColor = Color.white;

        private Renkai.Combat.Health health;
        private Color[] baseColors;
        private Collider[] colliders;

        private void Awake()
        {
            health = GetComponent<Renkai.Combat.Health>();
            if (renderers == null || renderers.Length == 0)
                renderers = GetComponentsInChildren<Renderer>(true);
            colliders = GetComponentsInChildren<Collider>(true);
            baseColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
                baseColors[i] = renderers[i].material.HasProperty("_BaseColor")
                    ? renderers[i].material.GetColor("_BaseColor")
                    : renderers[i].material.color;
        }

        private void OnEnable()
        {
            if (health == null) health = GetComponent<Renkai.Combat.Health>();
            if (health == null) return;
            health.Changed += OnHealthChanged;
            health.Died += OnDied;
        }

        private void OnDisable()
        {
            if (health == null) return;
            health.Changed -= OnHealthChanged;
            health.Died -= OnDied;
        }

        private void OnHealthChanged(float current, float max)
        {
            StopCoroutine(nameof(FlashRoutine));
            StartCoroutine(nameof(FlashRoutine));
        }

        private IEnumerator FlashRoutine()
        {
            SetColor(flashColor);
            yield return new WaitForSeconds(flashSeconds);
            RestoreColors();
        }

        private void OnDied(Renkai.Combat.DamageInfo info)
        {
            StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            SetVisible(false);
            yield return new WaitForSeconds(respawnDelay);
            health.ResetHealth();
            RestoreColors();
            SetVisible(true);
        }

        private void SetVisible(bool value)
        {
            foreach (var r in renderers) if (r != null) r.enabled = value;
            foreach (var c in colliders) if (c != null) c.enabled = value;
        }

        private void SetColor(Color color)
        {
            foreach (var r in renderers)
            {
                if (r == null) continue;
                if (r.material.HasProperty("_BaseColor")) r.material.SetColor("_BaseColor", color);
                else r.material.color = color;
            }
        }

        private void RestoreColors()
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                var r = renderers[i];
                if (r == null) continue;
                if (r.material.HasProperty("_BaseColor")) r.material.SetColor("_BaseColor", baseColors[i]);
                else r.material.color = baseColors[i];
            }
        }
    }
}
