using UnityEngine;
using RenkaiMobile.Core;
using RenkaiMobile.Audio;

namespace RenkaiMobile.Objective
{
    [RequireComponent(typeof(Collider))]
    public sealed class ZodiacWorldItem : MonoBehaviour
    {
        [SerializeField] private float pickupCooldown = 0.35f;
        [SerializeField] private float bobAmplitude = 0.12f;
        [SerializeField] private float bobSpeed = 2.2f;
        [SerializeField] private float spinSpeed = 40f;
        [SerializeField] private MobileSoundEventBus soundBus;

        private float enabledAt;
        private Vector3 basePosition;

        private void Awake()
        {
            Collider c = GetComponent<Collider>();
            c.isTrigger = true;
            if (soundBus == null) soundBus = FindFirstObjectByType<MobileSoundEventBus>();
            basePosition = transform.position;
        }

        private void OnEnable()
        {
            enabledAt = Time.time;
            basePosition = transform.position;
        }

        private void Update()
        {
            transform.position = basePosition + Vector3.up * (Mathf.Sin(Time.time * bobSpeed) * bobAmplitude);
            transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Time.time - enabledAt < pickupCooldown) return;
            ZodiacCarrier carrier = other.GetComponentInParent<ZodiacCarrier>();
            if (carrier == null) return;

            MobileTeamMember member = carrier.GetComponent<MobileTeamMember>();
            if (member == null || member.Team != MobileTeamId.Attackers) return;

            carrier.GiveZodiac();
            soundBus?.Emit(new MobileSoundEvent(transform.position, 12f, MobileSoundType.ZodiacActivate, carrier.gameObject, member.Team, Time.time));
            Destroy(gameObject);
        }
    }
}
