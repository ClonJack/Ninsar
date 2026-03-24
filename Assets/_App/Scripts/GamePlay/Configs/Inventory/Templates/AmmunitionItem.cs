using Ninsar.Inventory;
using UnityEngine;

namespace Ninsar.Configs.Inventory
{
    [CreateAssetMenu(fileName = "Name--(Item---Ammunition)", menuName = "Inventory/Templates/Ammunition", order = 0)]
    public class AmmunitionItem : BaseItem
    {
        public void Reset()
        {
            Property.Add(new ArmorComponent());
            EItemCategory = EItemCategory.Equipment;
        }
    }
}