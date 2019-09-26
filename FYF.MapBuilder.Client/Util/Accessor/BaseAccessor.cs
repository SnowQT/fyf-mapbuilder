using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace FYF.MapBuilder.Client
{
    public struct TickInfo
    {
        public string MethodName;
        public Func<Task> Method;
    }

    public class BaseAccessor : BaseScript, IAccessor
    {
        private HashSet<TickInfo> renderTicks = new HashSet<TickInfo>();
        private HashSet<TickInfo> updateTicks = new HashSet<TickInfo>();

        public BaseAccessor()
        {
            Tick += RenderTicks;
            Tick += UpdateTicks;
        }
        public void RegisterEvent(string eventName, Delegate callback)
        {
            EventHandlers.Add(eventName, callback);
        }

        public void OnRenderTick(Func<Task> method)
        {
            TickInfo info = new TickInfo()
            {
                MethodName = method.GetType().Name,
                Method = method
            };

            renderTicks.Add(info);
        }

        public void OnUpdateTick(Func<Task> method)
        {
            TickInfo info = new TickInfo()
            {
                MethodName = method.GetType().Name,
                Method = method
            };

            updateTicks.Add(info);
        }

        public void OnScheduledTick(Func<Task> method, int delayInMilliseconds)
        {
            string methodName = method.GetType().Name;

            Tick += new Func<Task>(() =>
            {
                ProfilerEnterScope("ScheduledTick_" + methodName);

                method.Invoke();

                ProfilerExitScope();

                return Delay(delayInMilliseconds);
            });
        }

        async Task RenderTicks()
        {
            //@TODO: #tick-parallel: Could we invoke these task using Parallel.Invoke?
            foreach (var info in renderTicks)
            {
                ProfilerEnterScope("RenderTick_" + info.MethodName);

                await info.Method.Invoke();

                ProfilerExitScope();
            }

            await Task.FromResult(0);
        }

        async Task UpdateTicks()
        {
            //@TODO: #tick-parallel: Could we invoke these task using Parallel.Invoke?
            foreach (var info in updateTicks)
            {
                ProfilerEnterScope("UpdateTick_" + info.MethodName);

                await info.Method.Invoke();

                ProfilerExitScope();
            }

            await Delay(0);
        }
    }
}
