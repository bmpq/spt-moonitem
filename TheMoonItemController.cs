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
        Player player => Singleton<GameWorld>.Instance.MainPlayer;

        Item item;
        public GameObject itemObject;
        LootItem lootItem;
        bool calledCreateStaticLoot;

        private static readonly Dictionary<string, ItemTemplate> _templates = new Dictionary<string, ItemTemplate>();


        internal static void SpawnTemplate(string template, Player player, Func<ItemTemplate, bool> filter)
        {
            var result = TemplateHelper.FindTemplates(template).FirstOrDefault();

            if (result == null)
                return;

            SpawnTemplate(result, player);
        }

        void Start()
        {
            SpawnTemplate("683e1aca717050d545879d90", player, null);
        }

        private static void SpawnTemplate(ItemTemplate template, Player player)
        {
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

                                SetupItem(itemFactory, item, lootItem);
                            }
                        }
                    });

                    return Task.CompletedTask;
                });
        }

        private static void SetupItem(ItemFactoryClass itemFactory, Item item, LootItem lootItem)
        {
            // ugly, wrote wehn i was eepy, todo cleanup
            TheMoonItemController.Instance.itemObject = lootItem.TrackableTransform.gameObject;

            item.SpawnedInSession = true; // found in raid

        }

        void FixedUpdate()
        {

        }

        void Update()
        {
            if (itemObject == null)
                return;

            if (player != null)
            {
                itemObject.transform.position = player.Position + new Vector3(0, 1.6f, 0) + player.Transform.Original.forward * 0.7f;
            }
        }
    }
}
