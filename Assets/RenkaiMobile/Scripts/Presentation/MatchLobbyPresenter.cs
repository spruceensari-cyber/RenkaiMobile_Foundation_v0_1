using UnityEngine;
using UnityEngine.UI;

namespace RenkaiMobile.Presentation
{
    public sealed class MatchLobbyPresenter : MonoBehaviour
    {
        [SerializeField] private MatchLobbyController lobby;
        [SerializeField] private Text readinessText;
        [SerializeField] private Text statusText;
        [SerializeField] private CanvasGroup launchPulse;
        [SerializeField] private float pulseSpeed = 4f;

        private bool readyToLaunch;

        private void Awake()
        {
            if (lobby == null) lobby = FindFirstObjectByType<MatchLobbyController>();
        }

        private void OnEnable()
        {
            if (lobby == null) return;
            lobby.ReadyCountChanged += OnReadyCountChanged;
            lobby.AllPlayersReady += OnAllPlayersReady;
        }

        private void OnDisable()
        {
            if (lobby == null) return;
            lobby.ReadyCountChanged -= OnReadyCountChanged;
            lobby.AllPlayersReady -= OnAllPlayersReady;
        }

        private void Update()
        {
            if (launchPulse == null) return;
            launchPulse.alpha = readyToLaunch
                ? 0.55f + 0.35f * (0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * pulseSpeed))
                : 0f;
        }

        private void OnReadyCountChanged(int ready, int total)
        {
            if (readinessText != null) readinessText.text = "LINKED // " + ready + " / " + total;
            if (statusText != null) statusText.text = readyToLaunch ? "RESONANCE READY" : "WAITING FOR LINKS";
        }

        private void OnAllPlayersReady()
        {
            readyToLaunch = true;
            if (statusText != null) statusText.text = "RESONANCE READY";
        }
    }
}
