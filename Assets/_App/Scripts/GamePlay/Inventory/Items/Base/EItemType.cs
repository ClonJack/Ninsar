using System;

namespace Ninsar.Inventory
{
    [Flags]
    public enum EItemCategory
    {
        Equipment = 1 << 0,
        Consumable = 1 << 1,
        Quest = 1 << 2
    }
}