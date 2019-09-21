using static CitizenFX.Core.Native.API;

namespace FYF.MapBuilder.Client
{
    internal class UserInterface
    {
        private readonly NuiHelper nui;
        private readonly Input input;
        private readonly Builder builder;

        //@TODO premature-ui-creation: This ideally should be created when user enters builder mode.
        //                             Doing this right now will register the toggle and the user will be able
        //                             to open the builder UI.
        public UserInterface()
        {
            var accessor = MapBuilderClient.Accessor;
            var locator = accessor.GetLocator();

            builder = locator.GetService<Builder>();

            nui = new NuiHelper();
            nui.AddToggle(37, "9", Open, Close);
            nui.AddCallback("Browser_OnObjectChanged", Browser_OnObjectChanged);
        }

        //@TODO: Can't we just bind this behavior directly to the Builder or BuilderObjectManager?
        void Browser_OnObjectChanged(dynamic args)
        {
            string name = (string)args.name;
            builder.BuilderObjectManager.OnObjectChanged(name);
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
