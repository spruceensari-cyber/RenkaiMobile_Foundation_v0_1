using UnityEngine;

namespace RenkaiMobile.Core
{
    public sealed class MobileTeamMember : MonoBehaviour
    {
        [SerializeField] private MobileTeamId team = MobileTeamId.None;
        public MobileTeamId Team => team;

        public void SetTeam(MobileTeamId value)
        {
            team = value;
        }
    }
}
