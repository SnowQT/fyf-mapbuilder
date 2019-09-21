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

        //@TODO #move-toggle-to-class: Should this really be in the main function? Couldn't we subjugate this a specialized class that manages the states?
        //@TODO #state-manager: Possible make use of a state manager that we can call from the service locator. So this doesn't need to be static.
        public static bool IsUserInBuildMode = false;
        private ServiceLocator locator = new ServiceLocator();

        private Input input;
        private Freecam freeCam;
        private UserInterface ui;
        private Builder builder;

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

            input = new Input();
            locator.RegisterService(input);

            //@TODO: Use generic parameter to instantiate these classes. i.e CreateService<T>(params object[] args);
            freeCam = new Freecam(config);
            locator.RegisterService(freeCam);

            ui = new UserInterface();

            //@TODO: Move this to #move-toggle-to-class.
            input.RegisterKey(0, 37, InputKeyType.Once);
            input.RegisterKey(0, 261, InputKeyType.Once);
            input.RegisterKey(0, 262, InputKeyType.Once);

            locator.RegisterService(ui);

            builder = new Builder();
            locator.RegisterService(builder);


            Tick += OnTick;

            EventHandlers.Add("onResourceStop", new Action<string>(OnResourceStopped));
        }

        private async Task OnTick()
        {
            if (IsUserInBuildMode)
            {
                //@TODO: This should use the RegisterTick from IAccessor.
                freeCam.Update();
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

                input.EnableKey(0, 37);
                input.EnableKey(0, 261);
                input.EnableKey(0, 262);

                IsUserInBuildMode = false;

            }
            else
            {
                freeCam.EnableFreecam();

                input.DisableKey(0, 37);
                input.DisableKey(0, 261);
                input.DisableKey(0, 262);

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

        public ServiceLocator GetLocator()
        {
            return locator;
        }

        public void RegisterTick(Func<Task> tick)
        {
            Tick += tick;
        }

        #endregion
    }
}
 