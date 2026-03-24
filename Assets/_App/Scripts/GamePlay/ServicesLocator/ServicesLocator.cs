using System;
using System.Collections.Generic;

namespace Ninsar.Locator
{
    public class ServicesLocator
    {
        private static ServicesLocator _instance;

        private readonly Dictionary<Type, object> _services = new();
        
        public static ServicesLocator Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ServicesLocator();

                return _instance;
            }
        }
        private ServicesLocator() { }

        public ServicesLocator Registration<T>(T service)
        {
            var serviceType = typeof(T);
            _services.TryAdd(serviceType, service);
            return this;
        }

        public T GetServices<T>()
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }

            throw new Exception($"Service: {typeof(T).Name} is not found!");
        }
    }
}