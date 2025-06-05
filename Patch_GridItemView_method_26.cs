using EFT;
using EFT.UI.DragAndDrop;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace tarkin.moonitem
{
    internal class Patch_GridItemView_method_26 : ModulePatch
    {
        public static event Action<GridItemView, TextMeshProUGUI> OnPostfix;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GridItemView), nameof(GridItemView.method_26));
        }

        [PatchPostfix]
        private static void PatchPostfix(GridItemView __instance, TextMeshProUGUI ___Caption)
        {
            if (UnityEngine.Random.Range(0, 100f) > 0.05f)
                return;

            CoroutineRunner.Instance.StartCoroutine(Animation(___Caption));

            OnPostfix?.Invoke(__instance, ___Caption);
        }

        static IEnumerator Animation(TextMeshProUGUI caption)
        {
            TextAsset textAsset = AssetBundleLoader.LoadAssetBundle("moonitem").LoadAsset<TextAsset>("captions");
            if (textAsset == null)
            {
                yield break;
            }

            string original = caption.text;

            string[] lines = textAsset.text.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

            caption?.SetText(lines[UnityEngine.Random.Range(0, lines.Length)].Trim());

            yield return new WaitForEndOfFrame();

            caption?.SetText(original);
        }
    }
}
