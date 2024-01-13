using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace MyseIfRDPatches
{
    public class WindowDanceScale
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(scrVfxControl), "GetWindowScaleForDancing")]
        public static void Postfix(ref int __result)
        {
            __result = Math.Max(RDC.windowDanceSimulate ? 1 : 2, __result);
        }
    }
}