using UnityEngine;
using Renkai.Combat;
using RenkaiMobile.UI;

namespace RenkaiMobile.Combat
{
    [RequireComponent(typeof(HitscanWeapon))]
    public sealed class WeaponFeelBridge : MonoBehaviour
    {
        [SerializeField] private WeaponKickback kickback;
        [SerializeField] private CrosshairController crosshair;
        [SerializeField] private float firePulse = 0.18f;

        private HitscanWeapon weapon;

        private void Awake()
        {
            weapon = GetComponent<HitscanWeapon>();
            if (kickback == null) kickback = Object.FindFirstObjectByType<WeaponKickback>();
            if (crosshair == null) crosshair = Object.FindFirstObjectByType<CrosshairController>();
        }

        private void OnEnable()
        {
            if (weapon == null) weapon = GetComponent<HitscanWeapon>();
            weapon.Fired += OnFired;
        }

        private void OnDisable()
        {
            if (weapon != null) weapon.Fired -= OnFired;
        }

        private void OnFired()
        {
            kickback?.Kick();
            crosshair?.Pulse(firePulse);
        }
    }
}
