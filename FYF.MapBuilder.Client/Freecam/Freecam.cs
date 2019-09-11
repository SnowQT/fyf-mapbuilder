using System.Threading.Tasks;

using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace FYF.MapBuilder.Client
{
    internal struct FreecamConfig
    {
        public int FieldOfView;

        public float RotationSensitivity;
        public float RotationBase;
        public float PositionSensitivity;
        public float PositionBase;

        public int KeySmoothTime;
    }

    internal sealed class Freecam
    {
        public bool Enabled { get; private set; }
        public FreecamConfig Config { get; private set; }
        
        private FreecamInput input;
        private FreecamCamera camera;

        private Vector3 movementVector = Vector3.Zero;
        private Vector3 rotationVector = Vector3.Zero;

        public Freecam(FreecamConfig config)
        {
            Config = config;

            camera = new FreecamCamera(this);

            input = new FreecamInput(this);
            input.BindKey(FreecamKeys.Forward, OnFreecamForward);
            input.BindKey(FreecamKeys.Backwards, OnFreecamBackwards);
            input.BindKey(FreecamKeys.Left, OnFreecamLeft);
            input.BindKey(FreecamKeys.Right, OnFreecamRight);
            input.BindKey(FreecamKeys.Up, OnFreecamDown);
            input.BindKey(FreecamKeys.Down, OnFreecamUp);
            input.BindMouse(OnFreecamMouseMove);
        }

        public void EnableFreecam()
        {
            camera.Create();
            FreezePlayerPed();
            Focus.Set(camera.Position, camera.Rotation);

            Enabled = true;
        }

        public void DisableFreecam()
        {
            camera.Destroy();
            UnfreezePlayerPed();
            Focus.Clear();

            Enabled = false;
        }

        public async Task Update()
        {
            if (!Enabled)
            {
                return;
            }

            input.PollKeys();
            input.PollMouse();

            camera.Update();
            Focus.Set(camera.Position, camera.Rotation);

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