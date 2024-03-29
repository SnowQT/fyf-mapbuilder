﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System.Linq;

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
                MethodName = method.Method.Name,
                Method = method
            };

            renderTicks.Add(info);
        }

        public void OnUpdateTick(Func<Task> method)
        {
            TickInfo info = new TickInfo()
            {
                MethodName = method.Method.Name,
                Method = method
            };

            updateTicks.Add(info);
        }

        public void OnScheduledTick(Func<Task> method, int delayInMilliseconds)
        {
            Tick += new Func<Task>(() =>
            {
                Profiler.Enter("ScheduledTick_" + method.Method.Name);

                method.Invoke();

                Profiler.Exit();

                return Delay(delayInMilliseconds);
            });
        }

        private async Task RenderTicks()
        {
            Profiler.Enter("RenderTicks");

            //@TODO(bma): Can we cache this .Select upon registering a new tick?
            var tasks = renderTicks.Select(async task =>
            {
                Profiler.Enter("RenderTick_" + task.MethodName);

                await task.Method.Invoke();

                Profiler.Exit();
            });

            await Task.WhenAll(tasks);

            Profiler.Exit();

            await Task.FromResult(0);
        }

        private async Task UpdateTicks()
        {
            Profiler.Enter("UpdateTicks");

            //@TODO(bma): Can we cache this .Select upon registering a new tick?
            var tasks = updateTicks.Select(async task =>
            {
                Profiler.Enter("RenderTick_" + task.MethodName);

                await task.Method.Invoke();

                Profiler.Exit();
            });

            await Task.WhenAll(tasks);

            Profiler.Exit();

            await Delay(0);
        }
    }
}
