using Ninsar.Inventory;
using UnityEngine;

namespace Ninsar.Configs.Inventory
{
    [CreateAssetMenu(fileName = "Name--(Item--Weapon)", menuName = "Inventory/Templates/Weapon", order = 0)]
    public class WeaponItem : BaseItem
    { 
        public void Reset()
        {
            Property.Add(new DamageComponent());
            EItemCategory = EItemCategory.Equipment;
        }
    }
}