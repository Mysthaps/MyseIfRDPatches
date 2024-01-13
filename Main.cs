using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace MyseIfRDPatches
{
    [BepInPlugin("com.rhythmdr.myseifrdpatches", "MyseIf's RD Patches", "2.3.0")]
    [BepInProcess("Rhythm Doctor.exe")]
    public class Main : BaseUnityPlugin
    {
        internal static ConfigEntry<float> configCustomChiliSpeed;
        internal static ConfigEntry<float> configCustomIceSpeed;
        internal static ConfigEntry<bool> configRankColorOnSpeedChange;
        internal static ConfigEntry<bool> configEnableBossSpeedChange;
        internal static ConfigEntry<float> configPauseMenuScale;
        internal static ConfigEntry<float> configPauseMenuTransparency;
        internal static ConfigEntry<ShowFPSOptions> configShowFPS;
        internal static ConfigEntry<bool> configShowAccuracy;
        internal static ConfigEntry<AccuracyOptions> configAccuracyMode;
        internal static ConfigEntry<bool> configGhostTapMiss;
        internal static ConfigEntry<FailConditionOptions> configFailCondition;
        internal static ConfigEntry<bool> configAutoArtistLinks;
        internal static ConfigEntry<bool> configWindowDanceScale;

        internal enum ShowFPSOptions { Enabled, Legacy, Disabled }
        internal enum FailConditionOptions { None, Heartbreak, Perfect }
        internal enum AccuracyOptions { Standard, ADOFAI }

        private void Awake()
        {
            configCustomChiliSpeed = Config.Bind(
                "Speed Modifiers", "Custom Chili Speed", 1.5f, 
                new ConfigDescription(
                    "Changes the speed of Chili mode.", 
                    new AcceptableValueRange<float>(1.5f, 3f), 
                    new ConfigurationManagerAttributes { Order = 3 }
                )
            );
            configCustomIceSpeed = Config.Bind(
                "Speed Modifiers", "Custom Ice Speed", 0.75f, 
                new ConfigDescription(
                    "Changes the speed of Ice mode.", 
                    new AcceptableValueRange<float>(0.1f, 0.75f), 
                    new ConfigurationManagerAttributes { Order = 2 }
                )
            );
            configRankColorOnSpeedChange = Config.Bind(
                "Speed Modifiers", "Change Rank Color on Speed Change", false, 
                new ConfigDescription(
                    "Changes rank screen color based on values set in Custom Chili Speed and Custom Ice Speed.",
                    null,
                    new ConfigurationManagerAttributes { Order = 1 }
                )
            );
            configWindowDanceScale = Config.Bind(
                "General", "Force Window Dance Scale", false, 
                new ConfigDescription(
                    "Forces normal Window Dance resolution to always be at least 2x.",
                    null, 
                    new ConfigurationManagerAttributes { Order = 5 }
                )
            );
            configPauseMenuScale = Config.Bind(
                "General", "Pause Menu Scale", 1f, 
                new ConfigDescription(
                    "Changes the scale of the pause menu.",
                    new AcceptableValueRange<float>(0.25f, 1f),
                    new ConfigurationManagerAttributes { ShowRangeAsPercent = true, Order = 4 }
                )
            );
            configPauseMenuTransparency = Config.Bind(
                "General", "Pause Menu Transparency", 1f, 
                new ConfigDescription(
                    "Changes the background transparency while paused.",
                    new AcceptableValueRange<float>(0f, 1f), 
                    new ConfigurationManagerAttributes { Order = 3 }
                )
            );
            configEnableBossSpeedChange = Config.Bind(
                "General", "Enable Boss Speed Change", false, 
                new ConfigDescription(
                    "Allows the speed of boss levels to be changed.",
                    null,
                    new ConfigurationManagerAttributes { Order = 2 }
                )
            );
            configShowFPS = Config.Bind(
                "General", "Show FPS Counter", ShowFPSOptions.Disabled, 
                new ConfigDescription(
                    "Adds an FPS counter on the top left of the screen while in a level.",
                    null,
                    new ConfigurationManagerAttributes { Order = 1 }
                )
            );
            configGhostTapMiss = Config.Bind(
                "Gameplay", "Miss on Ghost Taps", false, 
                new ConfigDescription(
                    "Get a miss if the player taps without hitting a beat.",
                    null,
                    new ConfigurationManagerAttributes { Order = 2 }
                )
            ); 
            configFailCondition = Config.Bind(
                "Gameplay", "Set Fail Condition", FailConditionOptions.None, 
                new ConfigDescription(
                    "Heartbreak: Level fails if any row's heart is cracked.\nPerfect: Level fails if the player gets any miss.",
                    null,
                    new ConfigurationManagerAttributes { Order = 1 }
                )
            ); 
            configShowAccuracy = Config.Bind(
                "Accuracy", "Show Accuracy", false, 
                new ConfigDescription(
                    "Adds an accuracy count at the end of a level if Detailed Level Results is enabled.",
                    null,
                    new ConfigurationManagerAttributes { Order = 2 }
                )
            ); 
            configAccuracyMode = Config.Bind(
                "Accuracy", "Accuracy Mode", AccuracyOptions.Standard, 
                new ConfigDescription(
                    "Change between the standard accuracy count or the ADOFAI accuracy count.",
                    null,
                    new ConfigurationManagerAttributes { Order = 1 }
                )
            ); 
            configAutoArtistLinks = Config.Bind(
                "Editor", "Automatic Artist Links", false, 
                new ConfigDescription(
                    "Automatically adds artist links to a level from 7BG's database.",
                    null,
                    new ConfigurationManagerAttributes { Order = 1 }
                )
            ); 
            
            if (configEnableBossSpeedChange.Value)
                Harmony.CreateAndPatchAll(typeof(BossSpeedChange));

            if (configShowAccuracy.Value)
                Harmony.CreateAndPatchAll(typeof(ShowAccuracy));

            if (configShowFPS.Value != ShowFPSOptions.Disabled)
                Harmony.CreateAndPatchAll(typeof(ShowFPS));
            
            if (configGhostTapMiss.Value)
                Harmony.CreateAndPatchAll(typeof(GhostTapMiss));

            if (configFailCondition.Value != FailConditionOptions.None)
                Harmony.CreateAndPatchAll(typeof(FailCondition));

            if (configAutoArtistLinks.Value)
                Harmony.CreateAndPatchAll(typeof(AutoArtistLinks));

            if (configPauseMenuScale.Value != 1f)
                Harmony.CreateAndPatchAll(typeof(PauseMenuScale));

            if (configCustomChiliSpeed.Value != 1.5f || configCustomIceSpeed.Value != 0.75f)
                Harmony.CreateAndPatchAll(typeof(SpeedChange));

            if (configPauseMenuTransparency.Value != 1f)
                Harmony.CreateAndPatchAll(typeof(PauseMenuTransparency));

            if (configWindowDanceScale.Value)
                Harmony.CreateAndPatchAll(typeof(WindowDanceScale));

            Harmony.CreateAndPatchAll(typeof(scnGamePatch));

            Logger.LogInfo($"MyseIf's RD Patches is loaded!");
        }

        private void OnDestroy()
        {
            Harmony.UnpatchAll();
        }

        private static class scnGamePatch 
        {
            // Makes Story Mode levels not break
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

            [HarmonyPatch(typeof(scnGame), "Start")]
            [HarmonyPostfix]
            public static void Postfix()
            {
                ShowAccuracy.P1Hits = new int[] {0, 0, 0, 0, 0};
                ShowAccuracy.P2Hits = new int[] {0, 0, 0, 0, 0};

                GhostTapMiss.hasFailedLevel = false;
            }
        }
    }
}
