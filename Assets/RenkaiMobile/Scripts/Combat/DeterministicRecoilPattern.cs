using UnityEngine;

namespace RenkaiMobile.Combat
{
    public sealed class DeterministicRecoilPattern : MonoBehaviour
    {
        [SerializeField] private Vector2[] pattern =
        {
            new Vector2(0.05f, 0.35f),
            new Vector2(-0.08f, 0.42f),
            new Vector2(0.12f, 0.48f),
            new Vector2(0.18f, 0.52f),
            new Vector2(-0.16f, 0.55f),
            new Vector2(-0.22f, 0.58f),
            new Vector2(0.20f, 0.60f),
            new Vector2(0.28f, 0.62f)
        };
        [SerializeField] private float resetDelay = 0.22f;
        [SerializeField] private float adsMultiplier = 0.72f;

        private int shotIndex;
        private float lastShotTime = -999f;

        public Vector2 NextKick(bool ads)
        {
            if (Time.time - lastShotTime > resetDelay) shotIndex = 0;
            lastShotTime = Time.time;

            if (pattern == null || pattern.Length == 0)
                return Vector2.zero;

            Vector2 kick = pattern[Mathf.Min(shotIndex, pattern.Length - 1)];
            shotIndex++;
            return ads ? kick * adsMultiplier : kick;
        }

        public void ResetPattern()
        {
            shotIndex = 0;
            lastShotTime = -999f;
        }
    }
}
