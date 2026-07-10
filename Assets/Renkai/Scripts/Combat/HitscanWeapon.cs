using System;
using System.Collections;
using UnityEngine;
using Renkai.Movement;

namespace Renkai.Combat
{
    public readonly struct WeaponShot
    {
        public WeaponShot(Vector3 origin, Vector3 endPoint, bool hitSomething, bool headshot)
        {
            Origin = origin;
            EndPoint = endPoint;
            HitSomething = hitSomething;
            Headshot = headshot;
        }

        public Vector3 Origin { get; }
        public Vector3 EndPoint { get; }
        public bool HitSomething { get; }
        public bool Headshot { get; }
    }

    public sealed class HitscanWeapon : MonoBehaviour
    {
        [SerializeField] private WeaponDefinition definition;
        [SerializeField] private Camera aimCamera;
        [SerializeField] private LayerMask hitMask = ~0;
        [SerializeField] private Transform muzzle;
        [SerializeField] private MobileFpsMotor motor;

        public event Action<int, int> AmmoChanged;
        public event Action<RaycastHit, bool> HitConfirmed;
        public event Action<WeaponShot> ShotFired;
        public event Action ReloadStarted;
        public event Action ReloadFinished;

        public int AmmoInMagazine { get; private set; }
        public int MagazineSize => definition != null ? definition.magazineSize : 0;
        public bool IsReloading { get; private set; }
        public float ReloadProgress => !IsReloading || definition == null
            ? 0f
            : Mathf.Clamp01((Time.time - reloadStartedAt) / definition.reloadSeconds);
        public Transform Muzzle => muzzle;

        private float nextFireTime;
        private float reloadStartedAt;

        private void Awake()
        {
            if (aimCamera == null)
                aimCamera = Camera.main;

            if (definition != null)
                AmmoInMagazine = definition.magazineSize;
        }

        public bool TryFire()
        {
            if (definition == null || aimCamera == null || IsReloading)
                return false;

            if (Time.time < nextFireTime || AmmoInMagazine <= 0)
                return false;

            nextFireTime = Time.time + definition.fireInterval;
            AmmoInMagazine--;
            AmmoChanged?.Invoke(AmmoInMagazine, definition.magazineSize);

            float spread = definition.hipSpreadDegrees;
            if (motor != null && motor.PlanarSpeed > 0.25f)
                spread += definition.movingSpreadBonus;
            if (motor != null && motor.IsCrouching)
                spread *= definition.crouchSpreadMultiplier;

            Vector3 direction = ApplySpread(aimCamera.transform.forward, spread);
            Vector3 origin = aimCamera.transform.position;
            Vector3 endPoint = origin + direction * definition.range;
            bool hitSomething = false;
            bool headshot = false;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, definition.range, hitMask, QueryTriggerInteraction.Ignore))
            {
                hitSomething = true;
                endPoint = hit.point;
                headshot = hit.collider.GetComponent<HeadshotZone>() != null;
                float amount = headshot ? definition.headDamage : definition.bodyDamage;

                IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    damageable.ApplyDamage(new DamageInfo(amount, hit.point, direction, gameObject, headshot));
                    HitConfirmed?.Invoke(hit, headshot);
                }
            }

            Vector3 visualOrigin = muzzle != null ? muzzle.position : origin;
            ShotFired?.Invoke(new WeaponShot(visualOrigin, endPoint, hitSomething, headshot));

            return true;
        }

        public void TryReload()
        {
            if (!IsReloading && definition != null && AmmoInMagazine < definition.magazineSize)
                StartCoroutine(ReloadRoutine());
        }

        private IEnumerator ReloadRoutine()
        {
            IsReloading = true;
            reloadStartedAt = Time.time;
            ReloadStarted?.Invoke();
            yield return new WaitForSeconds(definition.reloadSeconds);
            AmmoInMagazine = definition.magazineSize;
            IsReloading = false;
            AmmoChanged?.Invoke(AmmoInMagazine, definition.magazineSize);
            ReloadFinished?.Invoke();
        }

        private static Vector3 ApplySpread(Vector3 forward, float spreadDegrees)
        {
            if (spreadDegrees <= 0.001f)
                return forward.normalized;

            Quaternion yaw = Quaternion.AngleAxis(UnityEngine.Random.Range(-spreadDegrees, spreadDegrees), Vector3.up);
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
            Quaternion pitch = Quaternion.AngleAxis(UnityEngine.Random.Range(-spreadDegrees, spreadDegrees), right);
            return (yaw * pitch * forward).normalized;
        }
    }
}
