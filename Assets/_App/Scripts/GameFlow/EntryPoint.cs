using System.Collections.Generic;
using Ninsar.Configs.Inventory.UI;
using Ninsar.Inventory;
using Ninsar.Locator;
using UnityEngine;
using UnityEngine.Serialization;

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
            _inventory = new InventoryStorage(_inventoryConfig.MaxItems);
            
            ServicesLocator.Instance.Registration(_inventoryConfig);
            ServicesLocator.Instance.Registration(_inventory);
        }
    }
    
}