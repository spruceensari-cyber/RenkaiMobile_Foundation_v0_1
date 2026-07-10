using UnityEngine;
using RenkaiMobile.Audio;
using RenkaiMobile.Core;

namespace RenkaiMobile.Bots
{
    [RequireComponent(typeof(MobileTeamMember), typeof(BotPerceptionMemory))]
    public sealed class BotPerception : MonoBehaviour
    {
        [SerializeField] private MobileSoundEventBus soundBus;
        [SerializeField] private float visionRange = 32f;
        [SerializeField, Range(20f, 180f)] private float fieldOfView = 105f;
        [SerializeField] private float visionScanInterval = 0.25f;
        [SerializeField] private LayerMask sightMask = ~0;

        private MobileTeamMember team;
        private BotPerceptionMemory memory;
        private float nextVisionScan;

        private void Awake()
        {
            team = GetComponent<MobileTeamMember>();
            memory = GetComponent<BotPerceptionMemory>();
            if (soundBus == null) soundBus = FindFirstObjectByType<MobileSoundEventBus>();
        }

        private void OnEnable()
        {
            if (soundBus != null) soundBus.SoundEmitted += OnSoundEmitted;
        }

        private void OnDisable()
        {
            if (soundBus != null) soundBus.SoundEmitted -= OnSoundEmitted;
        }

        private void Update()
        {
            if (Time.time < nextVisionScan) return;
            nextVisionScan = Time.time + visionScanInterval;
            ScanVision();
        }

        private void ScanVision()
        {
            MobileTeamMember[] all = FindObjectsByType<MobileTeamMember>(FindObjectsSortMode.None);
            float bestSqr = visionRange * visionRange;
            GameObject best = null;

            foreach (MobileTeamMember candidate in all)
            {
                if (candidate == null || candidate == team || candidate.Team == MobileTeamId.None || candidate.Team == team.Team) continue;
                Vector3 delta = candidate.transform.position - transform.position;
                float sqr = delta.sqrMagnitude;
                if (sqr > bestSqr) continue;
                if (Vector3.Angle(transform.forward, delta) > fieldOfView * 0.5f) continue;

                Vector3 origin = transform.position + Vector3.up * 1.4f;
                Vector3 target = candidate.transform.position + Vector3.up * 1.2f;
                Vector3 ray = target - origin;
                if (Physics.Raycast(origin, ray.normalized, out RaycastHit hit, ray.magnitude + 0.3f, sightMask, QueryTriggerInteraction.Ignore))
                {
                    if (hit.transform != candidate.transform && !hit.transform.IsChildOf(candidate.transform)) continue;
                }

                bestSqr = sqr;
                best = candidate.gameObject;
            }

            if (best != null)
            {
                memory.Remember(new BotKnownContact
                {
                    LastKnownPosition = best.transform.position,
                    LastSensedTime = Time.time,
                    Confidence = 1f,
                    SourceType = BotContactSourceType.Vision,
                    Target = best
                });
            }
        }

        private void OnSoundEmitted(MobileSoundEvent sound)
        {
            if (team == null) return;
            if (sound.Team != MobileTeamId.None && sound.Team == team.Team) return;
            float distance = Vector3.Distance(transform.position, sound.Position);
            if (distance > sound.Radius) return;

            float priority = SoundPriority(sound.SoundType);
            float confidence = Mathf.Clamp01(priority * (1f - distance / Mathf.Max(0.01f, sound.Radius)));
            if (memory.HasContact && memory.CurrentContact.Confidence > confidence) return;

            memory.Remember(new BotKnownContact
            {
                LastKnownPosition = sound.Position,
                LastSensedTime = Time.time,
                Confidence = confidence,
                SourceType = BotContactSourceType.Sound,
                Target = sound.Source,
                SoundType = sound.SoundType
            });
        }

        private static float SoundPriority(MobileSoundType soundType)
        {
            return soundType switch
            {
                MobileSoundType.ZodiacActivate => 1f,
                MobileSoundType.ZodiacDisrupt => 1f,
                MobileSoundType.Gunshot => 0.9f,
                MobileSoundType.Melee => 0.7f,
                MobileSoundType.Footstep => 0.55f,
                MobileSoundType.Decoy => 0.4f,
                _ => 0.5f
            };
        }
    }
}
