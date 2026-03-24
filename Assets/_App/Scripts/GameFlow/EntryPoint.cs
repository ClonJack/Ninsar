using Ninsar.Configs.Inventory;
using Ninsar.Inventory;
using UnityEngine;

namespace Ninsar
{
    public class EntryPoint : MonoBehaviour
    {
        [Header("Configuration")] 
        [SerializeField]
        private InventoryConfig _inventoryConfig;
        
        private InventoryStorage _inventory;
        public void Awake()
        {
            Init();
        }
        
        private void Init()
        {
            _inventory = new InventoryStorage(_inventoryConfig.StartCapacity);
            
            ServicesLocator.Instance.Registration(_inventoryConfig);
            ServicesLocator.Instance.Registration(_inventory);
        }
    }
    
}