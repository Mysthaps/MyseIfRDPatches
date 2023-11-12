using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace MyseIfRDPatches
{
    public class ShowFPS
    {

        [HarmonyPostfix]
        [HarmonyPatch(typeof(scnGame), "Update")]
        public static void Postfix(scnGame __instance)
        {
            if (!RDC.debug){
                __instance.debugText.gameObject.SetActive(true);

                switch (Main.configShowFPS.Value)
                {
                    case Main.ShowFPSOptions.Enabled:
                        if (scnGame.fps == 0.0) scnGame.fps = 1f / Time.unscaledDeltaTime;
                        scnGame.fps = scnGame.fps * 0.99f + (1f / Time.unscaledDeltaTime) * 0.01f;
                        __instance.debugText.text = "" + string.Format("<color=#ffffff>FPS:</color> <color={0}>{1}</color>", (object) (scnGame.fps < 30 ? "#ff0000" : (scnGame.fps < 50 ? "#ffff00" : "#00ff00")), (object) (int) scnGame.fps);
                        break;
                    case Main.ShowFPSOptions.Legacy:
                        int num = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
                        __instance.debugText.text = "" + string.Format("<color=#ffffff>FPS:</color> <color={0}>{1}</color>", (object) (num < 30 ? "#ff0000" : (num < 50 ? "#ffff00" : "#00ff00")), (object) num);
                        break;
                }
                    
                __instance.currentLevel.Update();
            }
        }
    }
}