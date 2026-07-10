using UnityEngine;

namespace RenkaiMobile.Combat
{
    public sealed class MobileRecoilShotLink : MonoBehaviour
    {
        [SerializeField] private MobileWeaponShotSignal shotSignal;
        [SerializeField] private MobileRecoilDriver recoilDriver;

        private void Awake()
        {
            if (shotSignal == null) shotSignal = GetComponent<MobileWeaponShotSignal>();
            if (recoilDriver == null) recoilDriver = GetComponent<MobileRecoilDriver>();
        }

        private void OnEnable()
        {
            if (shotSignal != null) shotSignal.ShotFired += OnShotFired;
        }

        private void OnDisable()
        {
            if (shotSignal != null) shotSignal.ShotFired -= OnShotFired;
        }

        private void OnShotFired()
        {
            recoilDriver?.RegisterShot();
        }
    }
}
