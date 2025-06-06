using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using System.Threading.Tasks;
using UnityEngine;
using System.Threading;
using EFT.Interactive;
using System;
using Random = UnityEngine.Random;
using System.Linq;
using Diz.Utils;
using System.Collections.Generic;
using EFT.CameraControl;
using EFT.UI;
using System.Reflection;
using HarmonyLib;
using UnityEngine.UI;
using EFT.UI.BattleTimer;
using TMPro;

namespace tarkin.moonitem
{
    internal class TheMoonItemController : MonoBehaviour
    {
        public static TheMoonItemController Instance
        { 
        get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<TheMoonItemController>();
                return _instance;
            }
        }
        private static TheMoonItemController _instance;

        public const string guidMoon = "683e1aca717050d545879d90";
        GameWorld gameWorld => Singleton<GameWorld>.Instance;
        Player player => Singleton<GameWorld>.Instance.MainPlayer;

        Item item;
        public GameObject itemObject;
        LootItem lootItem;
        MeshRenderer[] rends;

        const float moonSize = 2.2f;

        bool lootable;
        bool looted => TOD_Sky.Instance.Night.LightIntensity <= 0f;

        bool lineOfSight;

        bool f44;
        float timeCountdown = 57;

        bool scale;
        float scaleSpeed = 0.1f;

        bool lightevent;

        GameObject iconEffect;

        TextMeshProUGUI textTimerPanel;

        void OnEnable()
        {
            AssetBundleLoader.LoadAssetBundle(Plugin.BundleName);

            scale = Random.Range(0f, 100f) < 0.3f * Plugin.ChanceMultiplier;
            lightevent = Random.Range(0f, 100f) < 0.2f * Plugin.ChanceMultiplier;

            player.InventoryController.AddItemEvent += InventoryController_AddItemEvent;

            Patch_GameWorld_ThrowItem.OnPostfix += OnThrowItem;

            if (TOD_Sky.Instance == null)
            {
                Destroy(gameObject);
                return;
            }
            SpawnTemplate(guidMoon, player);

            ToggleMoonHealthEffect(CheckIfPlayerHasMoonInInventory());
            
            CoroutineRunner.RunAfterDelay(() => Coming(), 180f * Plugin.DelayMultiplier);


        }

        void Coming()
        {
            f44 = Random.Range(0f, 100f) < 0.2f * Plugin.ChanceMultiplier;
            if (!f44)
                return;

            timeCountdown = 57f * Plugin.DelayMultiplier;

            MonoBehaviourSingleton<GameUI>.Instance.LocationTransitTimerPanel.Display();
            textTimerPanel = MonoBehaviourSingleton<GameUI>.Instance.LocationTransitTimerPanel.GetComponentInChildren<TextMeshProUGUI>();
            RectTransform panel = MonoBehaviourSingleton<GameUI>.Instance.LocationTransitTimerPanel.transform.GetChild(0) as RectTransform;
            panel.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            panel.sizeDelta = new Vector2(318, 51);
            panel.GetComponent<Image>().color = Color.red;
        }

        bool CheckIfPlayerHasMoonInInventory()
        {
            return player.Inventory.GetPlayerItems(EPlayerItems.Equipment).Any(item => item.TemplateId == guidMoon);
        }

        private void ToggleMoonHealthEffect(bool on)
        {
            if (iconEffect != null)
            {
                iconEffect.SetActive(on);

                BreakLegs();
                return;
            }

            if (!on)
                return;

            FieldInfo fieldCharacterHealthPanel = AccessTools.Field(typeof(EftBattleUIScreen), "_characterHealthPanel");
            CharacterHealthPanel healthPanel = fieldCharacterHealthPanel.GetValue(MonoBehaviourSingleton<CommonUI>.Instance.EftBattleUIScreen) as CharacterHealthPanel;

            FieldInfo fieldEffectPanel = AccessTools.Field(typeof(CharacterHealthPanel), "_effectsPanel");
            GameObject _effectsPanel = fieldEffectPanel.GetValue(healthPanel) as GameObject;
            iconEffect = new GameObject("IconMoonEffect");
            iconEffect.AddComponent<Image>().sprite = AssetBundleLoader.LoadAssetBundle(Plugin.BundleName).LoadAsset<Sprite>("effect_icon_moon");
            iconEffect.transform.SetParent(_effectsPanel.transform);

            BreakLegs();
        }

