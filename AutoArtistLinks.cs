using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace MyseIfRDPatches
{
    public class AutoArtistLinks
    {
        private static string str = "";
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ArtistUIDisclaimer), "SetData")]
        public static void Postfix(ArtistData data, ArtistUIDisclaimer __instance)
        {
            if (!string.IsNullOrEmpty(data.link1)) str = data.link1;
            if (!string.IsNullOrEmpty(data.link2)) str = str + ", " + data.link2;
            __instance.editor.levelSettings.artistLinks = str;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(RDLevelEditor.InspectorPanel_LevelSettings), "HideArtistDropdown")]
        public static void Postfix(ref InputField ___artistLinks)
        {
            ___artistLinks.text = str;
        }
    }
}