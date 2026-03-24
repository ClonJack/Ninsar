using Ninsar.Inventory;
using UnityEngine;

namespace Ninsar.Configs.Inventory
{
    [CreateAssetMenu(fileName = "Name--(Item---Quest)", menuName = "Inventory/Templates/Quest", order = 0)]
    public class QuestItem : BaseItem
    {
        public void Reset()
        {
            EItemCategory = EItemCategory.Quest;
        }
    }
}