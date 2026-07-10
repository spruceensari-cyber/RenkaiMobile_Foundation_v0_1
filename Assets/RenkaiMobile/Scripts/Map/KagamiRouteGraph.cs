using System;
using System.Collections.Generic;
using UnityEngine;

namespace RenkaiMobile.Map
{
    public enum KagamiLocationId
    {
        AttackerSpawn,
        AMain,
        AShort,
        AZodiacZone,
        Mid,
        MidConnector,
        BMain,
        BShort,
        BZodiacZone,
        DefenderConnector,
        DefenderSpawn
    }

    [Serializable]
    public sealed class KagamiRouteNode
    {
        public KagamiLocationId id;
        public Transform anchor;
        public KagamiLocationId[] neighbors = Array.Empty<KagamiLocationId>();
    }

    public sealed class KagamiRouteGraph : MonoBehaviour
    {
        [SerializeField] private KagamiRouteNode[] nodes = Array.Empty<KagamiRouteNode>();
        private readonly Dictionary<KagamiLocationId, KagamiRouteNode> lookup = new Dictionary<KagamiLocationId, KagamiRouteNode>();

        private void Awake()
        {
            RebuildLookup();
        }

        public void RebuildLookup()
        {
            lookup.Clear();
            foreach (KagamiRouteNode node in nodes)
                if (node != null) lookup[node.id] = node;
        }

        public bool TryGetPosition(KagamiLocationId id, out Vector3 position)
        {
            position = Vector3.zero;
            if (!lookup.TryGetValue(id, out KagamiRouteNode node) || node.anchor == null) return false;
            position = node.anchor.position;
            return true;
        }

        public IReadOnlyList<KagamiLocationId> GetNeighbors(KagamiLocationId id)
        {
            if (!lookup.TryGetValue(id, out KagamiRouteNode node) || node.neighbors == null)
                return Array.Empty<KagamiLocationId>();
            return node.neighbors;
        }
    }
}
