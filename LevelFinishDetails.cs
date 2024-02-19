using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace MyseIfRDPatches
{
    public class LevelFinishDetails
    {
        internal static Text levelDetails;
        internal static string song = "";
        internal static string artist = "";
        internal static string author = "";
        internal static string mods = "";
        internal static string bestPrev = "";

        [HarmonyPostfix]
        [HarmonyPatch(typeof(HUD), "ShowRankDescription")]
        public static void Postfix(HUD __instance)
        {
            levelDetails = Text.Instantiate(__instance.description, __instance.description.gameObject.transform.position, Quaternion.identity);
            levelDetails.gameObject.transform.parent = __instance.description.gameObject.transform.parent.transform.parent;
            levelDetails.fontSize = Main.configLevelFinishFontSize.Value;
            levelDetails.alignment = TextAnchor.UpperLeft;
            levelDetails.gameObject.transform.position = new Vector3(147, 170, 20);
            levelDetails.font = RDString.GetAppropiateFontForString(song + artist + author);

            if (song.Length > 50) song = song.Substring(0, 49) + "...";
            if (artist.Length > 50) artist = artist.Substring(0, 49) + "...";
            if (author.Length > 50) author = author.Substring(0, 49) + "...";

            List<string> modList = new List<string>();

            if (Main.configGhostTapMiss.Value) modList.Add("GhostTapMiss");
            if (Main.configFailCondition.Value == Main.FailConditionOptions.Heartbreak) modList.Add("Heartbreak");
            if (Main.configFailCondition.Value == Main.FailConditionOptions.Perfect) modList.Add("Perfect");
            if (RDTime.speed != 0) modList.Add(Math.Round(RDTime.speed, 2).ToString() + "x");

            if (modList.Count > 0) mods = string.Join(", ", modList.ToArray());
            else mods = "None";

            string str = "";
            if (scnGame.levelToLoadSource == LevelSource.InternalPath){
                song = RDString.Get("levelSelect." + __instance.game.levelIdentifier);
                artist = author = "";
                bestPrev = Persistence.GetLevelRank(__instance.game.levelIdentifier);
                str = Main.configLevelFinishText_MainGame.Value;
            }
            else str = Main.configLevelFinishText.Value;
            
            str = str.Replace("{song}", song);
            str = str.Replace("{artist}", artist);
            str = str.Replace("{author}", author);
            str = str.Replace("{mods}", mods);
            str = str.Replace("{bestPrev}", bestPrev);

            levelDetails.text = str;
        }
    }
}