using UnityEngine;

namespace RenkaiMobile.VFX
{
    public sealed class MobileVfxPoolItem : MonoBehaviour
    {
        public MobileVfxPool Owner { get; private set; }
        public string PoolKey { get; private set; }

        public void Initialize(MobileVfxPool owner, string poolKey)
        {
            Owner = owner;
            PoolKey = poolKey;
        }

        public void ReturnToPool()
        {
            Owner?.Release(this);
        }
    }
}
