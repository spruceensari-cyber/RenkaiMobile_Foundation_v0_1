using UnityEngine;

namespace RenkaiMobile.Combat
{
    public sealed class RuntimeRecoil : MonoBehaviour
    {
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private RecoilProfile profile;
        private Vector2 target;
        private Vector2 current;
        private int shotIndex;

        public void Kick()
        {
            if (profile == null) return;
            float t = Mathf.Repeat(shotIndex / 24f, 1f);
            target.x += profile.VerticalKick.Evaluate(t);
            target.y += profile.HorizontalKick.Evaluate(t);
            shotIndex++;
        }

        public void ResetPattern() => shotIndex = 0;

        private void LateUpdate()
        {
            if (profile == null || cameraRoot == null) return;
            target = Vector2.Lerp(target, Vector2.zero, profile.RecoverySpeed * Time.deltaTime);
            current = Vector2.Lerp(current, target, 18f * Time.deltaTime);
            cameraRoot.localRotation *= Quaternion.Euler(-current.x, current.y, 0f);
        }
    }
}
