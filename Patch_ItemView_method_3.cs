using EFT;
using EFT.UI.DragAndDrop;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace tarkin.moonitem
{
    internal class Patch_ItemView_method_3 : ModulePatch
    {
        public static event Action<ItemView, Image> OnPostfix;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ItemView), nameof(ItemView.method_3));
        }

        [PatchPostfix]
        private static void PatchPostfix(ItemView __instance, Image ___MainImage)
        {
            OnPostfix?.Invoke(__instance, ___MainImage);
        }
    }
}
