using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace MyseIfRDPatches
{
    public static class ShowAccuracy
    {
        internal static int[] P1Hits = {0, 0, 0, 0, 0};
        internal static int[] P2Hits = {0, 0, 0, 0, 0};

        public static void AddAccuracy(RDPlayer player, double timeOffset)
        {
            int miliseconds = (int) Math.Abs(timeOffset * 1000);
            int num = miliseconds / 40 + 1;
            if (miliseconds <= 25) num = 0;
            switch (player){
                case RDPlayer.P1:
                    P1Hits[num >= 4 ? 4 : num]++;
                    break;
                case RDPlayer.P2:
                    P2Hits[num >= 4 ? 4 : num]++;
                    break;
                default:
                    break;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(scrPlayerbox), "Pulse")]
        public static bool Prefix(scrPlayerbox __instance, float timeOffset, bool CPUTriggered, bool bomb)
        {
            if (CPUTriggered || Time.frameCount == Row.lastHitFrame[(int) __instance.ent.row.playerProp.GetCurrentPlayer()]) return true;
            if (bomb) AddAccuracy(__instance.ent.row.playerProp.GetCurrentPlayer(), 999);
            else AddAccuracy(__instance.ent.row.playerProp.GetCurrentPlayer(), (double) timeOffset);
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(scrPlayerbox), "SpaceBarReleased")]
        public static bool Prefix(RDPlayer player, scrPlayerbox __instance, bool cpuTriggered) 
        {
            if (player != __instance.ent.row.playerProp.GetCurrentPlayer() || !__instance.beatBeingHeld || cpuTriggered) return true;
            double timeOffset = __instance.conductor.audioPos - __instance.beatReleaseTime;
            AddAccuracy(player, timeOffset);
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Beat), "Update")]
        public static bool Prefix(Beat __instance)
        {
            if (__instance.row.playerBox != null && __instance.row.ent != null && !__instance.row.dead
             && __instance.conductor.audioPos > __instance.inputTime + 0.4 && !__instance.playerDrives7thBeat 
             && !__instance.hasPulsed7thBeat && !__instance.bomb && !__instance.unhittable) AddAccuracy(__instance.row.playerProp.GetCurrentPlayer(), 999);
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(HUD), "ShowRankDescription")]
        public static void Postfix(HUD __instance)
        {
            if (__instance.game.currentLevel.levelType == LevelType.Boss) return;
            if (GC.showAbsoluteOffsets)
            {
                double P1Accuracy = (P1Hits[0] + P1Hits[1] + P1Hits[2] * 0.75 + P1Hits[3] * 0.5) / (P1Hits[0] + P1Hits[1] + P1Hits[2] + P1Hits[3] + P1Hits[4]) * 100;
                double P2Accuracy = (P2Hits[0] + P2Hits[1] + P2Hits[2] * 0.75 + P2Hits[3] * 0.5) / (P2Hits[0] + P2Hits[1] + P2Hits[2] + P2Hits[3] + P2Hits[4]) * 100;
                string SingleplayerResults = "";
                string P1Results = "";
                string P2Results = "";
                if (!GC.twoPlayerMode)
                {
                    SingleplayerResults = __instance.resultsSingleplayer.text + 
                        "\nAccuracy: " + Math.Round(P1Accuracy, 2).ToString("0.00") + "%" + 
                        (Main.configAccuracyMode.Value == Main.AccuracyOptions.ADOFAI && P1Hits[0] > 0 ? " + " + (P1Hits[0] * 0.01).ToString("0.00") + "%" : "");
                }
                else 
                {
                    P1Results = __instance.resultsP1.text + 
                        "\nAccuracy: " + Math.Round(P1Accuracy, 2).ToString("0.00") + "%" +
                        (Main.configAccuracyMode.Value == Main.AccuracyOptions.ADOFAI && P1Hits[0] > 0 ? " + " + (P1Hits[0] * 0.01).ToString("0.00") + "%" : "");
                    P2Results = __instance.resultsP2.text + 
                        "\nAccuracy: " + Math.Round(P2Accuracy, 2).ToString("0.00") + "%" +
                        (Main.configAccuracyMode.Value == Main.AccuracyOptions.ADOFAI && P2Hits[0] > 0 ? " + " + (P2Hits[0] * 0.01).ToString("0.00") + "%" : "");
                }
                __instance.resultsSingleplayer.text = SingleplayerResults;
                __instance.resultsP1.text = P1Results;
                __instance.resultsP2.text = P2Results;
            }
        }
    }
}