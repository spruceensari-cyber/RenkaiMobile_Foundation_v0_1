using System.Collections;
using UnityEngine;
using RenkaiMobile.Weapons;

namespace RenkaiMobile.VFX
{
    public sealed class HikariBladeVfxController : MonoBehaviour
    {
        [SerializeField] private MobileMeleeController melee;
        [SerializeField] private Transform bladeRoot;
        [SerializeField] private Color slashColor = new Color(0.2f, 0.9f, 1f, 0.95f);
        [SerializeField] private float trailLifetime = 0.18f;

        public void PlaySlash()
        {
            StartCoroutine(SlashRoutine());
        }

        private IEnumerator SlashRoutine()
        {
            Transform root = bladeRoot != null ? bladeRoot : transform;
            GameObject trail = new GameObject("Hikari_SlashTrail");
            LineRenderer line = trail.AddComponent<LineRenderer>();
            line.positionCount = 3;
            Vector3 p = root.position;
            Vector3 f = root.forward;
            Vector3 r = root.right;
            line.SetPosition(0, p - r * 0.45f);
            line.SetPosition(1, p + f * 1.2f);
            line.SetPosition(2, p + r * 0.45f);
            line.startWidth = 0.11f;
            line.endWidth = 0.02f;
            Shader shader = Shader.Find("Sprites/Default");
            line.material = new Material(shader);
            line.startColor = slashColor;
            line.endColor = new Color(slashColor.r, slashColor.g, slashColor.b, 0f);
            yield return new WaitForSeconds(trailLifetime);
            if (trail != null) Destroy(trail);
        }
    }
}
