using EFT;
using EFT.Interactive;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Reflection;
using UnityEngine;

namespace tarkin.moonitem
{
    internal class Patch_GameWorld_ThrowItem : ModulePatch
    {
        public static event Action<Item, LootItem> OnPostfix;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GameWorld), nameof(GameWorld.ThrowItem), new Type[] { typeof(Item), typeof(IPlayer), typeof(Vector3), typeof(Quaternion), typeof(Vector3), typeof(Vector3), typeof(bool), typeof(bool), typeof(float)});
        }

        [PatchPostfix]
        private static void PatchPostfix(GameWorld __instance, LootItem __result, Item item, IPlayer player, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity, bool syncable, bool performPickUpValidation = true, float makeVisibleAfterDelay = 0f)
        {
            OnPostfix?.Invoke(item, __result);
        }
    }
}
