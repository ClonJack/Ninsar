using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ninsar.Inventory
{
    public class InventoryStorage
    {
        public IReadOnlyList<ItemData> Items => _items;
        public int MaxCapacity { get; private set; }
        public int UsedSlots => _items.Count;
        public int FreeSlots => MaxCapacity - UsedSlots;
        public bool IsFull => UsedSlots >= MaxCapacity;

        private readonly List<ItemData> _items = new();
        private readonly Dictionary<BaseItem, List<ItemData>> _index = new();

        public event Action<int, int> OnCapacityChanged;
        public event Action<BaseItem, int> OnAddFailed;
        public event Action<BaseItem, int> OnItemRejected;
        public event Action<ItemData> OnItemAdded;
        public event Action<ItemData> OnItemRemoved;

        public InventoryStorage(int maxCapacity = 5)
        {
            MaxCapacity = maxCapacity;
            Debug.Log($"[Inventory] Initialized with {MaxCapacity} slots");
        }

        public void SetMaxCapacity(int maxCapacity)
        {
            if (maxCapacity < 0)
            {
                Debug.LogWarning("[Inventory] Invalid max capacity");
                return;
            }

            MaxCapacity = maxCapacity;
            Debug.Log($"[Inventory] Capacity changed to {MaxCapacity} slots");
            OnCapacityChanged?.Invoke(UsedSlots, MaxCapacity);
        }
        public void Add(BaseItem baseItem, int count = 1)
        {
            if (!ValidateAddRequest(baseItem, count))
                return;

            if (baseItem.MaxStack <= 1)
            {
                int canAdd = Math.Min(count, FreeSlots);
                int rejected = count - canAdd;

                if (rejected > 0)
                {
                    Debug.LogWarning($"[Inventory] Rejected {rejected} {baseItem.Name}: no free slots");
                    OnItemRejected?.Invoke(baseItem, rejected);
                }

                if (canAdd <= 0)
                {
                    OnAddFailed?.Invoke(baseItem, count);
                    return;
                }

                AddNonStackableItems(baseItem, canAdd);
                Debug.Log($"[Inventory] Added {canAdd}/{count} non-stackable {baseItem.Name}");

                var addedItems = _items.GetRange(_items.Count - canAdd, canAdd);
                foreach (var item in addedItems)
                    OnItemAdded?.Invoke(item);

                OnCapacityChanged?.Invoke(UsedSlots, MaxCapacity);
                return;
            }

            AddStackableItems(baseItem, count);
        }
        public bool Remove(ItemData item)
        {
            if (item == null || !Items.Contains(item))
            {
                Debug.LogWarning("[Inventory] Failed to remove item: null or not found");
                return false;
            }

            _items.Remove(item);
            RemoveFromIndex(item);

            Debug.Log($"[Inventory] Removed stack {item.Source.Name} x{item.Count}");
            OnItemRemoved?.Invoke(item);
            OnCapacityChanged?.Invoke(UsedSlots, MaxCapacity);

            return true;
        }
        public int RemoveFromStack(ItemData stack, int count=1)
        {
            if (stack == null || count <= 0 || !Items.Contains(stack))
            {
                Debug.LogWarning("[Inventory] Failed RemoveFromStack: invalid stack");
                return 0;
            }

            if (count >= stack.Count)
            {
                var removed = stack.Count;
                Remove(stack);
                return removed;
            }

            stack.Count -= count;
            Debug.Log($"[Inventory] Reduced {stack.Source.Name} stack by {count}");
            OnItemRemoved?.Invoke(stack);
            OnCapacityChanged?.Invoke(UsedSlots, MaxCapacity);
            return count;
        }
        public int Remove(BaseItem baseItem, int count)
        {
            if (baseItem == null || count <= 0 || !_index.TryGetValue(baseItem, out var stacks))
            {
                Debug.LogWarning($"[Inventory] Failed to remove {count} {baseItem?.Name}: invalid params");
                return 0;
            }

            var remainingToRemove = count;
            var emptyStacks = new List<ItemData>();
            var modifiedStacks = new List<ItemData>();

            foreach (var stack in stacks)
            {
                if (remainingToRemove <= 0)
                    break;

                var toTake = Mathf.Min(remainingToRemove, stack.Count);
                stack.Count -= toTake;
                remainingToRemove -= toTake;

                modifiedStacks.Add(stack);

                if (stack.Count == 0)
                    emptyStacks.Add(stack);
            }

            foreach (var empty in emptyStacks)
            {
                _items.Remove(empty);
                RemoveFromIndex(empty);
                OnItemRemoved?.Invoke(empty);
            }

            foreach (var modified in modifiedStacks)
            {
                if (modified.Count > 0)
                    OnItemRemoved?.Invoke(modified);
            }

            var removedCount = count - remainingToRemove;
            Debug.Log($"[Inventory] Removed {removedCount}/{count} {baseItem.Name}");
            OnCapacityChanged?.Invoke(UsedSlots, MaxCapacity);

            return removedCount;
        }
        public void Clear()
        {
            foreach (var item in _items)
                OnItemRemoved?.Invoke(item);

            _items.Clear();
            _index.Clear();
            Debug.Log("[Inventory] Cleared");
            OnCapacityChanged?.Invoke(UsedSlots, MaxCapacity);
        }
        private bool ValidateAddRequest(BaseItem baseItem, int count)
        {
            if (baseItem == null)
            {
                Debug.LogWarning("[Inventory] Attempted to add null item");
                return false;
            }

            if (count <= 0)
            {
                Debug.LogWarning($"[Inventory] Invalid count: {count}");
                return false;
            }

            if (baseItem.MaxStack < 0)
            {
                Debug.LogWarning($"[Inventory] Invalid MaxStack ({baseItem.MaxStack}) for item: {baseItem.Name}");
                return false;
            }

            return true;
        }
        private void AddNonStackableItems(BaseItem baseItem, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var item = CreateItemData(baseItem, 1);
                AddRaw(item);
            }
        }
        private void AddStackableItems(BaseItem baseItem, int count)
        {
            int maxStack = baseItem.MaxStack;
            int toAdd = count;
            var modifiedStacks = new List<ItemData>();

            if (_index.TryGetValue(baseItem, out var existingStacks))
            {
                foreach (var stack in existingStacks)
                {
                    if (toAdd <= 0) break;

                    int space = maxStack - stack.Count;
                    if (space <= 0) continue;

                    int add = Mathf.Min(toAdd, space);
                    stack.Count += add;
                    toAdd -= add;
                    modifiedStacks.Add(stack);
                }
            }

            while (toAdd > 0 && FreeSlots > 0)
            {
                int stackSize = Mathf.Min(toAdd, maxStack);
                var newItem = CreateItemData(baseItem, stackSize);
                AddRaw(newItem);
                modifiedStacks.Add(newItem);
                toAdd -= stackSize;
            }

            int added = count - toAdd;
            int rejected = toAdd;

            Debug.Log($"[Inventory] Added {added}/{count} {baseItem.Name}");

            foreach (var item in modifiedStacks)
                OnItemAdded?.Invoke(item);

            if (rejected > 0)
            {
                Debug.LogWarning($"[Inventory] Rejected {rejected} {baseItem.Name}: inventory full");
                OnItemRejected?.Invoke(baseItem, rejected);
            }

            OnCapacityChanged?.Invoke(UsedSlots, MaxCapacity);
        }
        private ItemData CreateItemData(BaseItem source, int count) => new()
        {
            Source = source,
            Count = count
        };
        private void AddRaw(ItemData item)
        {
            _items.Add(item);
            AddToIndex(item);
        }
        private void AddToIndex(ItemData item)
        {
            if (item.Source == null) return;

            if (!_index.TryGetValue(item.Source, out var list))
            {
                list = new List<ItemData>();
                _index[item.Source] = list;
            }
            list.Add(item);
        }
        private void RemoveFromIndex(ItemData item)
        {
            if (item.Source == null || !_index.TryGetValue(item.Source, out var list))
                return;

            list.Remove(item);
            if (list.Count == 0)
                _index.Remove(item.Source);
        }
    }
}