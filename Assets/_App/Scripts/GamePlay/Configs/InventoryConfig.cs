using Ninsar.Inventory;
using UnityEngine;

namespace Ninsar.Configs.Inventory.UI
{
    [CreateAssetMenu(fileName = "GamePrefabs", menuName = "Inventory/InventoryConfig", order = 0)]
    public class InventoryConfig : ScriptableObject
    {
        [field: SerializeField]
        [field: Header("Inventory")]
        public ItemView ItemPrefab { get; private set; }

        [field: SerializeField] 
        public int MaxItems { get; private set; } = 10;
    }
}