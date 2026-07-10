using UnityEngine;

namespace RenkaiMobile.Combat
{
    public sealed class MobileCameraImpulseAdapter : MonoBehaviour
    {
        [SerializeField] private MobileCameraMotionStack motionStack;

        private void Awake()
        {
            if (motionStack == null) motionStack = FindFirstObjectByType<MobileCameraMotionStack>();
        }

        public void Recoil(Vector2 kick)
        {
            motionStack?.AddImpulse(CameraMotionLayer.Recoil, Vector3.zero, new Vector3(-kick.y, kick.x, kick.x * 0.2f));
        }

        public void Landing(float strength)
        {
            motionStack?.AddImpulse(CameraMotionLayer.Landing, new Vector3(0f, -0.03f * strength, 0f), new Vector3(1.2f * strength, 0f, 0f));
        }

        public void Damage(Vector3 direction, float strength)
        {
            Vector3 local = transform.InverseTransformDirection(direction.normalized);
            motionStack?.AddImpulse(CameraMotionLayer.Damage, Vector3.zero, new Vector3(-local.z, local.x, -local.x) * strength);
        }

        public void Ability(float strength)
        {
            motionStack?.AddImpulse(CameraMotionLayer.Ability, new Vector3(0f, 0.01f * strength, -0.015f * strength), new Vector3(-0.8f, 0.4f, 0.2f) * strength);
        }
    }
}
