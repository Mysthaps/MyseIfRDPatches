using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace MyseIfRDPatches
{
    public class DisableMute
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(scnBase), "FocusVolumeUpdate")]
        public static bool Prefix()
        {
            return false;
        }
    }
}