using System.Threading.Tasks;

using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace FYF.MapBuilder.Client
{
    internal sealed class Freecam
    {
        public bool Enabled { get; private set; }
        
        private FreecamInput input;
        private FreecamCamera camera;

        private Vector3 movementVector = Vector3.Zero;
        private Vector3 rotationVector = Vector3.Zero;

        public Freecam()
        {
            camera = new FreecamCamera();

            input = new FreecamInput();
            input.BindKey(FreecamKeys.Forward, 1000, OnFreecamForward);
            input.BindKey(FreecamKeys.Backwards, 1000, OnFreecamBackwards);
            input.BindKey(FreecamKeys.Left, 1000, OnFreecamLeft);
            input.BindKey(FreecamKeys.Right, 1000, OnFreecamRight);
            input.BindKey(FreecamKeys.Up, 1000, OnFreecamDown);
            input.BindKey(FreecamKeys.Down, 1000, OnFreecamUp);
            input.BindMouse(OnFreecamMouseMove);
        }

        public void EnableFreecam()
        {
            camera.Create();
            FreezePlayerPed();
            Enabled = true;
        }

        public void DisableFreecam()
        {
            camera.Destroy();
            UnfreezePlayerPed();   
            Enabled = false;
        }

        public async Task Update()
        {
            //Update the minimap.
            if (Enabled)
            {
                SetFocusArea(camera.Position.X, camera.Position.Y, camera.Position.Z, 0.0f, 0.0f, 0.0f);
                LockMinimapPosition(camera.Position.X, camera.Position.Y);
                LockMinimapAngle(-((int)camera.Rotation.Z));
                SetRadarZoomLevelThisFrame(100.0f);
            }
            else
            {
                //@TODO: This shouldn't be called every frame,
                UnlockMinimapPosition();
                UnlockMinimapAngle();
                ClearFocus();

                await BaseScript.Delay(500);
                return;
            }

            input.PollKeys();
            input.PollMouse();

            camera.Update();

            await Task.FromResult(0);
        }

        void OnFreecamForward(float reach)
        {
            Vector3 forward = reach * camera.Matrix.Up;
            camera.SetRelativePosition(forward);
        }

        void OnFreecamBackwards(float reach)
        {
            Vector3 backwards = reach * camera.Matrix.Down;
            camera.SetRelativePosition(backwards);
        }

        void OnFreecamRight(float reach)
        {
            Vector3 right = reach * camera.Matrix.Right;
            camera.SetRelativePosition(right);
        }

        void OnFreecamLeft(float reach)
        {
            Vector3 left = reach * camera.Matrix.Left;
            camera.SetRelativePosition(left);
        }

        void OnFreecamUp(float reach)
        {
            Vector3 up = reach * camera.Matrix.Forward;
            camera.SetRelativePosition(up);
        }

        void OnFreecamDown(float reach)
        {
            Vector3 down = reach * camera.Matrix.Backward;
            camera.SetRelativePosition(down);
        }

        private void OnFreecamMouseMove(Vector2 rotation)
        {
            camera.SetRelativeRotation(rotation);
        }

        //@TODO: This should be some util function, same for unfreeze player.
        private void FreezePlayerPed()
        {
            int playerId = PlayerId();
            int playerPedId = PlayerPedId();

            Ped playerPed = new Ped(playerPedId);
            playerPed.Position = playerPed.Position + Vector3.Up;

            //Freeze the player ped entity.
            SetEntityVisible(playerPedId, false, false);
            SetEntityCollision(playerPedId, false, false);
            FreezeEntityPosition(playerPedId, true);
            SetPlayerInvincible(playerPedId, true);

            //NOTE: Not sure if this works or is required at all.
            NetworkSetEntityInvisibleToNetwork(playerPedId, true);
        }

        private void UnfreezePlayerPed()
        {
            int playerId = PlayerId();
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