using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Renkai.Core;
using Renkai.Rounds;
using RenkaiMobile.Objective;
using RenkaiMobile.Teams;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiAutoSmokeTest
    {
        [MenuItem("Renkai Mobile/Run Auto Smoke Test")]
        public static void Run()
        {
            var failures = new List<string>();
            var warnings = new List<string>();

            ValidatePlayer(failures);
            ValidateRoster(failures, warnings);
            ValidateRoundSystems(failures);
            ValidateObjectiveLayer(failures);
            ValidateHud(warnings);

            if (failures.Count == 0)
            {
                string message = "SMOKE TEST PASSED\n\n" +
                                 "Player: OK\n" +
                                 "5v5 roster: OK\n" +
                                 "Round system: OK\n" +
                                 "A/B sites: OK\n" +
                                 "Spirit Core: OK\n" +
                                 (warnings.Count > 0 ? "\nWarnings:\n- " + string.Join("\n- ", warnings) : "");
                Debug.Log(message);
                EditorUtility.DisplayDialog("Renkai Mobile Auto Test", message, "OK");
            }
            else
            {
                string message = "SMOKE TEST FAILED\n\n- " + string.Join("\n- ", failures);
                if (warnings.Count > 0) message += "\n\nWarnings:\n- " + string.Join("\n- ", warnings);
                Debug.LogError(message);
                EditorUtility.DisplayDialog("Renkai Mobile Auto Test", message, "OK");
            }
        }

        private static void ValidatePlayer(List<string> failures)
        {
            GameObject player = GameObject.Find("Player");
            if (player == null)
            {
                failures.Add("Player object missing");
                return;
            }

            TeamMember team = player.GetComponent<TeamMember>();
            if (team == null || team.Team != TeamId.Attackers)
                failures.Add("Player must belong to Attackers");

            if (player.GetComponent<FiveVFiveRosterAgent>() == null)
                failures.Add("Player missing FiveVFiveRosterAgent");
        }

        private static void ValidateRoster(List<string> failures, List<string> warnings)
        {
            FiveVFiveRosterAgent[] roster = Object.FindObjectsByType<FiveVFiveRosterAgent>(FindObjectsSortMode.None);
            int attackers = 0;
            int defenders = 0;
            int playerControlled = 0;

            foreach (FiveVFiveRosterAgent agent in roster)
            {
                if (agent.Team == TeamId.Attackers) attackers++;
                if (agent.Team == TeamId.Defenders) defenders++;
                if (agent.PlayerControlled) playerControlled++;
            }

            if (attackers != 5) failures.Add("Attackers roster must be exactly 5, found " + attackers);
            if (defenders != 5) failures.Add("Defenders roster must be exactly 5, found " + defenders);
            if (playerControlled != 1) failures.Add("Exactly one player-controlled roster slot expected, found " + playerControlled);
            if (roster.Length > 10) warnings.Add("More than 10 roster agents found; duplicate setup may exist");
        }

        private static void ValidateRoundSystems(List<string> failures)
        {
            RoundManager round = Object.FindFirstObjectByType<RoundManager>();
            if (round == null) failures.Add("RoundManager missing");

            if (Object.FindFirstObjectByType<RenkaiMobile.Rounds.FiveVFiveRoundDirector>() == null)
                failures.Add("FiveVFiveRoundDirector missing");
        }

        private static void ValidateObjectiveLayer(List<string> failures)
        {
            SpiritCoreRoundObjective objective = Object.FindFirstObjectByType<SpiritCoreRoundObjective>();
            if (objective == null) failures.Add("SpiritCoreRoundObjective missing");

            SpiritCoreSiteZone[] sites = Object.FindObjectsByType<SpiritCoreSiteZone>(FindObjectsSortMode.None);
            bool hasA = false;
            bool hasB = false;
            foreach (SpiritCoreSiteZone site in sites)
            {
                if (site.SiteId == "A") hasA = true;
                if (site.SiteId == "B") hasB = true;
            }

            if (!hasA) failures.Add("Site A missing");
            if (!hasB) failures.Add("Site B missing");
        }

        private static void ValidateHud(List<string> warnings)
        {
            if (Object.FindFirstObjectByType<RenkaiMobile.UI.TacticalRoundHud>() == null)
                warnings.Add("TacticalRoundHud missing");
        }
    }
}
