using System;
using UnityEngine;

namespace RenkaiMobile.Combat
{
    public sealed class MobileWeaponShotSignal : MonoBehaviour
    {
        public event Action ShotFired;

        public void PublishShot()
        {
            ShotFired?.Invoke();
        }
    }
}
