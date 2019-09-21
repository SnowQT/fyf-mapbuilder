using CitizenFX.Core;
using System;
using System.Collections.Generic;

namespace FYF.MapBuilder.Client
{
    public class ServiceLocator
    {
        private Dictionary<Type, dynamic> services = new Dictionary<Type, dynamic>();

        public void RegisterService<T>(T instance) where T : class
        {
            if (services.ContainsKey(typeof(T)))
            {
                Debug.WriteLine($"Service {typeof(T).Name} is already registered.");
                return;
            }

            services.Add(typeof(T), instance);
        }

        //@TODO #wait-for-creation-service: Possible "wait for it to exist" + timeout thing to avoid race conditions. 
        public T GetService<T>() where T : class
        {
            bool foundService = services.TryGetValue(typeof(T), out dynamic service);

            if (!foundService)
            {
                Debug.WriteLine($"Service {typeof(T).Name} cannot be found.");
                return default(T);
            }

            return service as T;
        }

    }
}
