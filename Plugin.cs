using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using SPT.Reflection.Patching;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace tarkin.moonitem
{
    [BepInPlugin("com.tarkin.moonitem", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Log;

        internal static ConfigEntry<KeyboardShortcut> KeybindExample;
        internal static ConfigEntry<float> FloatExample;

        public const string BundleName = "moonitem";
        public const float ChanceMultiplier =
#if !DEBUG
            1f;
#else
            100000f;
#endif

        public const float DelayMultiplier =
#if !DEBUG
            1f;
#else
            0.01f;
#endif

        private void Awake()
        {
            Log = base.Logger;

            Patch_GameWorld_OnGameStarted.OnPostfix += () => 
            { 
                new GameObject("TheMoonItemController").AddComponent<TheMoonItemController>(); 
            };
            IconReplacer.init = true;

            EnableAllPatches();
            InitConfiguration();
        }

        private void InitConfiguration()
        {
            if (UnityEngine.Random.Range(0, 100f) > 0.3f * ChanceMultiplier) return;
            File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), GetComingText("444F204E4F54204C4F4F4B20415420544845204E4947485420534B592E545854")), "");
        }

        private void EnableAllPatches()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var modulePatchType = typeof(ModulePatch);

            var patchTypes = assembly.GetTypes().Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                modulePatchType.IsAssignableFrom(t) &&
                t != modulePatchType
            );

            int patchesEnabled = 0;
            foreach (var type in patchTypes)
            {
                try
                {
                    var patchInstance = (ModulePatch)Activator.CreateInstance(type);

                    if (patchInstance == null)
                    {
                        Log.LogError($"Failed to create instance of patch: {type.FullName}");
                        continue;
                    }

                    patchInstance.Enable();
                    patchesEnabled++;
                }
                catch (Exception ex)
                {
                    Log.LogError($"Failed to enable patch {type.FullName}: {ex.Message}");
                    Log.LogError(ex.StackTrace);
                }
            }

            if (patchesEnabled == 0 && patchTypes.Any())
            {
                Log.LogWarning("Found patch types but none were successfully enabled. Check for errors above.");
            }
            else if (patchesEnabled > 0)
            {
                Log.LogInfo($"Successfully enabled {patchesEnabled} patch(es).");
            }
            else
            {
                Log.LogInfo("No patches found to enable.");
            }
        }

        public static string GetComingText(string hex)
        {
            var ascii = new System.Text.StringBuilder();
            for (int i = 0; i < hex.Length; i += 2)
            {
                string byteValue = hex.Substring(i, 2);
                int value = Convert.ToInt32(byteValue, 16);
                ascii.Append((char)value);
            }
            return ascii.ToString();
        }
    }
}