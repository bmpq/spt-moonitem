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

        const string guidMoon = "683e1aca717050d545879d90";
        Player player => Singleton<GameWorld>.Instance.MainPlayer;

        Item item;
        public GameObject itemObject;
        LootItem lootItem;
        MeshRenderer[] rends;

        float moonSize = 2.2f;

        bool lootable;
        bool looted;

        void OnEnable()
        {
            player.InventoryController.AddItemEvent += InventoryController_AddItemEvent;

            if (TOD_Sky.Instance == null)
            {
                Destroy(gameObject);
                return;
            }
            SpawnTemplate(guidMoon, player);
        }

        private void InventoryController_AddItemEvent(GEventArgs2 obj)
        {
            Item grabbedItem = obj.Item;

            if (looted || grabbedItem?.StringTemplateId != guidMoon)
                return;

            looted = true;

            TOD_Sky.Instance.Night.LightIntensity = 0;
            TOD_Sky.Instance.Components.MoonRenderer.gameObject.SetActive(false);
        }

        void OnDisable()
        {
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

        void LateUpdate() // after input and procedural animations
        {
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

            lootable = IsLookingAtMoon(CameraClass.Instance.Camera);

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
