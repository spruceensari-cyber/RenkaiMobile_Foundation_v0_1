using System;
using System.Collections.Generic;
using UnityEngine;
using RenkaiMobile.Core;
using RenkaiMobile.Teams;

namespace RenkaiMobile.Spectate
{
    public sealed class TeamSpectateController : MonoBehaviour
    {
        [SerializeField] private Camera spectateCamera;
        [SerializeField] private MobileTeamId spectatedTeam = MobileTeamId.Attackers;
        [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 1.6f, -3.2f);
        [SerializeField] private float followSharpness = 8f;

        public event Action<FiveVFiveRosterAgent> TargetChanged;
        public bool IsSpectating { get; private set; }
        public FiveVFiveRosterAgent Current => CurrentTarget();

        private readonly List<FiveVFiveRosterAgent> candidates = new List<FiveVFiveRosterAgent>();
        private int index;

        private void Awake()
        {
            if (spectateCamera == null) spectateCamera = Camera.main;
            RefreshCandidates();
        }

        private void LateUpdate()
        {
            if (!IsSpectating) return;
            FiveVFiveRosterAgent target = CurrentTarget();
            if (target == null || spectateCamera == null) return;

            Vector3 desiredPosition = target.transform.TransformPoint(cameraOffset);
            spectateCamera.transform.position = Vector3.Lerp(
                spectateCamera.transform.position,
                desiredPosition,
                1f - Mathf.Exp(-followSharpness * Time.deltaTime));

            Vector3 lookPoint = target.transform.position + Vector3.up * 1.35f;
            Quaternion desiredRotation = Quaternion.LookRotation(lookPoint - spectateCamera.transform.position, Vector3.up);
            spectateCamera.transform.rotation = Quaternion.Slerp(
                spectateCamera.transform.rotation,
                desiredRotation,
                1f - Mathf.Exp(-followSharpness * Time.deltaTime));
        }

        public void BeginSpectating(MobileTeamId team)
        {
            spectatedTeam = team;
            IsSpectating = true;
            RefreshCandidates();
            TargetChanged?.Invoke(CurrentTarget());
        }

        public void EndSpectating()
        {
            IsSpectating = false;
        }

        public void RefreshCandidates()
        {
            candidates.Clear();
            FiveVFiveRosterAgent[] all = FindObjectsByType<FiveVFiveRosterAgent>(FindObjectsSortMode.None);
            foreach (FiveVFiveRosterAgent agent in all)
                if (agent != null && agent.Team == spectatedTeam && agent.IsAlive) candidates.Add(agent);
            index = Mathf.Clamp(index, 0, Mathf.Max(0, candidates.Count - 1));
        }

        public void NextTarget()
        {
            RefreshCandidates();
            if (candidates.Count == 0) return;
            index = (index + 1) % candidates.Count;
            TargetChanged?.Invoke(CurrentTarget());
        }

        public void PreviousTarget()
        {
            RefreshCandidates();
            if (candidates.Count == 0) return;
            index = (index - 1 + candidates.Count) % candidates.Count;
            TargetChanged?.Invoke(CurrentTarget());
        }

        private FiveVFiveRosterAgent CurrentTarget()
        {
            if (candidates.Count == 0) return null;
            if (index < 0 || index >= candidates.Count) index = 0;
            return candidates[index];
        }
    }
}
