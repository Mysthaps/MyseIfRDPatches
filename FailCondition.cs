using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace MyseIfRDPatches
{
    public class FailCondition
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(scrRowEntities), "CrackAdvance")]
        public static void Postfix(scrRowEntities __instance)
        {
            if ((Main.configFailCondition.Value == Main.FailConditionOptions.Heartbreak && __instance.crackCounter >= __instance.game.currentLevel.missesToCrackHeart) ||
                Main.configFailCondition.Value == Main.FailConditionOptions.Perfect)
            {
                try {
                    __instance.game.FailLevel(__instance);
                }
                catch {
                    __instance.game.LevelFailSequence(__instance);
                }
            }
        }
    }
}