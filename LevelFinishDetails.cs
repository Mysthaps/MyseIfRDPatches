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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(HUD), "ShowRankDescription")]
        public static void Postfix(HUD __instance)
        {
            levelDetails = Text.Instantiate(__instance.description, __instance.description.gameObject.transform.position, Quaternion.identity);
            levelDetails.gameObject.transform.parent = __instance.description.gameObject.transform.parent.transform.parent;
            levelDetails.fontSize = 4;
            levelDetails.alignment = TextAnchor.UpperLeft;
            levelDetails.gameObject.transform.position = new Vector3(147, 170, 20);

            if (song.Length > 50) song = song.Substring(0, 49) + "...";
            if (artist.Length > 50) artist = artist.Substring(0, 49) + "...";
            if (author.Length > 50) author = author.Substring(0, 49) + "...";

            string str1 = String.Format("Song: {0}\n", song);
            string str2 = String.Format("Artist: {0}\n", artist);
            string str3 = String.Format("Author: {0}\n", author);
            string str4 = String.Format((RDTime.speed != 1f ? "Speed: x{0}\n" : ""), Math.Round(RDTime.speed, 2));
            string str5 = string.Empty;
            List<string> modList = new List<string>();

            if (Main.configGhostTapMiss.Value) modList.Add("GhostTapMiss");
            if (Main.configFailCondition.Value == Main.FailConditionOptions.Heartbreak) modList.Add("Heartbreak");
            if (Main.configFailCondition.Value == Main.FailConditionOptions.Perfect) modList.Add("Perfect");

            if (scnGame.levelToLoadSource == LevelSource.InternalPath){
                str1 = String.Format("Song: {0}\n", RDString.Get("levelSelect." + __instance.game.levelIdentifier));
                str2 = str3 = "";
            }
            if (modList.Count > 0) str5 = String.Format("Mods used: {0}\n", string.Join(", ", modList.ToArray()));

            levelDetails.font = RDString.GetAppropiateFontForString(str1 + str2 + str3);

            levelDetails.text = str1 + str2 + str3 + str4 + str5;
        }
    }
}