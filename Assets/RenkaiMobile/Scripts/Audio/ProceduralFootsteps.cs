using UnityEngine;
using Renkai.Movement;

namespace RenkaiMobile.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class ProceduralFootsteps : MonoBehaviour
    {
        [SerializeField] private MobileFpsMotor motor;
        [SerializeField] private AudioClip[] clips;
        [SerializeField] private float walkInterval = 0.48f;
        [SerializeField] private float runInterval = 0.32f;
        [SerializeField] private float runSpeedThreshold = 5.2f;
        [SerializeField] private float minPlanarSpeed = 0.8f;
        [SerializeField] private Vector2 pitchRange = new Vector2(0.94f, 1.06f);

        private AudioSource source;
        private float nextStep;

        private void Awake()
        {
            source = GetComponent<AudioSource>();
            if (motor == null) motor = GetComponent<MobileFpsMotor>();
            source.playOnAwake = false;
            source.spatialBlend = 1f;
        }

        private void Update()
        {
            if (motor == null || motor.PlanarSpeed < minPlanarSpeed) return;
            if (Time.time < nextStep) return;

            float interval = motor.PlanarSpeed >= runSpeedThreshold ? runInterval : walkInterval;
            nextStep = Time.time + interval;
            PlayStep();
        }

        private void PlayStep()
        {
            if (clips == null || clips.Length == 0 || source == null) return;
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            if (clip == null) return;
            source.pitch = Random.Range(pitchRange.x, pitchRange.y);
            source.PlayOneShot(clip);
        }
    }
}
