using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using System.Threading.Tasks;
using UnityEngine;
using System.Threading;

namespace tarkin.moonitem
{
    internal class TheMoonItemController : MonoBehaviour
    {
        Item item;
        GameObject itemObject;

        void Start()
        {
            LoadBundle();
        }

        async Task LoadBundle()
        {
            // flimsy as fuck
            DependencyGraphClass<IEasyBundle>.GClass3901 gclass3901_0 = Singleton<IEasyAssets>.Instance.Retain(new string[] { "moon.bundle" }, null, default(CancellationToken));
            await GClass1628.LoadBundles(gclass3901_0);
            SpawnItem();
        }

        void SpawnItem()
        {
            item = Singleton<ItemFactoryClass>.Instance.CreateItem(MongoID.Generate(true), "683e1aca717050d545879d90", null);
            itemObject = Singleton<PoolManagerClass>.Instance.CreateItem(item, false);

            Singleton<GameWorld>.Instance.CreateStaticLoot(itemObject, item, item.LocalizedName(), true, null);
        }

        void FixedUpdate()
        {

        }

        void Update()
        {
            if (itemObject == null)
                return;

            itemObject.gameObject.SetActive(true);

            Player player = Singleton<GameWorld>.Instance?.MainPlayer;
            if (player != null)
            {
                itemObject.transform.position = player.Position + new Vector3(0, 1.6f, 0) + player.Transform.Original.forward * 0.7f;
            }
        }
    }
}
