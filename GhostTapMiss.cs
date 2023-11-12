using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace MyseIfRDPatches
{
    public class GhostTapMiss
    {
        internal static bool hasFailedLevel = false;
        [HarmonyPostfix]
        [HarmonyPatch(typeof(scnGame), "UpdateGameplayInput")]
        public static void Postfix(RDPlayer player, bool keyPressed, scnGame __instance)
        {
            if (hasFailedLevel) return;
            if (keyPressed && __instance.spacebarOnNothing && __instance.levelIdentifier != "SongOfTheSea" && __instance.levelIdentifier != "SongOfTheSeaH")
            {
                scrConductor.PlayImmediately(GameSoundType.BigMistake, group: RDUtils.GetMixerGroup((player == RDPlayer.P1 ? "PlayerOneMistakes" : "PlayerTwoMistakes")), pan: (GC.twoPlayerMode ? RDUtils.OverridePanFor2P(player, 0.0f) : 0.0f));
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
                            hasFailedLevel = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}