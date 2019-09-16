using System;

namespace FYF.MapBuilder.Client
{
    internal interface IAccessor
    {
        void RegisterEvent(string eventName, Delegate callback);
    }
}
