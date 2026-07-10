using System.Collections.Generic;
using UnityEngine;

namespace RenkaiMobile.Combat
{
    public enum CameraMotionLayer
    {
        Recoil,
        Landing,
        Damage,
        Ability,
        SprintBob,
        Cinematic
    }

    public sealed class MobileCameraMotionStack : MonoBehaviour
    {
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private float recoverySharpness = 14f;
        [SerializeField] private float rotationSharpness = 18f;

        private readonly Dictionary<CameraMotionLayer, Vector3> positionLayers = new Dictionary<CameraMotionLayer, Vector3>();
        private readonly Dictionary<CameraMotionLayer, Vector3> rotationLayers = new Dictionary<CameraMotionLayer, Vector3>();
        private Vector3 baseLocalPosition;
        private Quaternion baseLocalRotation;
        private Vector3 currentPositionOffset;
        private Vector3 currentRotationOffset;

        private void Awake()
        {
            if (cameraRoot == null)
            {
                Camera cam = GetComponentInChildren<Camera>(true);
                if (cam != null) cameraRoot = cam.transform;
            }

            if (cameraRoot != null)
            {
                baseLocalPosition = cameraRoot.localPosition;
                baseLocalRotation = cameraRoot.localRotation;
            }
        }

        private void LateUpdate()
        {
            if (cameraRoot == null) return;

            Vector3 targetPosition = Vector3.zero;
            Vector3 targetRotation = Vector3.zero;
            foreach (Vector3 value in positionLayers.Values) targetPosition += value;
            foreach (Vector3 value in rotationLayers.Values) targetRotation += value;

            currentPositionOffset = Vector3.Lerp(currentPositionOffset, targetPosition, 1f - Mathf.Exp(-recoverySharpness * Time.deltaTime));
            currentRotationOffset = Vector3.Lerp(currentRotationOffset, targetRotation, 1f - Mathf.Exp(-rotationSharpness * Time.deltaTime));

            cameraRoot.localPosition = baseLocalPosition + currentPositionOffset;
            cameraRoot.localRotation = baseLocalRotation * Quaternion.Euler(currentRotationOffset);

            DecayLayers(Time.deltaTime);
        }

        public void AddImpulse(CameraMotionLayer layer, Vector3 positionImpulse, Vector3 rotationImpulse)
        {
            positionLayers[layer] = positionLayers.TryGetValue(layer, out Vector3 p) ? p + positionImpulse : positionImpulse;
            rotationLayers[layer] = rotationLayers.TryGetValue(layer, out Vector3 r) ? r + rotationImpulse : rotationImpulse;
        }

        public void SetContinuousLayer(CameraMotionLayer layer, Vector3 positionOffset, Vector3 rotationOffset)
        {
            positionLayers[layer] = positionOffset;
            rotationLayers[layer] = rotationOffset;
        }

        public void ClearLayer(CameraMotionLayer layer)
        {
            positionLayers[layer] = Vector3.zero;
            rotationLayers[layer] = Vector3.zero;
        }

        private void DecayLayers(float deltaTime)
        {
            CameraMotionLayer[] keys = new CameraMotionLayer[positionLayers.Count];
            positionLayers.Keys.CopyTo(keys, 0);
            foreach (CameraMotionLayer key in keys)
                positionLayers[key] = Vector3.Lerp(positionLayers[key], Vector3.zero, 10f * deltaTime);

            keys = new CameraMotionLayer[rotationLayers.Count];
            rotationLayers.Keys.CopyTo(keys, 0);
            foreach (CameraMotionLayer key in keys)
                rotationLayers[key] = Vector3.Lerp(rotationLayers[key], Vector3.zero, 10f * deltaTime);
        }
    }
}
