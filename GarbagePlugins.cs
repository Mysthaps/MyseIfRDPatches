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
        private static ConfigEntry<bool> configDebugMode;
        private static ConfigEntry<ShowFpsOptions> configShowFPS;
        private static ConfigEntry<bool> configFiveFourteen;
        private static ConfigEntry<bool> configHMode;

        private enum ShowFpsOptions { Enabled, Legacy, Disabled }

        private void Awake()
        {
            configEnableOldSpeedChange = Config.Bind("General", "EnableOldSpeedChange", true, "Changes chili/ice speed to be 2x/.5x, respectively");
            configRankColorOnDoubleSpeed = Config.Bind("General", "RankColorOnDoubleSpeed", true, "Changes rank screen color if EnableOldSpeedChange is enabled");
            configDebugMode = Config.Bind("General", "DebugMode", false, "Enables Debug Mode");
            configShowFPS = Config.Bind("General", "ShowFPS", ShowFpsOptions.Enabled, "Shows FPS if Debug Mode is not active");
            configFiveFourteen = Config.Bind("Funny", "FiveFourteen", false, "514");
            configHMode = Config.Bind("Funny", "HMode", false, "Replaces \"Samurai.\" text with h");
            
            if (configEnableOldSpeedChange.Value)
                Harmony.CreateAndPatchAll(typeof(EnableOldSpeedChange));

            if (configEnableOldSpeedChange.Value && configRankColorOnDoubleSpeed.Value)
                Harmony.CreateAndPatchAll(typeof(RankColorOnDoubleSpeed));

            if (configDebugMode.Value)
                Harmony.CreateAndPatchAll(typeof(DebugMode));

            if (configShowFPS.Value != ShowFpsOptions.Disabled)
                Harmony.CreateAndPatchAll(typeof(ShowFPS));

            if (configFiveFourteen.Value)
                Harmony.CreateAndPatchAll(typeof(FiveFourteen));

            if (configHMode.Value)
                Harmony.CreateAndPatchAll(typeof(HMode));

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

        private static class DebugMode
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(scnBase), "Update")]
            public static void Postfix()
            {
                if (!RDC.debug) RDC.debug = true;
            }
        }

        private static class ShowFPS
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(scnGame), "Update")]
            public static void Postfix(scnGame __instance)
            {
                if (!RDC.debug){
                    __instance.debugText.gameObject.SetActive(true);

                    switch (configShowFPS.Value)
                    {
                        case ShowFpsOptions.Enabled:
                            if (scnGame.fps == 0.0) scnGame.fps = 1f / Time.unscaledDeltaTime;
                            scnGame.fps = scnGame.fps * 0.99f + (1f / Time.unscaledDeltaTime) * 0.01f;
                            __instance.debugText.text = "" + string.Format("<color=#ffffff>FPS:</color> <color={0}>{1}</color>", (object) (scnGame.fps < 30 ? "#ff0000" : (scnGame.fps < 50 ? "#ffff00" : "#00ff00")), (object) (int) scnGame.fps);
                            break;
                        case ShowFpsOptions.Legacy:
                            int num = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
                            __instance.debugText.text = "" + string.Format("<color=#ffffff>FPS:</color> <color={0}>{1}</color>", (object) (num < 30 ? "#ff0000" : (num < 50 ? "#ffff00" : "#00ff00")), (object) num);
                            break;
                    }
                    
                    __instance.currentLevel.Update();
                }
            }
        }

        private static class FiveFourteen
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(HUD), "ShowRankDescription")]
            public static void Postfix(HUD __instance)
            {
                string str = "Mistakes: 514\n514 early + 514 late = 514 offset frames";
                __instance.resultsSingleplayer.text = str;
                __instance.resultsSingleplayer.gameObject.SetActive(true);
                __instance.description.text = "514/514";
                __instance.description.gameObject.SetActive(true);
            }
        }

        private static class HMode
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(RDString), "Get")]
            public static void Postfix(ref string __result)
            {
                if (RDString.samuraiMode){
                    __result = "h";
                }
            }
        }
    }
}
