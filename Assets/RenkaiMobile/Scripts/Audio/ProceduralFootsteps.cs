using UnityEngine;
using RenkaiMobile.Core;

namespace RenkaiMobile.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class ProceduralFootsteps : MonoBehaviour
    {
        [SerializeField] private AudioClip[] clips;
        [SerializeField] private float walkInterval = 0.48f;
        [SerializeField] private float runInterval = 0.32f;
        [SerializeField] private float runSpeedThreshold = 5.2f;
        [SerializeField] private float minPlanarSpeed = 0.8f;
        [SerializeField] private Vector2 pitchRange = new Vector2(0.94f, 1.06f);
        [SerializeField] private float soundEventRadius = 9f;
        [SerializeField] private MobileSoundEventBus soundBus;

        private AudioSource source;
        private MobileTeamMember team;
        private Vector3 lastPosition;
        private float planarSpeed;
        private float nextStep;

        private void Awake()
        {
            source = GetComponent<AudioSource>();
            team = GetComponent<MobileTeamMember>();
            if (soundBus == null) soundBus = FindFirstObjectByType<MobileSoundEventBus>();
            source.playOnAwake = false;
            source.spatialBlend = 1f;
            lastPosition = transform.position;
        }

        private void Update()
        {
            Vector3 delta = transform.position - lastPosition;
            delta.y = 0f;
            planarSpeed = Time.deltaTime > 0.0001f ? delta.magnitude / Time.deltaTime : 0f;
            lastPosition = transform.position;

            if (planarSpeed < minPlanarSpeed || Time.time < nextStep) return;
            float interval = planarSpeed >= runSpeedThreshold ? runInterval : walkInterval;
            nextStep = Time.time + interval;
            PlayStep();
        }

        private void PlayStep()
        {
            if (clips != null && clips.Length > 0 && source != null)
            {
                AudioClip clip = clips[Random.Range(0, clips.Length)];
                if (clip != null)
                {
                    source.pitch = Random.Range(pitchRange.x, pitchRange.y);
                    source.PlayOneShot(clip);
                }
            }

            soundBus?.Emit(new MobileSoundEvent(
                transform.position,
                soundEventRadius,
                MobileSoundType.Footstep,
                gameObject,
                team != null ? team.Team : MobileTeamId.None,
                Time.time));
        }
    }
}
