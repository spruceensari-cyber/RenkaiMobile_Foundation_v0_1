using UnityEngine;

namespace Renkai.Networking
{
    public sealed class LocalNetworkBridge : MonoBehaviour, INetworkBridge
    {
        public bool IsServerAuthoritative => false;

        public bool IsLocalPlayer(GameObject player) => true;

        public void RequestFire(Vector3 origin, Vector3 direction, double clientTime)
        {
            // Local prototype only. Replace with server-validated implementation.
        }

        public void RequestAbility(string abilityId, Vector3 targetPoint, double clientTime)
        {
            // Local prototype only. Replace with server-validated implementation.
        }
    }
}
