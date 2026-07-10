using UnityEngine;
using Renkai.Combat;

namespace RenkaiMobile.Characters
{
    public sealed class ProceduralAgentMotion : MonoBehaviour
    {
        [SerializeField] private Transform torso;
        [SerializeField] private Transform leftArm;
        [SerializeField] private Transform rightArm;
        [SerializeField] private Transform leftLeg;
        [SerializeField] private Transform rightLeg;
        [SerializeField] private float strideSpeed = 7f;
        [SerializeField] private float strideAngle = 22f;
        [SerializeField] private float idleBob = 0.025f;

        private Vector3 lastPosition;
        private Vector3 torsoBase;
        private Health health;
        private float phase;

        public void Configure(Transform torsoRoot, Transform lArm, Transform rArm, Transform lLeg, Transform rLeg)
        {
            torso = torsoRoot;
            leftArm = lArm;
            rightArm = rArm;
            leftLeg = lLeg;
            rightLeg = rLeg;
        }

        private void Awake()
        {
            lastPosition = transform.position;
            if (torso != null) torsoBase = torso.localPosition;
            health = GetComponent<Health>();
        }

        private void Update()
        {
            if (health != null && !health.IsAlive) return;

            float speed = (transform.position - lastPosition).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
            lastPosition = transform.position;
            phase += Time.deltaTime * Mathf.Lerp(2f, strideSpeed, Mathf.Clamp01(speed));
            float swing = Mathf.Sin(phase) * strideAngle * Mathf.Clamp01(speed);

            if (leftArm != null) leftArm.localRotation = Quaternion.Euler(swing, 0f, 0f);
            if (rightArm != null) rightArm.localRotation = Quaternion.Euler(-swing, 0f, 0f);
            if (leftLeg != null) leftLeg.localRotation = Quaternion.Euler(-swing, 0f, 0f);
            if (rightLeg != null) rightLeg.localRotation = Quaternion.Euler(swing, 0f, 0f);
            if (torso != null) torso.localPosition = torsoBase + Vector3.up * (Mathf.Sin(phase * 0.5f) * idleBob);
        }
    }
}
