using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace FYF.MapBuilder.Client
{
    internal class UserInterface
    {
        private readonly NuiHelper nui;
        private readonly Input input;
        private readonly Builder builder;

        public UserInterface()
        {
            var accessor = MapBuilderClient.Accessor;
            var locator = accessor.GetLocator();

            builder = locator.GetService<Builder>();

            nui = new NuiHelper();
            nui.AddToggle(37, "9", Open, Close);
            nui.AddCallback("Browser_OnObjectChanged", Browser_OnObjectChanged);

            accessor.RegisterTick(UpdateUI);
        }

        //@TODO: Can't we just bind this behavior directly to the Builder or BuilderObjectManager?
        void Browser_OnObjectChanged(dynamic args)
        {
            string name = (string)args.name;
            builder.BuilderObjectManager.OnObjectChanged(name);
        }

        internal async Task UpdateUI()
        {
            //@HACK #state-manager: Don't rely on a static, move to a state manager
            bool isInBuildMode = MapBuilderClient.IsUserInBuildMode;
            if (isInBuildMode)
            {
                nui.UpdateToggles();
            }

            await BaseScript.Delay(1);
        }

        public void Close()
        {
            nui.SendMessage("close");
            SetNuiFocus(false, false);
        }

        public void Open()
        {
            nui.SendMessage("open");
            SetNuiFocus(true, true);
        }
    }
}
