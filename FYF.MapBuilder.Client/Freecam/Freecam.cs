using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Threading.Tasks;
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
        public FreecamConfig Config { get; private set; }
        
        private FreecamCamera camera;

        public Freecam(FreecamConfig config)
        {
            var accessor = MapBuilderClient.Accessor;
            var locator = MapBuilderClient.Locator;

            Config = config;

            //@TODO(bma) freecam-stutter: Don't use callbacks, this will lock camera to "Delay(0)" instead of Task.FromResult(0).Which causes notable lag.
            //                            Alternatively, we can allow for callbacks, but accumulate the changes in rot/pos in FreecamCamera.
            var input = locator.GetServiceReference<Input>().Get();
            input.RegisterKey(0, 32, InputKeyType.Continuous, OnFreecamForward);
            input.RegisterKey(0, 33, InputKeyType.Continuous, OnFreecamBackwards);
            input.RegisterKey(0, 34, InputKeyType.Continuous, OnFreecamLeft);
            input.RegisterKey(0, 35, InputKeyType.Continuous, OnFreecamRight);
            input.RegisterKey(0, 52, InputKeyType.Continuous, OnFreecamDown);
            input.RegisterKey(0, 54, InputKeyType.Continuous, OnFreecamUp);
            input.RegisterMouse(OnFreecamMouseMove);

            camera = new FreecamCamera(this);

            accessor.RegisterTick(Update);
        }

        public void EnableFreecam()
        {
            camera.Create();
            PlayerHelper.HidePlayer();
            Focus.Set(camera.Position, camera.Rotation);
        }

        public void DisableFreecam()
        {
            camera.Destroy();
            PlayerHelper.ShowPlayer();
            Focus.Clear();
        }

        public async Task Update()
        {
            ProfilerEnterScope("Freecam_Update");

            //Check if the camera is valid.
            if (camera.IsValid)
            {
                camera.Update();
                Focus.Set(camera.Position, camera.Rotation);
            }

            ProfilerExitScope();

            await Task.FromResult(0);
        }

        public Camera GetNativeCamera()
        {
            return camera.GetNativeCamera();
        }

        void OnFreecamForward(int time)
        {
            Vector3 forward = GetSmoothedKeyInput(time) * camera.Matrix.Up;
            camera.SetRelativePosition(forward);
        }

        void OnFreecamBackwards(int time)
        {
            Vector3 backwards = GetSmoothedKeyInput(time) * camera.Matrix.Down;
            camera.SetRelativePosition(backwards);
        }

        void OnFreecamRight(int time)
        {
            Vector3 right = GetSmoothedKeyInput(time) * camera.Matrix.Right;
            camera.SetRelativePosition(right);
        }

        void OnFreecamLeft(int time)
        {
            Vector3 left = GetSmoothedKeyInput(time) * camera.Matrix.Left;
            camera.SetRelativePosition(left);
        }

        void OnFreecamUp(int time)
        {
            Vector3 up = GetSmoothedKeyInput(time) * camera.Matrix.Forward;
            camera.SetRelativePosition(up);
        }

        void OnFreecamDown(int time)
        {
            Vector3 down = GetSmoothedKeyInput(time) * camera.Matrix.Backward;
            camera.SetRelativePosition(down);
        }

        float GetSmoothedKeyInput(int input)
        {
            return MathUtil.Clamp((float)input / (float)Config.KeySmoothTime, 0.01f, 1.0f);
        }

        private void OnFreecamMouseMove(Vector2 rotation)
        {
            camera.SetRelativeRotation(rotation);
        }
    }
}