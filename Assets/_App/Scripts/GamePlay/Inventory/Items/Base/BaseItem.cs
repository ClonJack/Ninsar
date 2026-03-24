using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ninsar.Inventory
{
    public class BaseItem : ScriptableObject
    {
        [field: Header("Refs")]
        [field: SerializeField]
        public Sprite Icon { get; protected set; }
        
        [field: SerializeField] 
        public string Name { get; protected set; }
        
        [field: Header("Settings")]
        [field: SerializeField] 
        public int MaxStack { get; protected set; } = 1;
        
        [field: SerializeField]
        public EItemCategory EItemCategory { get; protected set; }
        
        [field: SerializeField] 
        [field: Multiline(3)] 
        public string Description { get; protected set; }
        
        [SerializeReference, SubclassSelector]
        public List<IItemComponent> Property = new();

#if UNITY_EDITOR
        private void OnValidate()
        {
            RemoveDuplicateComponents();
        }
        private void RemoveDuplicateComponents()
        {
            if (Property == null || Property.Count <= 1)
                return;

            var seen = new HashSet<Type>();
            var toRemove = new List<IItemComponent>();

            foreach (var component in Property)
            {
                if (component == null)
                    continue;

                var type = component.GetType();
                
                if (seen.Contains(type))
                {
                    toRemove.Add(component);
                    Debug.LogWarning($"[BaseItem] Removed duplicate component: {type.Name} from {Name}");
                }
                else
                {
                    seen.Add(type);
                }
            }

            foreach (var duplicate in toRemove)
            {
                Property.Remove(duplicate);
            }
        }
#endif
    }
}