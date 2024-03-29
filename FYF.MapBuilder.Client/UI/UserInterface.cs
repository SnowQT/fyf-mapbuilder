﻿using static CitizenFX.Core.Native.API;

namespace FYF.MapBuilder.Client
{
    internal class UserInterface
    {
        private readonly NuiHelper nui;
        private readonly ServiceReference<Builder> builderRef;

        //@TODO(bma) #state-manager: This ideally should be created when user enters builder mode.
        //                             Doing this right now will register the toggle and the user will be able
        //                             to open the builder UI.
        public UserInterface()
        {
            var accessor = MapBuilderClient.Accessor;
            var locator = MapBuilderClient.Locator;

            builderRef = locator.GetServiceReference<Builder>();

            nui = new NuiHelper();
            nui.AddToggle(37, "9", Open, Close);
            nui.AddCallback("Browser_OnObjectChanged", Browser_OnObjectChanged);
        }

        //@TODO(bma) #broadcast: These callbacks should not be in the User Interface, would be nice if we could broadcast a message across the entire domain.
        void Browser_OnObjectChanged(dynamic args)
        {
            Builder builder = builderRef.Get();
            builder.BuilderObjectManager.OnObjectChanged(args.name);
        }

        public void Close()
        {
            SetNuiFocus(false, false);
        }

        public void Open()
        {
            SetNuiFocus(true, true);
        }
    }
}
