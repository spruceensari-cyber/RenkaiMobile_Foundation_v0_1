using UnityEngine;

namespace RenkaiMobile.Abilities
{
    public enum AgentKitId
    {
        Raika,
        Akari,
        Kuroha
    }

    public sealed class AgentAbilityDispatcher : MonoBehaviour
    {
        [SerializeField] private AgentKitId kit = AgentKitId.Raika;
        [SerializeField] private RaikaAbilityKit raika;
        [SerializeField] private AkariAbilityKit akari;
        [SerializeField] private KurohaAbilityKit kuroha;

        private void Awake()
        {
            if (raika == null) raika = GetComponent<RaikaAbilityKit>();
            if (akari == null) akari = GetComponent<AkariAbilityKit>();
            if (kuroha == null) kuroha = GetComponent<KurohaAbilityKit>();
        }

        public void UseSlot(int slot)
        {
            slot = Mathf.Clamp(slot, 0, 2);
            switch (kit)
            {
                case AgentKitId.Raika:
                    if (slot == 0) raika?.UseVelocitySurge();
                    else if (slot == 1) raika?.UseArcPulse();
                    else raika?.UseOverdrive();
                    break;
                case AgentKitId.Akari:
                    if (slot == 0) akari?.UsePlasmaBurst();
                    else if (slot == 1) akari?.UseIgnitionField();
                    else akari?.UseSolarBreak();
                    break;
                case AgentKitId.Kuroha:
                    if (slot == 0) kuroha?.UsePhaseStep();
                    else if (slot == 1) kuroha?.UseVeil();
                    else kuroha?.UseNullEcho();
                    break;
            }
        }
    }
}
