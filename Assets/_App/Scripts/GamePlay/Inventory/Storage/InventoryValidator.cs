using UnityEngine;

namespace Ninsar.Inventory
{
    public class InventoryValidator
    {
        public ValidationResult ValidateAddRequest(BaseItem baseItem, int count)
        {
            if (baseItem == null)
            {
                Debug.LogWarning("[Inventory] Attempted to add null item");
                return new ValidationResult { IsValid = false, Error = "Null item" };
            }

            if (count <= 0)
            {
                Debug.LogWarning($"[Inventory] Invalid count: {count}");
                return new ValidationResult { IsValid = false, Error = $"Invalid count: {count}" };
            }

            if (baseItem.MaxStack < 0)
            {
                Debug.LogWarning($"[Inventory] Invalid MaxStack ({baseItem.MaxStack}) for item: {baseItem.Name}");
                return new ValidationResult { IsValid = false, Error = $"Invalid MaxStack: {baseItem.MaxStack}" };
            }

            return new ValidationResult { IsValid = true };
        }
    }

    public struct ValidationResult
    {
        public bool IsValid;
        public string Error;
    }
}