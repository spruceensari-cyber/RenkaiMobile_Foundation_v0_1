using System;
using System.Collections;
using UnityEngine;
using RenkaiMobile.Combat;

namespace RenkaiMobile.Weapons
{
    public readonly struct MobileWeaponShot
    {
        public MobileWeaponShot(Vector3 origin, Vector3 endPoint, bool hit, bool headshot)
        {
            Origin = origin;
            EndPoint = endPoint;
            Hit = hit;
            Headshot = headshot;
        }

        public Vector3 Origin { get; }
        public Vector3 EndPoint { get; }
        public bool Hit { get; }
        public bool Headshot { get; }
    }

    public sealed class MobileRifleController : MonoBehaviour
    {
        [SerializeField] private MobileWeaponProfile profile;
        [SerializeField] private Camera aimCamera;
        [SerializeField] private Transform muzzle;
        [SerializeField] private MobileAdsController adsController;
        [SerializeField] private MobileWeaponShotSignal shotSignal;
        [SerializeField] private MobileDamagePolicy damagePolicy;
        [SerializeField] private LayerMask hitMask = ~0;

        public event Action<int, int> AmmoChanged;
        public event Action<MobileWeaponShot> ShotFired;
        public event Action ReloadStarted;
        public event Action ReloadFinished;

        public int AmmoInMagazine { get; private set; }
        public bool IsReloading { get; private set; }

        private float nextFireTime;

        private void Awake()
        {
            if (aimCamera == null) aimCamera = Camera.main;
            if (adsController == null) adsController = GetComponentInParent<MobileAdsController>();
            if (shotSignal == null) shotSignal = GetComponentInParent<MobileWeaponShotSignal>();
            if (damagePolicy == null) damagePolicy = FindFirstObjectByType<MobileDamagePolicy>();
            AmmoInMagazine = profile != null ? profile.magazineSize : 0;
        }

        public bool TryFire()
        {
            if (profile == null || aimCamera == null || IsReloading) return false;
            if (Time.time < nextFireTime || AmmoInMagazine <= 0) return false;

            nextFireTime = Time.time + profile.fireInterval;
            AmmoInMagazine--;
            AmmoChanged?.Invoke(AmmoInMagazine, profile.magazineSize);
            shotSignal?.PublishShot();

            float spread = profile.hipSpreadDegrees;
            if (adsController != null && adsController.IsAds)
                spread *= profile.adsSpreadMultiplier * adsController.SpreadMultiplier;

            Vector3 direction = ApplySpread(aimCamera.transform.forward, spread);
            Vector3 origin = aimCamera.transform.position;
            Vector3 endPoint = origin + direction * profile.range;
            bool hitSomething = false;
            bool headshot = false;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, profile.range, hitMask, QueryTriggerInteraction.Ignore))
            {
                hitSomething = true;
                endPoint = hit.point;
                MobileHealth health = hit.collider.GetComponentInParent<MobileHealth>();
                if (health != null && (damagePolicy == null || damagePolicy.CanDamage(gameObject, health.gameObject)))
                {
                    headshot = hit.collider.name.ToLowerInvariant().Contains("head");
                    float damage = headshot ? profile.headDamage : profile.bodyDamage;
                    health.ApplyDamage(new MobileDamageInfo(damage, hit.point, direction, gameObject, headshot));
                }
            }

            ShotFired?.Invoke(new MobileWeaponShot(muzzle != null ? muzzle.position : origin, endPoint, hitSomething, headshot));
            return true;
        }

        public void BeginAutomaticFire()
        {
            if (!isActiveAndEnabled) return;
            StopCoroutine(nameof(AutoFireRoutine));
            StartCoroutine(nameof(AutoFireRoutine));
        }

        public void EndAutomaticFire()
        {
            StopCoroutine(nameof(AutoFireRoutine));
        }

        public void TryReload()
        {
            if (!IsReloading && profile != null && AmmoInMagazine < profile.magazineSize)
                StartCoroutine(ReloadRoutine());
        }

        private IEnumerator AutoFireRoutine()
        {
            do
            {
                TryFire();
                yield return null;
            }
            while (profile != null && profile.automatic);
        }

        private IEnumerator ReloadRoutine()
        {
            IsReloading = true;
            ReloadStarted?.Invoke();
            yield return new WaitForSeconds(profile.reloadSeconds);
            AmmoInMagazine = profile.magazineSize;
            IsReloading = false;
            AmmoChanged?.Invoke(AmmoInMagazine, profile.magazineSize);
            ReloadFinished?.Invoke();
        }

        private static Vector3 ApplySpread(Vector3 forward, float spreadDegrees)
        {
            if (spreadDegrees <= 0.001f) return forward.normalized;
            Quaternion yaw = Quaternion.AngleAxis(UnityEngine.Random.Range(-spreadDegrees, spreadDegrees), Vector3.up);
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
            Quaternion pitch = Quaternion.AngleAxis(UnityEngine.Random.Range(-spreadDegrees, spreadDegrees), right);
            return (yaw * pitch * forward).normalized;
        }
    }
}
