using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace FYF.MapBuilder.Client
{
    internal static class PlayerHelper
    {
        public static void HidePlayer()
        {
            int playerPedId = PlayerPedId();

            Ped playerPed = new Ped(playerPedId);
            playerPed.Position = playerPed.Position + Vector3.Up;

            SetEntityVisible(playerPedId, false, false);
            SetEntityCollision(playerPedId, false, false);
            FreezeEntityPosition(playerPedId, true);
            SetPlayerInvincible(playerPedId, true);

            //NOTE: Not sure if this works or is required at all.
            NetworkSetEntityInvisibleToNetwork(playerPedId, true);
        }

        public static void ShowPlayer()
        {
            int playerPedId = PlayerPedId();

            //Unfreeze the player ped entity.
            SetEntityVisible(playerPedId, true, false);
            SetEntityCollision(playerPedId, true, false);
            SetPlayerInvincible(playerPedId, false);
            FreezeEntityPosition(playerPedId, false);

            NetworkSetEntityInvisibleToNetwork(playerPedId, false);
        }
    }
}
