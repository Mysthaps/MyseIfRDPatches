using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace MyseIfRDPatches
{
    public class SpeedChange
    {
        private static float chiliSpeed = Main.configCustomChiliSpeed.Value;
        private static float iceSpeed = Main.configCustomIceSpeed.Value;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(scnGame), "StartTheGame")]
        public static bool Prefix(ref float speed)
        {
            if (chiliSpeed == 2f) chiliSpeed = 2.000001f;
            if (speed > 1f) speed = chiliSpeed;
            if (speed < 1f) speed = iceSpeed;
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(HUD), "ShowAndSaveRank")]
        public static void Postfix(HUD __instance)
        {
            var levelType = __instance.game.currentLevel.levelType;
            if (levelType == LevelType.Boss || levelType == LevelType.Challenge) return;

            // nope not doing linear regression or whatever that is
            if (RDTime.speed > 1.5f) __instance.rank.color = new Color(1f, 0.2f, 0.2f);
            if (RDTime.speed < 0.75f) __instance.rank.color = new Color(0.2f, 0.2f, 1f);
        }

        /*
        private static float lastSpeed = 1f;
        private static int lastSpeedIndex = 1;
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LevelDetail), "Start")]
        public static bool Prefix()
        {
            if (scnCLS.CachedData.hasSavedData)
            {
                lastSpeed = scnGame.customLevelSpeed;
                lastSpeedIndex = (lastSpeed < 1 ? 0 : (lastSpeed == 1 ? 1 : 2));
            }
            return true;
        }*/
    }
}