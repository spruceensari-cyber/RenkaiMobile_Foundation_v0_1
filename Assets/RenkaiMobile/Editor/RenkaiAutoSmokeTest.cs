using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using RenkaiMobile.Abilities;
using RenkaiMobile.Bots;
using RenkaiMobile.Combat;
using RenkaiMobile.Core;
using RenkaiMobile.Events;
using RenkaiMobile.Objective;
using RenkaiMobile.Rounds;
using RenkaiMobile.Spawning;
using RenkaiMobile.Stats;
using RenkaiMobile.Teams;
using RenkaiMobile.UI;
using RenkaiMobile.VFX;
using RenkaiMobile.Weapons;

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
            ValidateCombatStack(failures, warnings);
            ValidatePresentationStack(warnings);
            ValidateBotAndSpawnLayer(warnings);

            string summary = failures.Count == 0 ? "SMOKE TEST PASSED" : "SMOKE TEST FAILED";
            string details = failures.Count > 0 ? "\n\nFailures:\n- " + string.Join("\n- ", failures) : string.Empty;
            if (warnings.Count > 0) details += "\n\nWarnings:\n- " + string.Join("\n- ", warnings);

            string message = summary + details;
            if (failures.Count == 0) Debug.Log(message);
            else Debug.LogError(message);
            EditorUtility.DisplayDialog("Renkai Mobile Auto Test", message, "OK");
        }

        private static void ValidatePlayer(List<string> failures)
        {
            GameObject player = GameObject.Find("Player");
            if (player == null)
            {
                failures.Add("Player object missing");
                return;
            }

            MobileTeamMember team = player.GetComponent<MobileTeamMember>();
            if (team == null || team.Team != MobileTeamId.Attackers)
                failures.Add("Player must have MobileTeamMember on Attackers");

            if (player.GetComponent<MobileHealth>() == null)
                failures.Add("Player missing MobileHealth");
            if (player.GetComponent<FiveVFiveRosterAgent>() == null)
                failures.Add("Player missing FiveVFiveRosterAgent");
            if (player.GetComponent<MobileCombatantIdentity>() == null)
                failures.Add("Player missing MobileCombatantIdentity");
        }

        private static void ValidateRoster(List<string> failures, List<string> warnings)
        {
            FiveVFiveRosterAgent[] roster = Object.FindObjectsByType<FiveVFiveRosterAgent>(FindObjectsSortMode.None);
            int attackers = 0;
            int defenders = 0;
            int playerControlled = 0;

            foreach (FiveVFiveRosterAgent agent in roster)
            {
                if (agent.Team == MobileTeamId.Attackers) attackers++;
                if (agent.Team == MobileTeamId.Defenders) defenders++;
                if (agent.PlayerControlled) playerControlled++;
                if (agent.GetComponent<MobileHealth>() == null) failures.Add(agent.name + " missing MobileHealth");
                if (agent.GetComponent<MobileTeamMember>() == null) failures.Add(agent.name + " missing MobileTeamMember");
            }

            if (attackers != 5) failures.Add("Attackers roster must be exactly 5, found " + attackers);
            if (defenders != 5) failures.Add("Defenders roster must be exactly 5, found " + defenders);
            if (playerControlled != 1) failures.Add("Exactly one player-controlled roster slot expected, found " + playerControlled);
            if (roster.Length > 10) warnings.Add("More than 10 roster agents found; duplicate setup may exist");
        }

        private static void ValidateRoundSystems(List<string> failures)
        {
            if (Object.FindFirstObjectByType<MobileRoundManager>() == null) failures.Add("MobileRoundManager missing");
            if (Object.FindFirstObjectByType<FiveVFiveRoundDirector>() == null) failures.Add("FiveVFiveRoundDirector missing");
            if (Object.FindFirstObjectByType<MobileMatchStateController>() == null) failures.Add("MobileMatchStateController missing");
        }

        private static void ValidateObjectiveLayer(List<string> failures)
        {
            if (Object.FindFirstObjectByType<ZodiacObjective>() == null) failures.Add("ZodiacObjective missing");

            ZodiacZone[] zones = Object.FindObjectsByType<ZodiacZone>(FindObjectsSortMode.None);
            bool hasA = false;
            bool hasB = false;
            foreach (ZodiacZone zone in zones)
            {
                if (zone.ZoneId == "A") hasA = true;
                if (zone.ZoneId == "B") hasB = true;
            }

            if (!hasA) failures.Add("Zodiac Zone A missing");
            if (!hasB) failures.Add("Zodiac Zone B missing");
        }

        private static void ValidateCombatStack(List<string> failures, List<string> warnings)
        {
            if (Object.FindFirstObjectByType<MobileWeaponRouter>() == null) failures.Add("MobileWeaponRouter missing");
            if (Object.FindFirstObjectByType<MobileAdsController>() == null) failures.Add("MobileAdsController missing");
            if (Object.FindFirstObjectByType<MobileDamagePolicy>() == null) failures.Add("MobileDamagePolicy missing");
            if (Object.FindFirstObjectByType<AbilityRuntimeController>() == null) failures.Add("AbilityRuntimeController missing");
            if (Object.FindFirstObjectByType<MobileVfxPool>() == null) warnings.Add("MobileVfxPool missing");
            if (Object.FindFirstObjectByType<MobileKillEventBus>() == null) warnings.Add("MobileKillEventBus missing");
        }

        private static void ValidatePresentationStack(List<string> warnings)
        {
            if (Object.FindFirstObjectByType<MatchStatsTracker>() == null) warnings.Add("MatchStatsTracker missing");
            if (Object.FindFirstObjectByType<PremiumMatchHudLayer>() == null) warnings.Add("PremiumMatchHudLayer missing");
            if (Object.FindFirstObjectByType<ZodiacInteractionPresenter>() == null) warnings.Add("ZodiacInteractionPresenter missing");
        }

        private static void ValidateBotAndSpawnLayer(List<string> warnings)
        {
            if (Object.FindFirstObjectByType<MobileSpawnRegistry>() == null) warnings.Add("MobileSpawnRegistry missing");
            if (Object.FindFirstObjectByType<CoverRegistry>() == null) warnings.Add("CoverRegistry missing");
            if (Object.FindFirstObjectByType<BotNavigationAgent>() == null) warnings.Add("No BotNavigationAgent found");
        }
    }
}
