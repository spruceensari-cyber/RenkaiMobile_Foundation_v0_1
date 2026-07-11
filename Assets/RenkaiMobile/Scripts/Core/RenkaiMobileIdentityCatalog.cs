using System;
using UnityEngine;

namespace RenkaiMobile.Core
{
    [Serializable]
    public struct RenkaiMobileAgentIdentity
    {
        public string agentId;
        public string displayName;
        public string role;
        public string abilityOne;
        public string abilityTwo;
        public string signature;
    }

    [Serializable]
    public struct RenkaiMobileWeaponIdentity
    {
        public string weaponId;
        public string displayName;
        public string slot;
    }

    [CreateAssetMenu(menuName = "Renkai Mobile/Core/Identity Catalog", fileName = "RenkaiMobileIdentityCatalog")]
    public sealed class RenkaiMobileIdentityCatalog : ScriptableObject
    {
        [Header("Game Identity")]
        public string gameTitle = "RENKAI MOBILE";
        public string mapId = "kagami_district";
        public string mapDisplayName = "Kagami District";
        public string objectiveId = "zodiac";
        public string objectiveDisplayName = "Zodiac";

        [Header("Agents")]
        public RenkaiMobileAgentIdentity[] agents =
        {
            new RenkaiMobileAgentIdentity
            {
                agentId = "raika",
                displayName = "RAIKA",
                role = "DUELIST",
                abilityOne = "Velocity Surge",
                abilityTwo = "Arc Pulse",
                signature = "Overdrive"
            },
            new RenkaiMobileAgentIdentity
            {
                agentId = "akari",
                displayName = "AKARI",
                role = "INITIATOR",
                abilityOne = "Plasma Burst",
                abilityTwo = "Ignition Field",
                signature = "Solar Break"
            },
            new RenkaiMobileAgentIdentity
            {
                agentId = "kuroha",
                displayName = "KUROHA",
                role = "CONTROLLER",
                abilityOne = "Phase Step",
                abilityTwo = "Veil",
                signature = "Null Echo"
            }
        };

        [Header("Weapons")]
        public RenkaiMobileWeaponIdentity[] weapons =
        {
            new RenkaiMobileWeaponIdentity { weaponId = "kitsune_ar", displayName = "Kitsune AR", slot = "PRIMARY" },
            new RenkaiMobileWeaponIdentity { weaponId = "zero_pulse", displayName = "Zero Pulse", slot = "SECONDARY" },
            new RenkaiMobileWeaponIdentity { weaponId = "hikari_blade", displayName = "Hikari Blade", slot = "MELEE" }
        };

        public bool TryGetAgent(string agentId, out RenkaiMobileAgentIdentity result)
        {
            foreach (RenkaiMobileAgentIdentity agent in agents)
            {
                if (string.Equals(agent.agentId, agentId, StringComparison.OrdinalIgnoreCase))
                {
                    result = agent;
                    return true;
                }
            }

            result = default;
            return false;
        }
    }
}
