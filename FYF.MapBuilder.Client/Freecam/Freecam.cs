using System.Threading.Tasks;

using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace FYF.MapBuilder.Client
{
    internal sealed class Freecam
    {
        public bool Enabled { get; private set; }
        
        public bool IsCameraValid
        {
            get
            {
                return Enabled && 
                    camera != null && 
                    camera.Exists();
            }

        }

        private Camera camera = null;

        public void Enable()
        {
            CreateCamera();
            DisablePlayer();

            Enabled = true;
        }

        public void Disable()
        {
            DestroyCamera();
            EnablePlayer();   

            Enabled = false;
        }

        public async Task Update()
        {
            if (!IsCameraValid)
            {
                await BaseScript.Delay(500);
                return;
            }

            Vector3 position = camera.Position;
            SetFocusArea(position.X, position.Y, position.Z, 0.0f, 0.0f, 0.0f);

            await Task.FromResult(0);
        }

        private void DisablePlayer()
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

            //NOTE: This will cause _all_ input to be killed.
            //      So that is why we are not using this.
            //SetPlayerControl(playerId, false, 0);
        }

        private void EnablePlayer()
        {
            int playerId = PlayerId();
            int playerPedId = PlayerPedId();

            //Reset area focus to the player.
            ClearFocus();

            //Unfreeze the player ped entity.
            SetEntityVisible(playerPedId, true, false);
            SetEntityCollision(playerPedId, true, false);
            SetPlayerInvincible(playerPedId, false);
            FreezeEntityPosition(playerPedId, false);

            //NOTE: This will cause _all_ input to be killed.
            //      So that is why we are not using this.
            //SetPlayerControl(playerId, true, 0);
        }

        //@TODO: This should allow for a config to be passed in.
        private void CreateCamera()
        {
            const float FOV = 90.0f;

            int cameraHandle = CreateCam("DEFAULT_SCRIPTED_CAMERA", true);

            camera = new Camera(cameraHandle);

            if (!camera.Exists())
            {
                Debug.WriteLine("FYF-MAPBUILDER - Failed to instantiate a new camera.");
                return;
            }

            MoveCamera(new Vector3(0.0f, 100.0f, 0.0f));
            RotateCamera(Vector3.Zero);

            camera.FieldOfView = FOV;
            camera.IsActive = true;

            //Normally we would use the wrapper, but this doesn't expose easing and easeTime.
            //World.RenderingCamera = camera;
            RenderScriptCams(true, true, 500, false, false);
        }

        private void DestroyCamera()
        {
            if (IsCameraValid)
            {
                camera.Delete();
            }

            RenderScriptCams(false, false, 0, false, false);
        }

        private void MoveCamera(Vector3 position)
        {
            SetFocusArea(position.X, position.Y, position.Z, 0.0f, 0.0f, 0.0f);
            camera.Position = position;
        }

        private void RotateCamera(Vector3 rotation)
        {
            camera.Rotation = rotation;
        }
    }
}
