using System;
using System.Threading.Tasks;

namespace FYF.MapBuilder.Client
{
    internal interface IAccessor
    {
        void RegisterEvent(string eventName, Delegate callback);

        void OnRenderTick(Func<Task> tick);
        void OnUpdateTick(Func<Task> tick);
        void OnScheduledTick(Func<Task> tick, int delayInMilliseconds);
    }
}
