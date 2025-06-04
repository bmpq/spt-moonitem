using EFT;
using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Reflection;

namespace tarkin.moonitem
{
    internal class Patch_LootItem_CreateLootWithRigidbody : ModulePatch
    {
        public static event Action<LootItem> OnPostfix;

        protected override MethodBase GetTargetMethod()
        {
            return typeof(LootItem)
                .GetMethod("CreateLootWithRigidbody", BindingFlags.Public | BindingFlags.Static)
                .MakeGenericMethod(typeof(LootItem));
        }

        [PatchPostfix]
        private static void PatchPostfix(LootItem __result)
        {
            OnPostfix?.Invoke(__result);
        }
    }
}
