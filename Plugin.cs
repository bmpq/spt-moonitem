using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using SPT.Reflection.Patching;
using System;
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

        private void Awake()
        {
            Log = base.Logger;

            Patch_GameWorld_OnGameStarted.OnPostfix += () => 
            { 
                new GameObject("TheMoonItemController").AddComponent<TheMoonItemController>(); 
            };

            InitConfiguration();
            EnableAllPatches();
        }

        private void InitConfiguration()
        {
            KeybindExample = Config.Bind("Keybinds", "Keybind Example", new KeyboardShortcut(KeyCode.Alpha7), "");
            FloatExample = Config.Bind("General", "Float value", 0.5f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f)));
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
    }
}