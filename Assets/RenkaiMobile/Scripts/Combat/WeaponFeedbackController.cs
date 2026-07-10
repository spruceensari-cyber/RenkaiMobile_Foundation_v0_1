using UnityEngine;
using Renkai.Combat;
using RenkaiMobile.UI;

namespace RenkaiMobile.Combat
{
    [RequireComponent(typeof(HitscanWeapon))]
    public sealed class WeaponFeedbackController : MonoBehaviour
    {
        [SerializeField] private CrosshairController crosshair;
        [SerializeField] private float normalPulse = 0.35f;
        [SerializeField] private float headshotPulse = 0.85f;
        [SerializeField] private float impactLifetime = 0.18f;
        [SerializeField] private float tracerLifetime = 0.055f;

        private HitscanWeapon weapon;
        private static Material tracerMaterial;
        private static Material impactMaterial;
        private static Material criticalImpactMaterial;

        private void Awake()
        {
            weapon = GetComponent<HitscanWeapon>();
            if (crosshair == null) crosshair = Object.FindFirstObjectByType<CrosshairController>();
        }

        private void OnEnable()
        {
            if (weapon == null) weapon = GetComponent<HitscanWeapon>();
            weapon.HitConfirmed += OnHitConfirmed;
            weapon.ShotFired += OnShotFired;
        }

        private void OnDisable()
        {
            if (weapon != null) weapon.HitConfirmed -= OnHitConfirmed;
            if (weapon != null) weapon.ShotFired -= OnShotFired;
        }

        private void OnHitConfirmed(RaycastHit hit, bool headshot)
        {
            if (crosshair != null)
            {
                crosshair.Pulse(headshot ? headshotPulse : normalPulse);
                crosshair.ShowHitMarker(headshot);
            }

            var impact = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            impact.name = headshot ? "HeadshotImpact" : "HitImpact";
            impact.transform.position = hit.point + hit.normal * 0.015f;
            impact.transform.localScale = Vector3.one * (headshot ? 0.14f : 0.08f);
            var renderer = impact.GetComponent<Renderer>();
            if (renderer != null) renderer.sharedMaterial = GetImpactMaterial(headshot);
            var collider = impact.GetComponent<Collider>();
            if (collider != null) Destroy(collider);
            Destroy(impact, impactLifetime);
        }

        private void OnShotFired(WeaponShot shot)
        {
            if (crosshair != null) crosshair.Pulse(0.12f);

            var tracer = new GameObject("RenkaiTracer");
            var line = tracer.AddComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.positionCount = 2;
            line.SetPosition(0, shot.Origin);
            line.SetPosition(1, shot.EndPoint);
            line.startWidth = 0.018f;
            line.endWidth = 0.006f;
            line.numCapVertices = 2;
            line.material = GetTracerMaterial();
            Color color = shot.Headshot ? new Color(1f, 0.35f, 0.9f, 0.95f) : new Color(0.2f, 0.9f, 1f, 0.8f);
            line.startColor = color;
            line.endColor = new Color(color.r, color.g, color.b, 0f);
            Destroy(tracer, tracerLifetime);
        }

        private static Material GetTracerMaterial()
        {
            if (tracerMaterial != null) return tracerMaterial;
            Shader shader = Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Color");
            tracerMaterial = new Material(shader) { color = Color.white };
            return tracerMaterial;
        }

        private static Material GetImpactMaterial(bool headshot)
        {
            ref Material material = ref headshot ? ref criticalImpactMaterial : ref impactMaterial;
            if (material != null) return material;

            Shader shader = Shader.Find("Standard") ?? Shader.Find("Sprites/Default");
            material = new Material(shader);
            Color color = headshot ? new Color(1f, 0.2f, 0.7f, 1f) : new Color(0.1f, 0.85f, 1f, 1f);
            if (material.HasProperty("_Color")) material.color = color;
            if (material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * 2.5f);
            }
            return material;
        }
    }
}
