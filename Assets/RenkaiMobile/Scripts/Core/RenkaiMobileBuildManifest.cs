using UnityEngine;

namespace RenkaiMobile.Core
{
    public enum RenkaiMobileBuildFlavor
    {
        UnifiedMobileAdaptation
    }

    public sealed class RenkaiMobileBuildManifest : MonoBehaviour
    {
        [SerializeField] private RenkaiMobileBuildFlavor buildFlavor = RenkaiMobileBuildFlavor.UnifiedMobileAdaptation;
        [SerializeField] private RenkaiMobileIdentityCatalog identityCatalog;
        [SerializeField] private string sourceIdentity = "RENKAI";
        [SerializeField] private string mobileTitle = "RENKAI MOBILE";
        [SerializeField] private string controlProfile = "MOBILE_SIMPLIFIED";
        [SerializeField] private bool sharedAgents = true;
        [SerializeField] private bool sharedWeapons = true;
        [SerializeField] private bool sharedMaps = true;
        [SerializeField] private bool sharedObjectiveLanguage = true;

        public RenkaiMobileBuildFlavor BuildFlavor => buildFlavor;
        public RenkaiMobileIdentityCatalog IdentityCatalog => identityCatalog;
        public string SourceIdentity => sourceIdentity;
        public string MobileTitle => mobileTitle;
        public string ControlProfile => controlProfile;
        public bool SharedAgents => sharedAgents;
        public bool SharedWeapons => sharedWeapons;
        public bool SharedMaps => sharedMaps;
        public bool SharedObjectiveLanguage => sharedObjectiveLanguage;

        public void Configure(RenkaiMobileIdentityCatalog catalog)
        {
            identityCatalog = catalog;
        }
    }
}
