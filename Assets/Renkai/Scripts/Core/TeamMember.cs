using UnityEngine;

namespace Renkai.Core
{
    public sealed class TeamMember : MonoBehaviour
    {
        [SerializeField] private TeamId team = TeamId.None;
        public TeamId Team => team;

        public void SetTeam(TeamId value) => team = value;
    }
}
