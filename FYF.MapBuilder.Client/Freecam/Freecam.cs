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

        private Camera camera;
        private FreecamInput input;

        private Vector3 movementVector = Vector3.Zero;
        private Vector3 rotationVector = Vector3.Zero;

        public Freecam()
        {
            input = new FreecamInput();

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
            CreateCamera();
            FreezePlayerPed();

            Enabled = true;
        }

        public void DisableFreecam()
        {
            DestroyCamera();
            UnfreezePlayerPed();   

            Enabled = false;
        }

        public async Task Update()
        {
            if (!IsCameraValid)
            {
                await BaseScript.Delay(500);
                return;
            }

            UpdateMinimap();

            input.PollKeys();
            input.PollMouse();

            MoveCameraRelative(movementVector);
            RotateCameraRelative(rotationVector);

            movementVector = Vector3.Zero;
            rotationVector = Vector3.Zero;

            await Task.FromResult(0);
        }

        private void UpdateMinimap()
        {
            if (Enabled)
            {
                LockMinimapPosition(camera.Position.X, camera.Position.Y);
                LockMinimapAngle(-((int)camera.Rotation.Z));
                SetRadarZoomLevelThisFrame(100.0f);
            }
            else
            {
                UnlockMinimapPosition();
                UnlockMinimapAngle();
            }
        }

        private void OnFreecamForward(float heldTime)
        {
            movementVector += camera.UpVector;
        }

        private void OnFreecamBackwards(float heldTime)
        {
            movementVector -= camera.UpVector;
        }

        private void OnFreecamRight(float heldTime)
        {
            movementVector += camera.RightVector;
        }

        private void OnFreecamLeft(float heldTime)
        {
            movementVector -= camera.RightVector;
        }

        private void OnFreecamUp(float heldTime)
        {
            movementVector += camera.ForwardVector;
        }

        private void OnFreecamDown(float heldTime)
        {
            movementVector -= camera.ForwardVector;
        }

        private void OnFreecamMouseMove(Vector2 rotation)
        {
            //@TODO: Remove this.
            const float sensitivity = 2.0f;

            Vector3 newRotation = new Vector3(rotation.X, 0.0f, rotation.Y);
            newRotation = newRotation * GetFrameTime() * (sensitivity * 500.0f);

            rotationVector += newRotation;
        }

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

            //Reset area focus to the player.
            ClearFocus();

            //Unfreeze the player ped entity.
            SetEntityVisible(playerPedId, true, false);
            SetEntityCollision(playerPedId, true, false);
            SetPlayerInvincible(playerPedId, false);
            FreezeEntityPosition(playerPedId, false);

            NetworkSetEntityInvisibleToNetwork(playerPedId, false);
        }

        //@TODO: This should allow for a config to be passed in.
        private void CreateCamera()
        {
            const float FOV = 90.0f;

            int cameraHandle = CreateCam("DEFAULT_SCRIPTED_CAMERA", true);

            camera = new Camera(cameraHandle);

            if (!camera.Exists())
            {
                Debug.WriteLine("MapBuilder - Failed to instantiate a new camera.");
                return;
            }

            MoveCamera(new Vector3(0.0f, 0.0f, 500.0f));
            RotateCamera(new Vector3(0.0f, 0.0f, 0.0f));

            camera.FieldOfView = FOV;
            camera.IsActive = true;

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

        //Move the camera relative to the current position.
        private void MoveCameraRelative(Vector3 offset)
        {
            Vector3 cameraPosition = camera.Position;
            cameraPosition += offset;
            MoveCamera(cameraPosition);
        }

        private void RotateCamera(Vector3 rotation)
        {
            camera.Rotation = rotation;
        }

        private void RotateCameraRelative(Vector3 rotation)
        {
            Vector3 cameraRotation = camera.Rotation;
            cameraRotation += rotation;

            float clampedX = MathUtil.Clamp(cameraRotation.X, -89.0f, 89.0f);
            Vector3 clampedRotation = new Vector3(clampedX, 0.0f, cameraRotation.Z);
            RotateCamera(clampedRotation);
        }
    }
}
