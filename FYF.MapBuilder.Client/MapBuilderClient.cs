using System;

using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace FYF.MapBuilder.Client
{
    public class MapBuilderClient : BaseScript
    {
        private Freecam freeCam;

        public MapBuilderClient()
        {
            freeCam = new Freecam();
            Tick += freeCam.Update;

            EventHandlers.Add("onResourceStop", new Action<string>(OnResourceStopped));
        }

        [Command("freecam")]
        void CommandFreecamToggle()
        {
            if (freeCam == null)
            {
                return;
            }

            if (!freeCam.Enabled)
            {
                freeCam.Enable();
            }
            else
            {
                freeCam.Disable();
            }     
        }

        /// <summary>
        ///     Disable the freecam if the resource is stopped.
        /// </summary>
        /// <param name="resource">The resource name that is currently being stopped.</param>
        void OnResourceStopped(string resource)
        {
            if (!resource.Equals(GetCurrentResourceName(), StringComparison.Ordinal))
            {
                return;
            }

            if (freeCam == null)
            {
                return;
            }

            if (freeCam.Enabled)
            {
                freeCam.Disable();
            }
        }
    }
}
 