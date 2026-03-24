using System;
using System.Collections.Generic;

namespace Ninsar.Inventory
{
    [Serializable]
    public class ItemData
    {
        public BaseItem Source;
        public int Count = 1;

        private Dictionary<Type, IItemComponent> _componentsCache;

        public bool IsStackable => Source != null && Source.MaxStack > 1;

        public void Build()
        {
            _componentsCache = new Dictionary<Type, IItemComponent>();

            if (Source == null)
                return;

            foreach (var component in Source.Property)
            {
                if (component == null)
                    continue;

                _componentsCache[component.GetType()] = component;
            }
        }
        public bool TryGet<T>(out T result) where T : class, IItemComponent
        {
            if (_componentsCache == null)
                Build();

            if (_componentsCache != null &&
                _componentsCache.TryGetValue(typeof(T), out var comp))
            {
                result = comp as T;
                return true;
            }

            result = null;
            return false;
        }
    }
}