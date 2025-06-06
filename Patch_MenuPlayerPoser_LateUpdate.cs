using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Reflection;
using UnityEngine;

namespace tarkin.moonitem
{
    internal class Patch_MenuPlayerPoser_LateUpdate : ModulePatch
    {
        public static bool enabled;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MenuPlayerPoser), nameof(MenuPlayerPoser.LateUpdate));
        }

        [PatchPostfix]
        private static void PatchPostfix(MenuPlayerPoser __instance)
        {
            if (!enabled)
                return;

            Transform neck = __instance.PBones.Neck.transform;
            Quaternion lookAtRotation = Quaternion.LookRotation(new Vector3(157, -15, -144), Vector3.left);
            neck.rotation = lookAtRotation;
            __instance.PBones.Head.Original.localEulerAngles = Vector3.zero;
        }
    }
}
