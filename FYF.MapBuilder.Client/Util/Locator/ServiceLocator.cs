using System;
using System.Collections.Generic;

using CitizenFX.Core;

namespace FYF.MapBuilder.Client
{
    public struct ServiceReference<T> where T : class
    {
        private ServiceLocator locator;
        private T serviceObject;

        public ServiceReference(ServiceLocator locator)
        {
            this.locator = locator;
            this.serviceObject = default(T);
        }

        public ServiceReference(ServiceLocator locator, T reference)
        {
            this.locator = locator;
            this.serviceObject = reference;
        }

        public T Get()
        {
            if (serviceObject == null)
            {
                serviceObject = locator.GetService<T>();
                return serviceObject;
            }
            else
            {
                return serviceObject;
            }
        }
    }

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

        public T CreateService<T>(params object[] args) where T : class
        {
            if (services.ContainsKey(typeof(T)))
            {
                Debug.WriteLine($"Service {typeof(T).Name} is already registered.");
                return null;
            }

            T instance = (T)Activator.CreateInstance(typeof(T), args);
            services.Add(typeof(T), instance);

            return instance;
        }

        public T GetService<T>() where T : class
        {
            bool foundService = services.TryGetValue(typeof(T), out dynamic service);

            if (!foundService)
            {
                Debug.WriteLine($"Service {typeof(T).Name} cannot be found.");
                return null;
            }

            return service as T;
        }

        //Returns a reference to a service which may or may not exist (yet).
        public ServiceReference<T> GetServiceReference<T>() where T : class
        {
            Debug.WriteLine("Resolving reference for: " + typeof(T).Name);
            T service = GetService<T>();

            if (service != null)
            {
                return new ServiceReference<T>(this, service);
            }
            else
            {
                return new ServiceReference<T>(this);
            }
        }
    }
}