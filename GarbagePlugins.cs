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
    [BepInPlugin("com.rhythmdr.garbageplugins", "Garbage Plugins", "1.3.2")]
    [BepInProcess("Rhythm Doctor.exe")]
    public class Plugin : BaseUnityPlugin
    {
        private static ConfigEntry<bool> configEnableOldSpeedChange;
        private static ConfigEntry<bool> configRankColorOnDoubleSpeed;
        private static ConfigEntry<bool> configEnableBossSpeedChange;
        private static ConfigEntry<ShowFpsOptions> configShowFPS;
        private static ConfigEntry<bool> configFiveFourteen;
        private static ConfigEntry<bool> configSamuHrai;
        private static ConfigEntry<bool> configShowAccuracy;

        private enum ShowFpsOptions { Enabled, Legacy, Disabled }

        private void Awake()
        {
            configEnableOldSpeedChange = Config.Bind("Speed Modifiers", "EnableOldSpeedChange", false, "Changes chili/ice speed to be 2x/.5x, respectively");
            configRankColorOnDoubleSpeed = Config.Bind("Speed Modifiers", "RankColorOnDoubleSpeed", false, "Changes rank screen color if EnableOldSpeedChange is enabled");
            configEnableBossSpeedChange = Config.Bind("Speed Modifiers", "EnableBossSpeedChange", false, "Allows the speed of boss levels to be changed");
            configShowFPS = Config.Bind("General", "ShowFPS", ShowFpsOptions.Disabled, "Adds an FPS counter on the top right of the screen while in a level.");
            configFiveFourteen = Config.Bind("Funny", "FiveFourteen", false, "514");
            configSamuHrai = Config.Bind("Funny", "SamuHrai", false, "Replaces \"Samurai.\" text with \"h\"");
            configShowAccuracy = Config.Bind("General", "ShowAccuracy", false, "Adds an accuracy count at the end of a level if Detailed Level Results is enabled.\nDoes not work for 2P.");
            
            if (configEnableOldSpeedChange.Value)
                Harmony.CreateAndPatchAll(typeof(EnableOldSpeedChange));

            if (configEnableOldSpeedChange.Value && configRankColorOnDoubleSpeed.Value)
                Harmony.CreateAndPatchAll(typeof(RankColorOnDoubleSpeed));

            if (configEnableBossSpeedChange.Value)
                Harmony.CreateAndPatchAll(typeof(EnableBossSpeedChange));

            if (configShowFPS.Value != ShowFpsOptions.Disabled)
                Harmony.CreateAndPatchAll(typeof(ShowFPS));

            if (configFiveFourteen.Value)
                Harmony.CreateAndPatchAll(typeof(FiveFourteen));

            if (configSamuHrai.Value)
                Harmony.CreateAndPatchAll(typeof(SamuHrai));

            if (configShowAccuracy.Value)
                Harmony.CreateAndPatchAll(typeof(ShowAccuracy));

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

        private static class EnableBossSpeedChange
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(HeartMonitor), "Show")]
            public static void Postfix(HeartMonitor __instance, SelectableCharacter character, Difficulty difficulty)
            {
                Rank levelRank = Persistence.GetLevelRank(character.levels[difficulty]);
                __instance.ToggleSpeed((int) levelRank != -1 && (int) levelRank != -2);
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

        private static class SamuHrai
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

        private static class ShowAccuracy
        {
            // i'd like to apologize for this awful code
            private static int[] hits = {0, 0, 0, 0, 0, 0}; // 40, 80, 120, 160, 200, miss

            [HarmonyTranspiler]
            [HarmonyPatch(typeof(scnGame), "Start")]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                return new CodeMatcher(instructions)
                    .MatchForward(false,
                        new CodeMatch(OpCodes.Ldstr, "Level_"))
                    .Advance(3)
                    .InsertAndAdvance(
                        new CodeInstruction(OpCodes.Ldstr, ", Assembly-CSharp"),
                        new CodeInstruction(OpCodes.Call, AccessTools.Method("System.String:Concat", new Type[] { typeof(String), typeof(String) })))
                    .InstructionEnumeration();
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(scnGame), "Start")]
            public static void Prefix()
            {
                hits = new int[] {0, 0, 0, 0, 0, 0};
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(scrPlayerbox), "Pulse")]
            public static void Postfix(float timeOffset, bool CPUTriggered = false, bool bomb = false)
            {
                if (!CPUTriggered){
                    int num = Math.Abs((int) ((double) timeOffset * 1000.0 / 40));
                    if (num >= 5) hits[5]++;
                    else hits[num]++;
                }
                if (bomb){
                    hits[5]++;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Beat), "Update")]
            public static void Postfix(Beat __instance)
            {
                if (!((UnityEngine.Object) __instance.row.playerBox == (UnityEngine.Object) null || (UnityEngine.Object) __instance.row.ent == (UnityEngine.Object) null || __instance.row.dead))
                {
                    if (__instance.conductor.audioPos > __instance.inputTime + 0.4 && !__instance.playerDrives7thBeat && !__instance.hasPulsed7thBeat && !__instance.bomb && !__instance.unhittable)
                    {
                        hits[5]++;
                    }
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(HUD), "ShowRankDescription")]
            public static void Postfix(HUD __instance)
            {
                if (__instance.game.currentLevel.levelType == LevelType.Boss) return;
                if (GC.showAbsoluteOffsets && !GC.twoPlayerMode)
                {
                    double num = (hits[0] + hits[1] * 0.9 + hits[2] * 0.75 + hits[3] * 0.5 + hits[4] * 0.25) / (hits[0] + hits[1] + hits[2] + hits[3] + hits[4] + hits[5]) * 100;
                    __instance.resultsSingleplayer.text = __instance.resultsSingleplayer.text + "\nAccuracy: " + Math.Round(num, 2).ToString() + "%";
                    __instance.resultsSingleplayer.gameObject.SetActive(true);
                }
                else 
                {
                    __instance.resultsSingleplayer.gameObject.SetActive(false);
                }
            }

            // debug
            /*
            [HarmonyPostfix]
            [HarmonyPatch(typeof(scnGame), "Update")]
            public static void Postfix(scnGame __instance)
            {
                if (!RDC.debug){
                    __instance.debugText.gameObject.SetActive(true);
                    __instance.debugText.text += string.Format("\n{0}, {1}, {2}, {3}, {4}, {5}", (object) (int) hits[0], (object) (int) hits[1], (object) (int) hits[2], (object) (int) hits[3], (object) (int) hits[4], (object) (int) hits[5]);
                    __instance.currentLevel.Update();
                }
            }
            */
        }
    }
}