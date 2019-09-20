using CitizenFX.Core;
using System;
using System.Collections.Generic;
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

        private IAccessor accessor;
        private NuiHelper nui;

        public UserInterface()
        {
            accessor = MapBuilderClient.Accessor;

            nui = new NuiHelper();
            nui.AddToggle(OpenUIKey, "9", Open, Close);
            nui.AddCallback("Browser_OnObjectChanged", Browser_OnObjectChanged);
        }

        //@TODO: Can't we just bind this behavior directly to the Builder or BuilderObjectManager?
        void Browser_OnObjectChanged(dynamic args)
        {
            string name = (string)args.name;
            var locator = accessor.GetLocator();
            Builder builder = locator.GetService<Builder>();

            builder.BuilderObjectManager.OnObjectChanged(name);
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
        }

        public void Open()
        {
            nui.SendMessage("open");
            SetNuiFocus(true, true);
        }
    }
}
