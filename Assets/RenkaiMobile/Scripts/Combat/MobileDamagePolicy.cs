using UnityEngine;
using RenkaiMobile.Core;

namespace RenkaiMobile.Combat
{
    public sealed class MobileDamagePolicy : MonoBehaviour
    {
        [SerializeField] private bool friendlyFire;
        [SerializeField] private bool allowSelfDamage;

        public bool CanDamage(GameObject attacker, GameObject target, bool selfDamageOverride = false)
        {
            if (target == null) return false;
            if (attacker == null) return true;
            if (attacker == target)
                return selfDamageOverride || allowSelfDamage;

            MobileTeamMember attackerTeam = attacker.GetComponentInParent<MobileTeamMember>();
            MobileTeamMember targetTeam = target.GetComponentInParent<MobileTeamMember>();
            if (attackerTeam == null || targetTeam == null) return true;
            if (attackerTeam.Team == MobileTeamId.None || targetTeam.Team == MobileTeamId.None) return true;
            if (attackerTeam.Team != targetTeam.Team) return true;
            return friendlyFire;
        }
    }
}
