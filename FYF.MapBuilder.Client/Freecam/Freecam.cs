using CitizenFX.Core;
using System.Threading.Tasks;

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

        private ServiceReference<Input> inputRef;
        private FreecamCamera camera;

        public Freecam(FreecamConfig config)
        {
            var accessor = MapBuilderClient.Accessor;
            var locator = MapBuilderClient.Locator;

            Config = config;

            inputRef = locator.GetServiceReference<Input>();
            var input = inputRef.Get();

            input.RegisterKey(0, 32, InputKeyType.Continuous);
            input.RegisterKey(0, 33, InputKeyType.Continuous);
            input.RegisterKey(0, 34, InputKeyType.Continuous);
            input.RegisterKey(0, 35, InputKeyType.Continuous);
            input.RegisterKey(0, 52, InputKeyType.Continuous);
            input.RegisterKey(0, 54, InputKeyType.Continuous);

            camera = new FreecamCamera(this);

            accessor.OnRenderTick(Freecam_Update);
            accessor.OnRenderTick(Freecam_UpdateCamera);
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

        async Task Freecam_Update()
        {
            //Check if the camera is valid.
            if (!camera.IsValid)
            {
                await Task.FromResult(0);
                return;
            }

            int time = -1;
            Input input = inputRef.Get();

            if (input.PollKey(0, 32, out time))
            {
                OnFreecamForward(time);
            }

            if (input.PollKey(0, 33, out time))
            {
                OnFreecamBackwards(time);
            }

            if (input.PollKey(0, 34, out time))
            {
                OnFreecamLeft(time);
            }

            if (input.PollKey(0, 35, out time))
            {
                OnFreecamRight(time);
            }

            if (input.PollKey(0, 52, out time))
            {
                OnFreecamDown(time);
            }

            if (input.PollKey(0, 54, out time))
            {
                OnFreecamUp(time);
            }

            OnFreecamMouseMove(input.PollMouse());
        }

        async Task Freecam_UpdateCamera()
        {
            //Check if the camera is valid.
            if (!camera.IsValid)
            {
                await Task.FromResult(0);
                return;
            }

            camera.Update();
            Focus.Set(camera.Position, camera.Rotation);
        }

        public Camera GetNativeCamera()
        {
            return camera.GetNativeCamera();
        }

        private void OnFreecamForward(int time)
        {
            Vector3 forward = GetSmoothedKeyInput(time) * camera.Matrix.Up;
            camera.SetRelativePosition(forward);
        }

        private void OnFreecamBackwards(int time)
        {
            Vector3 backwards = GetSmoothedKeyInput(time) * camera.Matrix.Down;
            camera.SetRelativePosition(backwards);
        }

        private void OnFreecamRight(int time)
        {
            Vector3 right = GetSmoothedKeyInput(time) * camera.Matrix.Right;
            camera.SetRelativePosition(right);
        }

        private void OnFreecamLeft(int time)
        {
            Vector3 left = GetSmoothedKeyInput(time) * camera.Matrix.Left;
            camera.SetRelativePosition(left);
        }

        private void OnFreecamUp(int time)
        {
            Vector3 up = GetSmoothedKeyInput(time) * Vector3.ForwardLH;
            camera.SetRelativePosition(up);
        }

        private void OnFreecamDown(int time)
        {
            Vector3 down = GetSmoothedKeyInput(time) * Vector3.ForwardRH;
            camera.SetRelativePosition(down);
        }

        private float GetSmoothedKeyInput(int input)
        {
            return MathUtil.Clamp((float)input / (float)Config.KeySmoothTime, 0.01f, 1.0f);
        }

        private void OnFreecamMouseMove(Vector2 rotation)
        {
            camera.SetRelativeRotation(rotation);
        }
    }
}