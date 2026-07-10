using UnityEngine;
using Renkai.Combat;
using Renkai.Movement;

namespace RenkaiMobile.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(HitscanWeapon))]
    public sealed class WeaponViewmodelController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AdsController adsController;
        [SerializeField] private MobileFpsMotor motor;
        [SerializeField] private Transform muzzleFlash;

        [Header("Viewmodel Pose")]
        [SerializeField] private Vector3 hipPosition = new Vector3(0.22f, -0.22f, 0.55f);
        [SerializeField] private Vector3 adsPosition = new Vector3(0.035f, -0.14f, 0.39f);
        [SerializeField] private Vector3 hipRotation = new Vector3(2f, -6f, 0f);
        [SerializeField] private Vector3 adsRotation = Vector3.zero;
        [SerializeField, Min(1f)] private float poseSpeed = 15f;

        [Header("Feel")]
        [SerializeField] private float bobFrequency = 8f;
        [SerializeField] private float bobAmount = 0.009f;
        [SerializeField] private float recoilRecovery = 13f;

        private HitscanWeapon weapon;
        private Vector3 recoilPosition;
        private Vector3 recoilRotation;
        private float flashTimer;

        public void Configure(AdsController ads, MobileFpsMotor fpsMotor, Transform flash)
        {
            adsController = ads;
            motor = fpsMotor;
            muzzleFlash = flash;
        }

        private void Awake()
        {
            weapon = GetComponent<HitscanWeapon>();
            if (adsController == null) adsController = GetComponentInParent<AdsController>();
            if (motor == null) motor = GetComponentInParent<MobileFpsMotor>();
        }

        private void OnEnable()
        {
            if (weapon == null) weapon = GetComponent<HitscanWeapon>();
            if (weapon != null) weapon.ShotFired += OnShotFired;
        }

        private void OnDisable()
        {
            if (weapon != null) weapon.ShotFired -= OnShotFired;
        }

        private void LateUpdate()
        {
            bool ads = adsController != null && adsController.IsAds;
            float movement = motor == null ? 0f : Mathf.Clamp01(motor.PlanarSpeed / 4.8f);
            float bobTime = Time.time * bobFrequency;
            Vector3 bob = new Vector3(Mathf.Sin(bobTime) * bobAmount * movement,
                Mathf.Abs(Mathf.Cos(bobTime * 0.5f)) * bobAmount * movement,
                0f);

            Vector3 posePosition = ads ? adsPosition : hipPosition;
            Vector3 poseRotation = ads ? adsRotation : hipRotation;
            if (weapon != null && weapon.IsReloading)
            {
                float reload = weapon.ReloadProgress;
                posePosition += new Vector3(0.08f, -0.04f, -0.07f) * Mathf.Sin(reload * Mathf.PI);
                poseRotation += new Vector3(18f, 0f, 32f) * Mathf.Sin(reload * Mathf.PI);
            }

            recoilPosition = Vector3.Lerp(recoilPosition, Vector3.zero, recoilRecovery * Time.deltaTime);
            recoilRotation = Vector3.Lerp(recoilRotation, Vector3.zero, recoilRecovery * Time.deltaTime);
            transform.localPosition = Vector3.Lerp(transform.localPosition, posePosition + bob + recoilPosition, poseSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation,
                Quaternion.Euler(poseRotation + recoilRotation), poseSpeed * Time.deltaTime);

            flashTimer -= Time.deltaTime;
            if (muzzleFlash != null) muzzleFlash.gameObject.SetActive(flashTimer > 0f);
        }

        private void OnShotFired(WeaponShot shot)
        {
            recoilPosition += new Vector3(0f, 0.004f, -0.062f);
            recoilRotation += new Vector3(-3.6f, Random.Range(-0.9f, 0.9f), Random.Range(-0.4f, 0.4f));
            flashTimer = 0.045f;
        }
    }
}
