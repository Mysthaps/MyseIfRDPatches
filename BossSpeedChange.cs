using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace MyseIfRDPatches
{
    public class BossSpeedChange
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(HeartMonitor), "Show")]
        public static void Postfix(HeartMonitor __instance, SelectableCharacter character, Difficulty difficulty)
        {
            Rank levelRank = Persistence.GetLevelRank(character.levels[difficulty]);
            __instance.ToggleSpeed((int) levelRank != -1 && (int) levelRank != -2);
        }
    }
}