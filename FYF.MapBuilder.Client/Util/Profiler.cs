using System.Diagnostics;
using static CitizenFX.Core.Native.API;

namespace FYF.MapBuilder.Client
{
    public static class Profiler
    {
        [Conditional("PROFILE")]
        public static void Enter(string name)
        {
            ProfilerEnterScope(name);
        }

        [Conditional("PROFILE")]
        public static void Exit()
        {
            ProfilerExitScope();
        }
    }
}
