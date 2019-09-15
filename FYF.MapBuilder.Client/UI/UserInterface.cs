using System;
using System.Threading.Tasks;

using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace FYF.MapBuilder.Client
{
    internal class UserInterface
    {
        const int WeaponWheelInputGroup = 24;
        const int OpenUIKey = 37;

        int[] DisabledActionKeys = new int[] {
            OpenUIKey,
            261,
            262
        };

        internal async Task Update()
        {
            foreach (int key in DisabledActionKeys)
            {
                DisableControlAction(0, key, true);
            }

            DisableInputGroup(WeaponWheelInputGroup);

            if (IsDisabledControlJustPressed(0, OpenUIKey))
            {
                SendNuiMessage("{\"messageType\": \"openCloseState\"}");
                SetNuiFocus(true, true);
            }

            await Task.FromResult(0);
        }
    }
}
