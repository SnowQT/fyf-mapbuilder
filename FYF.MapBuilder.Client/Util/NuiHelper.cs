using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CitizenFX.Core.Native.API;

namespace FYF.MapBuilder.Client
{
    internal delegate void NuiToggleFunctions();

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
                    if (!IsControlJustPressed(0, info.KeyCode) ||
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
                    SetNuiCallback($"toggleInvoke_{uniqueName}", new Action(() =>
                    {
                        info.CloseFunction();
                        info.State = false;
                    }));
                }
            }
        }

        public void SetNuiCallback(string eventName, Delegate callback)
        {
            RegisterNuiCallbackType(eventName);
            accessor.RegisterEvent($"__cfx_nui:{eventName}", callback);
        }

        public void SendMessage(string type, IDictionary<string, string> values = null)
        {
            //@TODO: Super hacky, this really should use something like a JSON serializer.... like really... 
            StringBuilder builder = new StringBuilder();
            builder.Append("{ \"messageType\": \"");
            builder.Append(type);

            if (values != null && values.Count > 0)
            {
                builder.Append("\", ");

                foreach (var kv in values)
                {
                    builder.Append($"\"{kv.Key}\": \"{kv.Value}\"");

                    //Does not equal the last element in the collection.
                    if (!kv.Equals(values.Last()))
                    {
                        builder.Append(",");
                    }
                }
            }
            else
            {
                builder.Append("\"");
            }

            builder.Append("}");


            Debug.WriteLine(builder.ToString());
            SendNuiMessage(builder.ToString());
        }
    }
}
