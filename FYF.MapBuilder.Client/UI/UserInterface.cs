using static CitizenFX.Core.Native.API;

namespace FYF.MapBuilder.Client
{
    internal class UserInterface
    {
        const int WeaponWheelInputGroup = 24;
        const int OpenUIKey = 37;

        private int[] InterfaceKeys = new int[] {
            OpenUIKey,
            261,
            262
        };

        private bool OpenState = false;
        private NuiHelper nui;

        public UserInterface()
        {
            nui = new NuiHelper();
            nui.AddToggle(OpenUIKey, "9", Open, Close);
        }

        internal void Update()
        {
            //Disable any keys that might interfere with the Tab key.
            foreach (int key in InterfaceKeys)
            {
                DisableControlAction(0, key, true);
            }

            nui?.UpdateToggles();
        }

        public void Close()
        {
            nui.SendMessage("close");
            SetNuiFocus(false, false);

            //Re-enable any keys that got disabled.
            // I suspect this resets automatically when the update loop is stopped.
            foreach (int key in InterfaceKeys)
            {
                EnableControlAction(0, key, true);
            }

            OpenState = false;
        }

        public void Open()
        {
            nui.SendMessage("open");
            SetNuiFocus(false, true);

            OpenState = true;
        }
    }
}
