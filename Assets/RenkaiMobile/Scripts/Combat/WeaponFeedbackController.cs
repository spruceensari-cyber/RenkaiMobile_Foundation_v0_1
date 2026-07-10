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

        private HitscanWeapon weapon;

        private void Awake()
        {
            weapon = GetComponent<HitscanWeapon>();
            if (crosshair == null) crosshair = Object.FindFirstObjectByType<CrosshairController>();
        }

        private void OnEnable()
        {
            if (weapon == null) weapon = GetComponent<HitscanWeapon>();
            weapon.HitConfirmed += OnHitConfirmed;
        }

        private void OnDisable()
        {
            if (weapon != null) weapon.HitConfirmed -= OnHitConfirmed;
        }

        private void OnHitConfirmed(RaycastHit hit, bool headshot)
        {
            if (crosshair != null) crosshair.Pulse(headshot ? headshotPulse : normalPulse);

            var impact = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            impact.name = headshot ? "HeadshotImpact" : "HitImpact";
            impact.transform.position = hit.point + hit.normal * 0.015f;
            impact.transform.localScale = Vector3.one * (headshot ? 0.14f : 0.08f);
            var collider = impact.GetComponent<Collider>();
            if (collider != null) Destroy(collider);
            Destroy(impact, impactLifetime);
        }
    }
}
