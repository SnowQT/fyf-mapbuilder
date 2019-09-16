using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace FYF.MapBuilder.Client
{
    public class MapBuilderClient : BaseScript, IAccessor
    {
        private static IAccessor _accessor;
        internal static IAccessor Accessor
        {
            get
            {
                return _accessor;
            }
        }

        private bool IsUserInBuildMode = false;
        private Freecam freeCam;
        private UserInterface ui;

        public MapBuilderClient()
        {
            _accessor = this;

            FreecamConfig config = new FreecamConfig
            {
                FieldOfView = 75,
                PositionSensitivity = 1.0f,
                PositionBase = 100.0f,
                RotationSensitivity = 2.0f,
                RotationBase = 500.0f,
                KeySmoothTime = 500,
            };

            freeCam = new Freecam(config);
            ui = new UserInterface();

            Tick += OnTick;

            EventHandlers.Add("onResourceStop", new Action<string>(OnResourceStopped));
        }

        private async Task OnTick()
        {
            if (IsUserInBuildMode)
            {
                freeCam.Update();
                ui.Update();
            }

            await Task.FromResult(0);
        }

        [Command("build")]
        void CommandBuildToggle()
        {
            if (IsUserInBuildMode)
            {
                freeCam.DisableFreecam();
                ui.Close();

                IsUserInBuildMode = false;
            }
            else
            {
                freeCam.EnableFreecam();
                IsUserInBuildMode = true;
            }
        }

        /// <summary>
        ///     Disable the freecam if the resource is stopped.
        ///     If we don't do this, the camera won't be disposed properly and get stuck in freecam mode forever.
        /// </summary>
        /// <param name="resource">The resource name that is currently being stopped.</param>
        void OnResourceStopped(string resource)
        {
            if (resource.Equals(GetCurrentResourceName(), StringComparison.Ordinal))
            {
                if (freeCam != null)
                {
                    freeCam.DisableFreecam();
                }

                if (ui != null)
                {
                    ui.Close();
                }

            }
        }

        #region IAccessor

        public void RegisterEvent(string eventName, Delegate callback)
        {
            EventHandlers.Add(eventName, callback);
        }

        #endregion
    }
}
 