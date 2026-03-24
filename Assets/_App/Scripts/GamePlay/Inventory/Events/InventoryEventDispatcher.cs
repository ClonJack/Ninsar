using System;
using Ninsar.Inventory.Enum;

namespace Ninsar.Inventory
{
    public class InventoryEventDispatcher
    {
        public event Action<InventoryEvent> OnInventoryChanged;
        public void Notify(InventoryEvent inventoryEvent)
        {
            OnInventoryChanged?.Invoke(inventoryEvent);
        }
        public void NotifyStackAdded(ItemData item)
        {
            Notify(InventoryEvent.StackAdded(item));
        }
        public void NotifyStackCountChanged(ItemData item)
        {
            Notify(InventoryEvent.StackCountChanged(item));
        }
        public void NotifyStackRemoved(ItemData item)
        {
            Notify(InventoryEvent.StackRemoved(item));
        }
        public void NotifyItemsRejected(BaseItem item, int count)
        {
            Notify(InventoryEvent.ItemsRejected(item, count));
        }
        public void NotifyItemsDropped(ItemData item)
        {
            Notify(InventoryEvent.ItemsDropped(item));
        }
        public void NotifyCapacityChanged(int used, int max)
        {
            Notify(InventoryEvent.CapacityChanged(used, max));
        }
        public void NotifyAddFailed(BaseItem item, int count)
        {
            Notify(InventoryEvent.AddFailed(item, count));
        }
    }
}