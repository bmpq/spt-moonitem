using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace tarkin.moonitem
{
    internal class AssetBundleLoader
    {
        public static string addPathToApplicationDataPath = Path.Combine("BepInEx", "plugins", "tarkin", "bundles");

        private static Dictionary<string, AssetBundle> loadedAssetBundles = new Dictionary<string, AssetBundle>();

        public static AssetBundle LoadAssetBundle(string filename)
        {
            string gameDirectory = Path.GetDirectoryName(Application.dataPath);
            string relativePath = Path.Combine(addPathToApplicationDataPath, filename);
            string fullPath = Path.Combine(gameDirectory, relativePath);

            string key = System.IO.Path.GetFileName(fullPath);

            if (loadedAssetBundles.ContainsKey(key))
            {
                return loadedAssetBundles[key];
            }

            AssetBundle assetBundle = AssetBundle.LoadFromFile(fullPath);
            if (assetBundle == null)
            {
                return null;
            }

            loadedAssetBundles.Add(key, assetBundle);
            return assetBundle;
        }

        public static void ReplaceShadersToNative(GameObject go)
        {
            Renderer[] rends = go.GetComponentsInChildren<Renderer>();

            foreach (var rend in rends)
            {
                foreach (var mat in rend.materials)
                {
                    Shader nativeShader = Shader.Find(mat.shader.name);
                    if (nativeShader != null)
                        mat.shader = nativeShader;
                }
            }
        }
    }
}
