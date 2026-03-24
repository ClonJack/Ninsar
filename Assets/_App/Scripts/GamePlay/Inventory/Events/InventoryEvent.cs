namespace Ninsar.Inventory.Enum
{
    public struct InventoryEvent
    {
        public EInventoryEventType EInventoryEvent;
        public ItemData Item;
        public BaseItem BaseItem;
        public int Count;
        public int UsedSlots;
        public int MaxCapacity;

        public static InventoryEvent StackAdded(ItemData item) => new()
        {
            EInventoryEvent = EInventoryEventType.StackAdded,
            Item = item
        };

        public static InventoryEvent StackCountChanged(ItemData item) => new()
        {
            EInventoryEvent = EInventoryEventType.StackCountChanged,
            Item = item
        };

        public static InventoryEvent StackRemoved(ItemData item) => new()
        {
            EInventoryEvent = EInventoryEventType.StackRemoved,
            Item = item
        };

        public static InventoryEvent ItemsRejected(BaseItem item, int count) => new()
        {
            EInventoryEvent = EInventoryEventType.ItemsRejected,
            BaseItem = item,
            Count = count
        };

        public static InventoryEvent ItemsDropped(ItemData item) => new()
        {
            EInventoryEvent = EInventoryEventType.ItemsDropped,
            Item = item
        };

        public static InventoryEvent CapacityChanged(int used, int max) => new()
        {
            EInventoryEvent = EInventoryEventType.CapacityChanged,
            UsedSlots = used,
            MaxCapacity = max
        };

        public static InventoryEvent AddFailed(BaseItem item, int count) => new()
        {
            EInventoryEvent = EInventoryEventType.AddFailed,
            BaseItem = item,
            Count = count
        };
    }
}