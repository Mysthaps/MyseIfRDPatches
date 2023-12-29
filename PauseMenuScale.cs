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
    public class PauseMenuScale
    {
        private static float pauseScale = Main.configPauseMenuScale.Value;
        private static float posScale = 2f - pauseScale;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(RDPauseMenu), "Initialize")]
        public static void Postfix(RDPauseMenu __instance)
        {
            __instance.phoneRect.localScale = new Vector3(pauseScale, pauseScale, pauseScale);
            __instance.phoneRect.anchoredPosition3D = __instance.phoneRect.anchoredPosition3D.WithX(__instance.phoneShowPos.x * posScale);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(RDPauseMenu), "ShowAnimated")]
        public static void ShowAnimated(RDPauseMenu __instance, bool showLevelDetail, bool useDelay)
        {
            float delay = useDelay ? 0.1f : 0.0f;
            __instance.phoneRect.DOKill();
            if (showLevelDetail) __instance.phoneRect.DOAnchorPos(__instance.usingDetailPhonePos * posScale, __instance.toggleAnimDuration).SetUpdate(true).SetEase(Ease.OutExpo);
            else __instance.phoneRect.DOAnchorPos(__instance.phoneShowPos * posScale, __instance.toggleAnimDuration - delay).SetDelay(delay).SetEase(Ease.OutBack, 1f).SetUpdate(true);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(RDPauseMenu), "Hide")]
        public static void Hide(RDPauseMenu __instance)
        {
            __instance.phoneRect.DOKill();
            __instance.phoneRect.DOAnchorPos(__instance.phoneHidePos * posScale, __instance.toggleAnimDuration).SetEase(__instance.currentSceneName == "scnMenu" ? Ease.OutExpo : Ease.OutCubic).SetUpdate(true);
        }
    }
}