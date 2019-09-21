using System;
using System.Collections.Generic;
using System.Dynamic;

using Newtonsoft.Json;

using static CitizenFX.Core.Native.API;

namespace FYF.MapBuilder.Client
{
    internal delegate void NuiToggleFunctions();
    internal delegate void NuiCallback(dynamic args);

    internal class NuiToggleInfo
    {
        public bool State;
        public int KeyCode;
        public string KeyName;
        public NuiToggleFunctions OpenFunction;
        public NuiToggleFunctions CloseFunction;
    }

    internal class NuiHelper
    {
        private IAccessor accessor;
        private readonly Input input;


        private HashSet<NuiToggleInfo> toggles = new HashSet<NuiToggleInfo>();

        public NuiHelper()
        {
            accessor = MapBuilderClient.Accessor;
            input = accessor.GetLocator().GetService<Input>();
        }

        public void AddToggle(int keyCode, string keyName, NuiToggleFunctions openFunc, NuiToggleFunctions closeFunc)
        {
            NuiToggleInfo info = new NuiToggleInfo
            {
                KeyCode = keyCode,
                KeyName = keyName,
                OpenFunction = openFunc,
                CloseFunction = closeFunc
            };

            input.RegisterKey(0, keyCode, InputKeyType.Once, (time) => { HandleToggle(info); });

            toggles.Add(info);
        }

        void HandleToggle(NuiToggleInfo toggle)
        {
            //If the current state is closed, we want to open it and request a toggle to the NUI backend.
            if (!toggle.State)
            {
                //Open the menu.
                toggle.OpenFunction();
                toggle.State = true;

                //Setup a callback for NUI to call us back when the player want to close the UI.
                string uniqueName = Guid.NewGuid().ToString("n");
                var values = new Dictionary<string, string>
                    {
                        { "toggleName", uniqueName },
                        { "toggleKey", toggle.KeyName }
                    };

                SendMessage("toggleInit", values);

                //Listen for when the user wants to close the UI.
                AddCallback($"toggleInvoke_{uniqueName}", (dict) =>
                {
                    toggle.CloseFunction();
                    toggle.State = false;
                });
            }
        }

        public void AddCallback(string eventName, NuiCallback callback)
        {
            RegisterNuiCallbackType(eventName);

            //Setup a listener for this event, process JSON when we receive this.
            accessor.RegisterEvent($"__cfx_nui:{eventName}", 
                new Action<ExpandoObject>((data) => callback?.Invoke(data))
            );
        }

        public void SendMessage(string type, IDictionary<string, string> values = null)
        {
            IDictionary<string, string> message = new Dictionary<string, string>
            {
                { "messageType", type }
            };

            if (values != null && values.Count > 0)
            {
                foreach (var val in values)
                {
                    message.Add(val.Key, val.Value);
                }
            }

            string json = JsonConvert.SerializeObject(message);
            SendNuiMessage(json);
        }
    }
}
