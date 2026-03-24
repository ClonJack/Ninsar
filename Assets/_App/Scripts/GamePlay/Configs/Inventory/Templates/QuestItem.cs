using UnityEngine;

namespace Ninsar.Inventory
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