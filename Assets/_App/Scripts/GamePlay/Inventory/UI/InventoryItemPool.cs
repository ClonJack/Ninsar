using System.Collections.Generic;
using Ninsar.Configs.Inventory;
using UnityEngine;

namespace Ninsar.Inventory
{
    public class InventoryItemPool
    {
        public readonly List<ItemView> Pool = new();
        private Transform _parent;
        public int Count => Pool.Count;
        public void Create(Transform parent, int poolSize)
        {
            _parent = parent;
            var itemPrefab = ServicesLocator.Instance.GetServices<InventoryConfig>().ItemPrefab;

            for (int i = 0; i < poolSize; i++)
            {
                var instance = Object.Instantiate(itemPrefab, _parent);
                instance.gameObject.SetActive(false);
                Pool.Add(instance);
            }
        }
        public ItemView Get(int index)
        {
            if (index < Pool.Count)
                return Pool[index];

            var itemPrefab = ServicesLocator.Instance.GetServices<InventoryConfig>().ItemPrefab;
            var instance = Object.Instantiate(itemPrefab, _parent);
            Pool.Add(instance);
            return instance;
        }
        public void HideFrom(int index)
        {
            for (int i = index; i < Pool.Count; i++)
                Pool[i].gameObject.SetActive(false);
        }
        public void Clear()
        {
            foreach (var item in Pool)
            {
                if (item != null)
                    Object.Destroy(item.gameObject);
            }
            Pool.Clear();
        }
    }
}