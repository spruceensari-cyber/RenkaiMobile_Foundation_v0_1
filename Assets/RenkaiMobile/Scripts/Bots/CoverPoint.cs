using UnityEngine;

namespace RenkaiMobile.Bots
{
    public enum CoverHeightClass
    {
        Low,
        High
    }

    public sealed class CoverPoint : MonoBehaviour
    {
        [SerializeField] private CoverHeightClass heightClass = CoverHeightClass.High;
        [SerializeField] private bool canPeekLeft = true;
        [SerializeField] private bool canPeekRight = true;
        [SerializeField] private bool crouchCompatible = true;
        [SerializeField, Range(0f, 1f)] private float dangerScore;

        public CoverHeightClass HeightClass => heightClass;
        public bool CanPeekLeft => canPeekLeft;
        public bool CanPeekRight => canPeekRight;
        public bool CrouchCompatible => crouchCompatible;
        public float DangerScore => dangerScore;
        public bool Occupied { get; private set; }
        public GameObject Occupant { get; private set; }

        public bool TryOccupy(GameObject occupant)
        {
            if (Occupied || occupant == null) return false;
            Occupied = true;
            Occupant = occupant;
            return true;
        }

        public void Release(GameObject occupant)
        {
            if (!Occupied || Occupant != occupant) return;
            Occupied = false;
            Occupant = null;
        }
    }
}
