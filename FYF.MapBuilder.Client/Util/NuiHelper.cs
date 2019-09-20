using System;
using System.Collections.Generic;
using System.Dynamic;
using CitizenFX.Core;
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
        private HashSet<NuiToggleInfo> toggles = new HashSet<NuiToggleInfo>();

        public NuiHelper()
        {
            accessor = MapBuilderClient.Accessor;
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

            toggles.Add(info);
        }

        public void UpdateToggles()
        {
            foreach (NuiToggleInfo info in toggles)
            {
                //@TODO: Use an input layer to make this more consistent or something.
                //          And reducing overhead by not checking for disabled keys.
                //If the current toggle is closed, we should poll it's keys.
                if (!info.State)
                {
                    if (!IsControlJustPressed(0, info.KeyCode) &&
                        !IsDisabledControlJustPressed(0, info.KeyCode))
                    {
                        continue;
                    }
                    
                    //Open the toggle and set the state accordingly.
                    info.OpenFunction();
                    info.State = true;

                    //Setup and send the toggleInit information.
                    string uniqueName = Guid.NewGuid().ToString("n");
                    var values = new Dictionary<string, string>
                    {
                        { "toggleName", uniqueName },
                        { "toggleKey", info.KeyName }
                    };

                    SendMessage("toggleInit", values);

                    //Setup the NUI close callback using the unique ID.
                    AddCallback($"toggleInvoke_{uniqueName}", (dict) =>
                    {
                        info.CloseFunction();
                        info.State = false;
                    });
                }
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
