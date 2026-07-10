using UnityEngine;

namespace RenkaiMobile.Bots
{
    public enum BotNavigationIntentPriority
    {
        Patrol = 10,
        Rotate = 20,
        InvestigateSound = 30,
        Combat = 40,
        TakeCover = 50,
        ObjectiveCritical = 60
    }

    [RequireComponent(typeof(BotNavigationAgent))]
    public sealed class BotNavigationIntentController : MonoBehaviour
    {
        private BotNavigationAgent navigation;
        private BotNavigationIntentPriority activePriority;
        private float activeUntil;
        private bool hasIntent;

        public BotNavigationIntentPriority ActivePriority => activePriority;
        public bool HasIntent => hasIntent && Time.time <= activeUntil;

        private void Awake()
        {
            navigation = GetComponent<BotNavigationAgent>();
        }

        private void Update()
        {
            if (hasIntent && Time.time > activeUntil)
            {
                hasIntent = false;
                navigation.Stop();
            }
        }

        public bool Submit(Vector3 destination, BotNavigationIntentPriority priority, float holdSeconds)
        {
            if (hasIntent && Time.time <= activeUntil && priority < activePriority)
                return false;

            activePriority = priority;
            activeUntil = Time.time + Mathf.Max(0.1f, holdSeconds);
            hasIntent = true;
            navigation.SetDestination(destination);
            return true;
        }

        public void Clear(BotNavigationIntentPriority minimumPriority)
        {
            if (!hasIntent || activePriority < minimumPriority) return;
            hasIntent = false;
            navigation.Stop();
        }
    }
}
