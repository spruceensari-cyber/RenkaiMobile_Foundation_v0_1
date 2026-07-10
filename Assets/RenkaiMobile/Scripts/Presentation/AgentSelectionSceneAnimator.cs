using System.Collections;
using UnityEngine;
using RenkaiMobile.Agents;

namespace RenkaiMobile.Presentation
{
    public sealed class AgentSelectionSceneAnimator : MonoBehaviour
    {
        [SerializeField] private AgentSelectionController selection;
        [SerializeField] private Transform cameraRig;
        [SerializeField] private Transform previewFocus;
        [SerializeField] private Light keyLight;
        [SerializeField] private float transitionSeconds = 0.45f;
        [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 1.5f, -4.2f);

        private Coroutine routine;

        private void Awake()
        {
            if (selection == null) selection = FindFirstObjectByType<AgentSelectionController>();
        }

        private void OnEnable()
        {
            if (selection != null) selection.SelectionChanged += OnSelectionChanged;
        }

        private void OnDisable()
        {
            if (selection != null) selection.SelectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged(AgentDefinition agent, int index)
        {
            if (routine != null) StopCoroutine(routine);
            routine = StartCoroutine(Transition(agent, index));
        }

        private IEnumerator Transition(AgentDefinition agent, int index)
        {
            if (agent == null) yield break;
            Vector3 startPos = cameraRig != null ? cameraRig.position : Vector3.zero;
            Quaternion startRot = cameraRig != null ? cameraRig.rotation : Quaternion.identity;
            Vector3 targetPos = previewFocus != null ? previewFocus.TransformPoint(cameraOffset + Vector3.right * ((index - 1) * 0.22f)) : startPos;
            Quaternion targetRot = previewFocus != null ? Quaternion.LookRotation((previewFocus.position + Vector3.up * 1.25f) - targetPos, Vector3.up) : startRot;

            float t = 0f;
            while (t < transitionSeconds)
            {
                t += Time.unscaledDeltaTime;
                float eased = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / transitionSeconds));
                if (cameraRig != null)
                {
                    cameraRig.position = Vector3.Lerp(startPos, targetPos, eased);
                    cameraRig.rotation = Quaternion.Slerp(startRot, targetRot, eased);
                }
                if (keyLight != null) keyLight.color = Color.Lerp(keyLight.color, agent.primaryColor, eased);
                yield return null;
            }

            routine = null;
        }
    }
}
