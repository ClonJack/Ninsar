using UnityEngine;

namespace Ninsar.Inventory
{
    [CreateAssetMenu(fileName = "Name--(Item--- Consumable)", menuName = "Inventory/Templates/Consumable", order = 0)]
    public class ConsumableItem : BaseItem
    {
        public void Reset()
        {
            EItemCategory = EItemCategory.Consumable;
        }
    }
}