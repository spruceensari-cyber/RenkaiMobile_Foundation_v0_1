using UnityEngine;

namespace Renkai.Networking
{
    public interface INetworkBridge
    {
        bool IsServerAuthoritative { get; }
        bool IsLocalPlayer(GameObject player);
        void RequestFire(Vector3 origin, Vector3 direction, double clientTime);
        void RequestAbility(string abilityId, Vector3 targetPoint, double clientTime);
    }
}
