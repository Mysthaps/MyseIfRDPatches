using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GarbagePlugins
{
    [BepInPlugin("com.rhythmdr.garbageplugins", "Garbage Plugins", "1.0.0")]
    [BepInProcess("Rhythm Doctor.exe")]
    public class Plugin : BaseUnityPlugin
    {
        private static ConfigEntry<bool> configEnableOldSpeedChange;
        private static ConfigEntry<bool> configRankColorOnDoubleSpeed;

        private void Awake()
        {
            configEnableOldSpeedChange = Config.Bind("General", "EnableOldSpeedChange", true, "Changes chili/ice speed to be 2x/.5x, respectively");
            configRankColorOnDoubleSpeed = Config.Bind("General", "RankColorOnDoubleSpeed", true, "Changes rank screen color if EnableOldSpeedChange is enabled");
            
            if (configEnableOldSpeedChange.Value)
                Harmony.CreateAndPatchAll(typeof(EnableOldSpeedChange));

            if (configEnableOldSpeedChange.Value && configRankColorOnDoubleSpeed.Value)
                Harmony.CreateAndPatchAll(typeof(RankColorOnDoubleSpeed));

            Logger.LogInfo($"Plugin is loaded!");
        }

        private void OnDestroy()
        {
            Harmony.UnpatchAll();
        }

        private static class EnableOldSpeedChange
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(scnGame), "StartTheGame")]
            public static bool Prefix(ref float speed)
            {
                if (speed > 1f) speed = 2f;
                if (speed < 1f) speed = 0.5f;
                return true;
            }
        }

        private static class RankColorOnDoubleSpeed
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(HUD), "ShowAndSaveRank")]
            public static void Postfix(HUD __instance)
            {
                var levelType = __instance.game.currentLevel.levelType;
                if (levelType == LevelType.Boss || levelType == LevelType.Challenge) return;

                if (RDTime.speed > 1.5f) __instance.rank.color = new Color(1f, 0.2f, 0.2f);
                if (RDTime.speed < 0.75f) __instance.rank.color = new Color(0.2f, 0.2f, 1f);
            }
        }
    }
}
