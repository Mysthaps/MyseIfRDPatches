using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace MyseIfRDPatches
{
    public class GhostTapMiss
    {
        internal static bool endLevel = false;
        [HarmonyPostfix]
        [HarmonyPatch(typeof(HUD), "AdvanceGameover")]
        public static void Postfix(){
            endLevel = true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(scnGame), "UpdateGameplayInput")]
        public static void Postfix(RDPlayer player, bool keyPressed, scnGame __instance)
        {
            if (endLevel) return;
            if (keyPressed && __instance.spacebarOnNothing && __instance.levelIdentifier != "SongOfTheSea" && __instance.levelIdentifier != "SongOfTheSeaH")
            {
                scrConductor.PlayFeedback(GameSoundType.BigMistake, group: RDUtils.GetMixerGroup((player == RDPlayer.P1 ? "PlayerOneMistakes" : "PlayerTwoMistakes")));
                __instance.game.OnMistakeOrHeal(0f, 1f, null, false, player);
                RDBase.Vfx.FlashBorderFeedback(false);
                if (Main.configFailCondition.Value == Main.FailConditionOptions.Perfect)
                {
                    foreach (Row row in __instance.rows){
                        if (row.ent != null){
                            try {
                                __instance.game.FailLevel(row.ent);
                            }
                            catch {
                                __instance.game.LevelFailSequence(row.ent);
                            }
                            endLevel = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}