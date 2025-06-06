using EFT.UI.DragAndDrop;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace tarkin.moonitem
{
    internal static class IconReplacer
    {
        public static bool init; // set this so this static class instantiates

        static IconReplacer()
        {
            Patch_ItemView_method_3.OnPostfix += OnPostfix;
        }

        static void OnPostfix(ItemView __instance, Image ___MainImage)
        {
            if (___MainImage == null)
                return;

            if (__instance.Item.StringTemplateId != TheMoonItemController.guidMoon)
                return;

            if (Random.Range(0, 100f) > 0.7f * Plugin.ChanceMultiplier)
                return;

            Sprite originalSprite = ___MainImage.sprite;

            ___MainImage.sprite = AssetBundleLoader.LoadAssetBundle(Plugin.BundleName).LoadAsset<Sprite>("13");

            ___MainImage.SetNativeSize();
            LayoutRebuilder.ForceRebuildLayoutImmediate(___MainImage.rectTransform);
            __instance.UpdateScale();

#if DEBUG
            Plugin.Log.LogInfo("Icon replaced");
#endif

            CoroutineRunner.Instance.StartCoroutine(RevertSpriteAfterDelay(__instance, ___MainImage, originalSprite));
        }

        private static IEnumerator RevertSpriteAfterDelay(ItemView itemView, Image mainImage, Sprite originalSprite)
        {
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForEndOfFrame();
            }

            if (itemView == null || mainImage == null)
            {
                yield break;
            }

            mainImage.sprite = originalSprite;

            mainImage.SetNativeSize();
            LayoutRebuilder.ForceRebuildLayoutImmediate(mainImage.rectTransform);
            itemView.UpdateScale();
        }
    }
}