        void BreakLegs()
        {
            player.ActiveHealthController.DoFracture(EBodyPart.LeftLeg);
            player.ActiveHealthController.DoFracture(EBodyPart.RightLeg);
        }

        private void OnThrowItem(Item item, LootItem lootItem)
        {
            CoroutineRunner.RunAfterDelay(() => OnMoonDrop(item, lootItem), 0.1f);
        }

        void OnMoonDrop(Item item, LootItem lootItem)
        {
            ToggleMoonHealthEffect(CheckIfPlayerHasMoonInInventory());

            Item moon = item.GetRootItem().GetAllItems().FirstOrDefault(item => item?.TemplateId == guidMoon);
            if (moon == null)
                return;

            if (lootItem == null)
                return;

            MeshRenderer[] rends = lootItem.GetComponentsInChildren<MeshRenderer>();

            foreach (var rend in rends)
            {
                rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                foreach (var mat in rend.materials)
                {
                    mat.SetVector("_SpecVals", new Vector4(9, 90f, 0, 0));
                }
            }

            Collider col = lootItem.GetComponentInChildren<Collider>();
            Light light = col.gameObject.GetOrAddComponent<Light>();
            light.intensity = item.SpawnedInSession && !looted ? 0 : 2f;
            light.range = 5;
            light.shadows = LightShadows.Hard;
        }

        private void InventoryController_AddItemEvent(GEventArgs2 obj)
        {
            Item grabbedItem = obj.Item;

            if (grabbedItem?.StringTemplateId != guidMoon)
                return;

            ToggleMoonHealthEffect(CheckIfPlayerHasMoonInInventory());

            if (looted)
                return;

            if (obj.Item.SpawnedInSession)
            {
                TOD_Sky.Instance.Night.LightIntensity = 0;
                TOD_Sky.Instance.Components.MoonRenderer.gameObject.SetActive(false);

                foreach (var lamp in LocationScene.GetAllObjects<LampController>(false))
                {
                    lamp.Switch(Turnable.EState.Destroyed);
                }
            }
        }

        void OnDisable()
        {
            Patch_GameWorld_ThrowItem.OnPostfix -= OnThrowItem;

            if (player?.InventoryController == null)
                return;
            player.InventoryController.AddItemEvent -= InventoryController_AddItemEvent;
        }

        private static void SpawnTemplate(string templateId, Player player)
        {
            var template = TemplateHelper.FindTemplates(templateId).FirstOrDefault();

            if (template == null)
                return;

            var poolManager = Singleton<PoolManagerClass>.Instance;

            poolManager
                .LoadBundlesAndCreatePools(PoolManagerClass.PoolsCategory.Raid, PoolManagerClass.AssemblyType.Online, template.AllResources.ToArray(), JobPriorityClass.Immediate)
                .ContinueWith(task =>
                {
                    AsyncWorker.RunInMainTread(delegate
                    {
                        if (task.IsFaulted)
                        {
                            Plugin.Log.LogError("load bundle failed");
                        }
                        else
                        {
                            var itemFactory = Singleton<ItemFactoryClass>.Instance;
                            var item = itemFactory.CreateItem(MongoID.Generate(), template._id, null);
                            if (item == null)
                            {
                                Plugin.Log.LogError("failed to create item");
                            }
                            else
                            {
                                _ = new TraderControllerClass(item, item.Id, item.ShortName);
                                var go = poolManager.CreateLootPrefab(item, ECameraType.Default);

                                go.SetActive(value: true);
                                var lootItem = Singleton<GameWorld>.Instance.CreateLootWithRigidbody(go, item, item.ShortName, randomRotation: false, null, out _, true);

                                var transform = player.Transform;
                                var position = transform.position
                                               + transform.right * Random.Range(-1f, 1f)
                                               + transform.forward * 2f
                                               + transform.up * 0.5f;

                                lootItem.transform.SetPositionAndRotation(position, transform.rotation);
                                lootItem.LastOwner = player;

                                TheMoonItemController.Instance.itemObject = lootItem.TrackableTransform.gameObject;
                                item.SpawnedInSession = true; // found in raid
                            }
                        }
                    });

                    return Task.CompletedTask;
                });
        }

