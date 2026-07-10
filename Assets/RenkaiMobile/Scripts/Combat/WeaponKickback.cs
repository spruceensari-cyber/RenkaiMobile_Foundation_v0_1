using UnityEngine;
using Renkai.Combat;

namespace RenkaiMobile.Combat
{
    [RequireComponent(typeof(HitscanWeapon))]
    public sealed class WeaponKickback : MonoBehaviour
    {
        [SerializeField] private Transform visualRoot;
        [SerializeField] private float kickDistance = 0.06f;
        [SerializeField] private float returnSpeed = 16f;
        [SerializeField] private float snapSpeed = 28f;

        private Vector3 restPosition;
        private Vector3 targetPosition;

        private void Awake()
        {
            if (visualRoot == null) visualRoot = transform;
            restPosition = visualRoot.localPosition;
            targetPosition = restPosition;
        }

        public void Kick()
        {
            targetPosition = restPosition + Vector3.back * kickDistance;
        }

        private void LateUpdate()
        {
            targetPosition = Vector3.Lerp(targetPosition, restPosition, returnSpeed * Time.deltaTime);
            visualRoot.localPosition = Vector3.Lerp(visualRoot.localPosition, targetPosition, snapSpeed * Time.deltaTime);
        }
    }
}
