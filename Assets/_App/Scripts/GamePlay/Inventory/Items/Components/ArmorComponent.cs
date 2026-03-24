using System;

namespace Ninsar.Inventory
{
    [Serializable]
    public class ArmorComponent : IItemComponent
    {
        public float Value;
        public string Description => $"Armor:({Value})";
    }
}