        bool IsLookingAtMoon(Camera cam)
        {
            return Vector3.Angle(cam.transform.forward, TOD_Sky.Instance.MoonDirection) < moonSize;
        }

        void FixedUpdate()
        {
            if (CameraClass.Instance.Camera == null || TOD_Sky.Instance == null)
                return;
            Vector3 cameraPos = CameraClass.Instance.Camera.transform.position;
            lineOfSight = HasLineOfSightToTarget(cameraPos, TOD_Sky.Instance.MoonDirection);
        }

        private bool HasLineOfSightToTarget(Vector3 currentPosition, Vector3 dir)
        {
            Vector3 rayOrigin = currentPosition;

            Vector3 directionToTargetNormalized = dir.normalized;

            LayerMask layerMask = LayerMaskClass.HighPolyWithTerrainMask;

            bool isObstructed = Physics.Raycast(
                rayOrigin,
                directionToTargetNormalized,
                out RaycastHit hit,
                float.MaxValue,
                layerMask,
                QueryTriggerInteraction.Ignore);

            return !isObstructed;
        }

        void LateUpdate() // after input and procedural animations
        {
            if (scale && TOD_Sky.Instance.Moon.MeshSize < 18)
            {
                scaleSpeed += Time.deltaTime * 0.02f;
                TOD_Sky.Instance.Moon.MeshSize += scaleSpeed * Time.deltaTime * 0.1f;
            }

            if (f44)
            {
                timeCountdown -= Time.deltaTime;
                textTimerPanel?.SetText(Plugin.GetComingText("474F4420495320434F4D494E47") + " " + timeCountdown.ToString("F2"));

                if (timeCountdown < 0f)
                {
                    f44 = false;
                    MonoBehaviourSingleton<GameUI>.Instance.LocationTransitTimerPanel.Close();

                    GameObject moon00 = Instantiate(AssetBundleLoader.LoadAssetBundle(Plugin.BundleName).LoadAsset<GameObject>("moon_00_0_prefab"));
                    moon00.AddComponent<TheF>();

                    textTimerPanel?.SetText(Plugin.GetComingText("474F442049532048455245"));
                }
            }

            if (lightevent)
            {
                timeCountdown -= Time.deltaTime;
                TOD_Sky.Instance.Day.LightIntensity += (30f - timeCountdown) * Time.deltaTime * 0.01f;
                TOD_Sky.Instance.Night.LightIntensity += (30f - timeCountdown) * Time.deltaTime * 0.01f;

                if (timeCountdown < 0f)
                {
                    Singleton<GameWorld>.Instance.MainPlayer.KillMe(EBodyPartColliderType.Eyes, 7347);
                }
            }

            if (looted || itemObject == null)
                return;

            if (rends == null)
            {
                rends = itemObject.GetComponentsInChildren<MeshRenderer>();
                foreach (var rend in rends)
                {
                    rend.forceRenderingOff = true;
                }
            }

            lootable = lineOfSight && IsLookingAtMoon(CameraClass.Instance.Camera);

            if (lootable)
            {
                itemObject.transform.position = CameraClass.Instance.Camera.transform.position + CameraClass.Instance.Camera.transform.forward * 0.7f;
            }
            else
            {
                itemObject.transform.position = new Vector3(0, -1000, 0);
            }
        }
    }
}
