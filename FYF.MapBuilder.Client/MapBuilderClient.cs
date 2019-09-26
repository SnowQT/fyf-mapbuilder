using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace FYF.MapBuilder.Client
{
    public class MapBuilderClient : BaseAccessor
    {
        private static IAccessor _accessor;
        internal static IAccessor Accessor
        {
            get
            {
                return _accessor;
            }
        }

        private static ServiceLocator locator = new ServiceLocator();
        public static ServiceLocator Locator
        {
            get { return locator; }
            set { locator = value; }
        }

        //@TODO(bma) #state-manager: Should this really be in the main function? Couldn't we subjugate this a specialized class that manages the states?
        //@TODO(bma) #state-manager: Possible make use of a state manager that we can call from the service locator. So this doesn't need to be static.
        public static bool IsUserInBuildMode = false;

        private Input input;
        private Freecam freeCam;
        private UserInterface ui;
        private Builder builder;

        public MapBuilderClient() : base()
        {
            _accessor = this;

            FreecamConfig config = new FreecamConfig
            {
                FieldOfView = 75,
                PositionSensitivity = 1.0f,
                PositionBase = 100.0f,
                RotationSensitivity = 1.0f,
                RotationBase = 500.0f,
                KeySmoothTime = 500,
            };

            input = locator.CreateService<Input>();
            //@TODO(bma) #state-manager: Move this to #move-toggle-to-class.
            input.RegisterKey(0, 37, InputKeyType.Once);
            input.RegisterKey(0, 261, InputKeyType.Once);
            input.RegisterKey(0, 262, InputKeyType.Once);

            freeCam = locator.CreateService<Freecam>(config);
            ui = locator.CreateService<UserInterface>();
            builder = locator.CreateService<Builder>();

            EventHandlers.Add("onResourceStop", new Action<string>(OnResourceStopped));
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
    }
}
 