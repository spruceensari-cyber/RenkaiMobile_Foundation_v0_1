using System.Collections;
using UnityEngine;
using RenkaiMobile.Weapons;

namespace RenkaiMobile.VFX
{
    public sealed class MobileWeaponVfxController : MonoBehaviour
    {
        [SerializeField] private MobileRifleController rifle;
        [SerializeField] private Transform muzzle;
        [SerializeField] private float muzzleFlashSeconds = 0.045f;
        [SerializeField] private float tracerSeconds = 0.06f;
        [SerializeField] private float impactSeconds = 0.22f;
        [SerializeField] private Color muzzleColor = new Color(0.2f, 0.9f, 1f, 1f);
        [SerializeField] private Color impactColor = new Color(0.8f, 0.2f, 1f, 1f);

        private void Awake()
        {
            if (rifle == null) rifle = GetComponent<MobileRifleController>();
        }

        private void OnEnable()
        {
            if (rifle != null) rifle.ShotFired += OnShotFired;
        }

        private void OnDisable()
        {
            if (rifle != null) rifle.ShotFired -= OnShotFired;
        }

        private void OnShotFired(MobileWeaponShot shot)
        {
            Vector3 origin = muzzle != null ? muzzle.position : shot.Origin;
            StartCoroutine(MuzzleFlash(origin));
            StartCoroutine(Tracer(origin, shot.EndPoint));
            if (shot.Hit) StartCoroutine(Impact(shot.EndPoint, shot.Headshot));
        }

        private IEnumerator MuzzleFlash(Vector3 position)
        {
            GameObject flash = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            flash.name = "Mobile_MuzzleFlash";
            flash.transform.position = position;
            flash.transform.localScale = Vector3.one * 0.12f;
            Collider c = flash.GetComponent<Collider>();
            if (c != null) Destroy(c);
            SetColor(flash.GetComponent<Renderer>(), muzzleColor);
            yield return new WaitForSeconds(muzzleFlashSeconds);
            if (flash != null) Destroy(flash);
        }

        private IEnumerator Tracer(Vector3 start, Vector3 end)
        {
            GameObject tracer = new GameObject("Mobile_Tracer");
            LineRenderer line = tracer.AddComponent<LineRenderer>();
            line.positionCount = 2;
            line.SetPosition(0, start);
            line.SetPosition(1, end);
            line.startWidth = 0.025f;
            line.endWidth = 0.008f;
            Shader shader = Shader.Find("Sprites/Default");
            line.material = new Material(shader);
            line.startColor = muzzleColor;
            line.endColor = new Color(muzzleColor.r, muzzleColor.g, muzzleColor.b, 0f);
            yield return new WaitForSeconds(tracerSeconds);
            if (tracer != null) Destroy(tracer);
        }

        private IEnumerator Impact(Vector3 position, bool headshot)
        {
            GameObject impact = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            impact.name = headshot ? "Headshot_Fracture" : "Impact_Fracture";
            impact.transform.position = position;
            impact.transform.localScale = Vector3.one * (headshot ? 0.18f : 0.1f);
            Collider c = impact.GetComponent<Collider>();
            if (c != null) Destroy(c);
            SetColor(impact.GetComponent<Renderer>(), headshot ? Color.white : impactColor);

            float t = 0f;
            Vector3 start = impact.transform.localScale;
            while (impact != null && t < impactSeconds)
            {
                t += Time.deltaTime;
                impact.transform.localScale = Vector3.Lerp(start, start * 2.4f, t / impactSeconds);
                yield return null;
            }
            if (impact != null) Destroy(impact);
        }

        private static void SetColor(Renderer renderer, Color color)
        {
            if (renderer == null) return;
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Unlit/Color");
            Material material = new Material(shader);
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            renderer.material = material;
        }
    }
}
