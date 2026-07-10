using UnityEngine;

namespace RenkaiMobile.Objective
{
    [RequireComponent(typeof(Collider))]
    public sealed class SpiritCoreSiteZone : MonoBehaviour
    {
        [SerializeField] private string siteId = "A";
        public string SiteId => siteId;

        private void Reset()
        {
            Collider c = GetComponent<Collider>();
            c.isTrigger = true;
        }

        public void Configure(string id)
        {
            siteId = id;
        }
    }
}
