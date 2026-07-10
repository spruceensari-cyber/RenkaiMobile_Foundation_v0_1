using UnityEngine;

namespace RenkaiMobile.Objective
{
    [RequireComponent(typeof(Collider))]
    public sealed class ZodiacZone : MonoBehaviour
    {
        [SerializeField] private string zoneId = "A";
        public string ZoneId => zoneId;

        private void Reset()
        {
            Collider c = GetComponent<Collider>();
            c.isTrigger = true;
        }

        public void Configure(string id)
        {
            zoneId = string.IsNullOrWhiteSpace(id) ? "A" : id.Trim().ToUpperInvariant();
        }
    }
}
