using System.Collections.Generic;
using UnityEngine;

namespace RenkaiMobile.VFX
{
    public sealed class MobileVfxPool : MonoBehaviour
    {
        [SerializeField] private int defaultPrewarm = 8;
        [SerializeField] private int maxPerKey = 24;

        private readonly Dictionary<string, Queue<MobileVfxPoolItem>> available = new Dictionary<string, Queue<MobileVfxPoolItem>>();
        private readonly Dictionary<string, int> totalCounts = new Dictionary<string, int>();

        public void Prewarm(string key, GameObject prefab, int count = -1)
        {
            if (string.IsNullOrEmpty(key) || prefab == null) return;
            int target = count > 0 ? count : defaultPrewarm;
            EnsureQueue(key);
            while (totalCounts[key] < target && totalCounts[key] < maxPerKey)
            {
                MobileVfxPoolItem item = CreateItem(key, prefab);
                item.gameObject.SetActive(false);
                available[key].Enqueue(item);
            }
        }

        public MobileVfxPoolItem Spawn(string key, GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (string.IsNullOrEmpty(key) || prefab == null) return null;
            EnsureQueue(key);

            MobileVfxPoolItem item = null;
            while (available[key].Count > 0 && item == null)
                item = available[key].Dequeue();

            if (item == null)
            {
                if (totalCounts[key] >= maxPerKey) return null;
                item = CreateItem(key, prefab);
            }

            item.transform.SetPositionAndRotation(position, rotation);
            item.gameObject.SetActive(true);
            return item;
        }

        public void Release(MobileVfxPoolItem item)
        {
            if (item == null || string.IsNullOrEmpty(item.PoolKey)) return;
            EnsureQueue(item.PoolKey);
            item.gameObject.SetActive(false);
            item.transform.SetParent(transform, false);
            available[item.PoolKey].Enqueue(item);
        }

        private MobileVfxPoolItem CreateItem(string key, GameObject prefab)
        {
            GameObject go = Instantiate(prefab, transform);
            go.name = prefab.name + "_Pooled";
            MobileVfxPoolItem item = go.GetComponent<MobileVfxPoolItem>();
            if (item == null) item = go.AddComponent<MobileVfxPoolItem>();
            item.Initialize(this, key);
            totalCounts[key] = totalCounts.TryGetValue(key, out int count) ? count + 1 : 1;
            return item;
        }

        private void EnsureQueue(string key)
        {
            if (!available.ContainsKey(key)) available[key] = new Queue<MobileVfxPoolItem>();
            if (!totalCounts.ContainsKey(key)) totalCounts[key] = 0;
        }
    }
}
