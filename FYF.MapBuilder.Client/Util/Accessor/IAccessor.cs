using System;
using System.Threading.Tasks;

namespace FYF.MapBuilder.Client
{
    internal interface IAccessor
    {
        void RegisterEvent(string eventName, Delegate callback);
        void RegisterTick(Func<Task> tick);

        ServiceLocator GetLocator();
    }
}
