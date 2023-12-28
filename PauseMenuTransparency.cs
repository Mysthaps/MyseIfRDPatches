using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace MyseIfRDPatches
{
    public class PauseMenuTransparency
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(scnGame), "TogglePauseGame")]
        public static void Postfix(scnGame __instance)
        {
            if (__instance.paused){
                __instance.pauseBackgroundCanvasGroup.DOKill();
                __instance.pauseBackgroundCanvasGroup.DOFade(Main.configPauseMenuTransparency.Value, 0.15f).SetUpdate<TweenerCore<float, float, FloatOptions>>(true);
            }
        }
    }
}