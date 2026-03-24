using System.Collections.Generic;

namespace Ninsar.Inventory
{
    public class InventoryIndex
    {
        private readonly Dictionary<BaseItem, List<ItemData>> _index = new();
        public void Add(ItemData item)
        {
            if (item.Source == null) return;

            if (!_index.TryGetValue(item.Source, out var list))
            {
                list = new List<ItemData>();
                _index[item.Source] = list;
            }
            list.Add(item);
        }
        public void Remove(ItemData item)
        {
            if (item.Source == null || !_index.TryGetValue(item.Source, out var list))
                return;

            list.Remove(item);
            if (list.Count == 0)
                _index.Remove(item.Source);
        }
        public bool TryGetStacks(BaseItem baseItem, out List<ItemData> stacks)
        {
            return _index.TryGetValue(baseItem, out stacks);
        }
        public void Clear()
        {
            _index.Clear();
        }
    }
}