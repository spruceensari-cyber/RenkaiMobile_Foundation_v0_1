using System.Collections;
using UnityEngine;
using RenkaiMobile.Weapons;

namespace RenkaiMobile.VFX
{
    public sealed class MobileWeaponVfxController : MonoBehaviour
    {
        [SerializeField] private MobileRifleController rifle;
        [SerializeField] private Transform muzzle;
        [SerializeField] private MobileVfxPool pool;
        [SerializeField] private MobileWeaponVfxPoolProfile poolProfile;
        [SerializeField] private float muzzleFlashSeconds = 0.045f;
        [SerializeField] private float tracerSeconds = 0.06f;
        [SerializeField] private float impactSeconds = 0.22f;

        private void Awake()
        {
            if (rifle == null) rifle = GetComponent<MobileRifleController>();
            if (pool == null) pool = FindFirstObjectByType<MobileVfxPool>();
        }

        private void Start()
        {
            if (pool == null || poolProfile == null) return;
            pool.Prewarm("muzzle", poolProfile.muzzlePrefab, poolProfile.muzzlePrewarm);
            pool.Prewarm("tracer", poolProfile.tracerPrefab, poolProfile.tracerPrewarm);
            pool.Prewarm("impact", poolProfile.impactPrefab, poolProfile.impactPrewarm);
            pool.Prewarm("headshot", poolProfile.headshotPrefab, poolProfile.headshotPrewarm);
        }

        private void OnEnable()
        {
            if (rifle != null) rifle.ShotFired += OnShotFired;
        }

        private void OnDisable()
        {
            if (rifle != null) rifle.ShotFired -= OnShotFired;
        }

        private void OnShotFired(MobileWeaponShot shot)
        {
            if (pool == null || poolProfile == null) return;
            Vector3 origin = muzzle != null ? muzzle.position : shot.Origin;
            SpawnTimed("muzzle", poolProfile.muzzlePrefab, origin, Quaternion.identity, muzzleFlashSeconds);
            SpawnTracer(origin, shot.EndPoint);
            if (shot.Hit)
                SpawnTimed(shot.Headshot ? "headshot" : "impact",
                    shot.Headshot ? poolProfile.headshotPrefab : poolProfile.impactPrefab,
                    shot.EndPoint,
                    Quaternion.identity,
                    impactSeconds);
        }

        private void SpawnTracer(Vector3 start, Vector3 end)
        {
            MobileVfxPoolItem item = pool.Spawn("tracer", poolProfile.tracerPrefab, start, Quaternion.identity);
            if (item == null) return;
            LineRenderer line = item.GetComponentInChildren<LineRenderer>();
            if (line != null)
            {
                line.positionCount = 2;
                line.SetPosition(0, start);
                line.SetPosition(1, end);
            }
            StartCoroutine(ReturnLater(item, tracerSeconds));
        }

        private void SpawnTimed(string key, GameObject prefab, Vector3 position, Quaternion rotation, float lifetime)
        {
            MobileVfxPoolItem item = pool.Spawn(key, prefab, position, rotation);
            if (item != null) StartCoroutine(ReturnLater(item, lifetime));
        }

        private static IEnumerator ReturnLater(MobileVfxPoolItem item, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            item?.ReturnToPool();
        }
    }
}
