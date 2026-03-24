using System;
using System.Collections.Generic;
using System.Linq;
using Ninsar.Inventory.Enum;
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
        private readonly InventoryIndex _index = new();
        private readonly InventoryEventDispatcher _dispatcher = new();
        private readonly InventoryValidator _validator = new();
        
        private readonly List<ItemData> _tempList1 = new();
        private readonly List<ItemData> _tempList2 = new();

        public event Action<InventoryEvent> OnInventoryChanged
        {
            add => _dispatcher.OnInventoryChanged += value;
            remove => _dispatcher.OnInventoryChanged -= value;
        }

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
            
            if (UsedSlots > MaxCapacity)
                DropExcessItems();

            Debug.Log($"[Inventory] Capacity changed to {MaxCapacity} slots");
            _dispatcher.NotifyCapacityChanged(UsedSlots, MaxCapacity);
        }
        public void Add(BaseItem baseItem, int count = 1)
        {
            var validation = _validator.ValidateAddRequest(baseItem, count);
            if (!validation.IsValid)
                return;

            if (baseItem.MaxStack <= 1)
                AddNonStackableItems(baseItem, count);
            else
                AddStackableItems(baseItem, count);
        }
        public bool Remove(ItemData item)
        {
            if (item == null || !Items.Contains(item))
            {
                Debug.LogWarning("[Inventory] Failed to remove item: null or not found");
                return false;
            }

            RemoveItemInternal(item);
            return true;
        }
        public int RemoveFromStack(ItemData stack, int count = 1)
        {
            if (stack == null || count <= 0 || !Items.Contains(stack))
            {
                Debug.LogWarning("[Inventory] Failed RemoveFromStack: invalid stack");
                return 0;
            }

            if (count >= stack.Count)
            {
                var removed = stack.Count;
                RemoveItemInternal(stack);
                return removed;
            }

            stack.Count -= count;
            Debug.Log($"[Inventory] Reduced {stack.Source.Name} stack by {count}");
            
            _dispatcher.NotifyStackCountChanged(stack);
            _dispatcher.NotifyCapacityChanged(UsedSlots, MaxCapacity);
            
            return count;
        }
        public int Remove(BaseItem baseItem, int count)
        {
            if (baseItem == null || count <= 0 || !_index.TryGetStacks(baseItem, out var stacks))
            {
                Debug.LogWarning($"[Inventory] Failed to remove {count} {baseItem?.Name}: invalid params");
                return 0;
            }

            return RemoveFromStacks(stacks, count, baseItem);
        }
        public void Clear()
        {
            foreach (var item in _items)
                _dispatcher.NotifyStackRemoved(item);

            _items.Clear();
            _index.Clear();
            Debug.Log("[Inventory] Cleared");
            _dispatcher.NotifyCapacityChanged(UsedSlots, MaxCapacity);
        }
        private void AddNonStackableItems(BaseItem baseItem, int count)
        {
            var canAdd = Math.Min(count, FreeSlots);
            var rejected = count - canAdd;

            if (rejected > 0)
                _dispatcher.NotifyItemsRejected(baseItem, rejected);

            if (canAdd <= 0)
            {
                _dispatcher.NotifyAddFailed(baseItem, count);
                return;
            }

            var startIndex = _items.Count;
            
            for (var i = 0; i < canAdd; i++)
            {
                var item = new ItemData { Source = baseItem, Count = 1 };
                _items.Add(item);
                _index.Add(item);
            }
            
            Debug.Log($"[Inventory] Added {canAdd}/{count} non-stackable {baseItem.Name}");

            for (var i = startIndex; i < _items.Count; i++)
                _dispatcher.NotifyStackAdded(_items[i]);

            _dispatcher.NotifyCapacityChanged(UsedSlots, MaxCapacity);
        }
        private void AddStackableItems(BaseItem baseItem, int count)
        {
            var maxStack = baseItem.MaxStack;
            var toAdd = count;
            
            _tempList1.Clear();
            var modifiedStacks = _tempList1;

            if (_index.TryGetStacks(baseItem, out var existingStacks))
            {
                foreach (var stack in existingStacks)
                {
                    if (toAdd <= 0) break;

                    var space = maxStack - stack.Count;
                    if (space <= 0) continue;

                    var add = Mathf.Min(toAdd, space);
                    stack.Count += add;
                    toAdd -= add;
                    modifiedStacks.Add(stack);
                }
            }

            var stacksBefore = _items.Count;

            while (toAdd > 0 && FreeSlots > 0)
            {
                var stackSize = Mathf.Min(toAdd, maxStack);
                var newItem = new ItemData { Source = baseItem, Count = stackSize };
                _items.Add(newItem);
                _index.Add(newItem);
                modifiedStacks.Add(newItem);
                toAdd -= stackSize;
            }

            var added = count - toAdd;
            var rejected = toAdd;

            Debug.Log($"[Inventory] Added {added}/{count} {baseItem.Name}");

            foreach (var item in modifiedStacks)
            {
                if (_items.IndexOf(item) >= stacksBefore)
                    _dispatcher.NotifyStackAdded(item);
                else
                    _dispatcher.NotifyStackCountChanged(item);
            }

            if (rejected > 0)
                _dispatcher.NotifyItemsRejected(baseItem, rejected);

            _dispatcher.NotifyCapacityChanged(UsedSlots, MaxCapacity);
        }
        private int RemoveFromStacks(List<ItemData> stacks, int count, BaseItem baseItem)
        {
            _tempList1.Clear();
            _tempList2.Clear();
            
            var emptyStacks = _tempList1;
            var modifiedStacks = _tempList2;

            var remainingToRemove = count;

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
                RemoveItemInternal(empty);

            foreach (var modified in modifiedStacks)
            {
                if (modified.Count > 0)
                    _dispatcher.NotifyStackCountChanged(modified);
            }

            var removedCount = count - remainingToRemove;
            Debug.Log($"[Inventory] Removed {removedCount}/{count} {baseItem.Name}");
            
            return removedCount;
        }
        private void RemoveItemInternal(ItemData item)
        {
            _items.Remove(item);
            _index.Remove(item);

            Debug.Log($"[Inventory] Removed stack {item.Source.Name} x{item.Count}");
            _dispatcher.NotifyStackRemoved(item);
            _dispatcher.NotifyCapacityChanged(UsedSlots, MaxCapacity);
        }
        private void DropExcessItems()
        {
            var excessCount = UsedSlots - MaxCapacity;
            Debug.LogWarning($"[Inventory] Capacity reduced! Dropping {excessCount} items");

            for (var i = 0; i < excessCount; i++)
            {
                var lastIndex = _items.Count - 1;
                var item = _items[lastIndex];

                _items.RemoveAt(lastIndex);
                _index.Remove(item);

                Debug.LogWarning($"[Inventory] Dropped: {item.Source.Name} x{item.Count}");
                _dispatcher.NotifyItemsDropped(item);
                _dispatcher.NotifyStackRemoved(item);
            }
        }
        
    }
}