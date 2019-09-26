
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace FYF.MapBuilder.Client
{
    internal sealed class FreecamCamera
    {
        public bool IsValid
        {
            get
            {
                return cameraReference != null && cameraReference.Exists();
            }
        }

        public Vector3 Position
        {
            get
            {
                if (!IsValid)
                {
                    return Vector3.Zero;
                }

                return cameraReference.Position;
            }
        }

        public Vector3 Rotation
        {
            get
            {
                if (!IsValid)
                {
                    return Vector3.Zero;
                }

                return cameraReference.Rotation;
            }
        }

        public Matrix Matrix
        {
            get
            {
                if (!IsValid)
                {
                    return Matrix.Identity;
                }

                return cameraReference.Matrix;
            }
        }

        private Freecam self;
        private Camera cameraReference;
        private Vector3 positionDeltaVector = Vector3.Zero;
        private Vector3 rotationDeltaVector = Vector3.Zero;

        public FreecamCamera(Freecam self)
        {
            this.self = self;
        }

        public void Create()
        {
            int cameraHandle = CreateCam("DEFAULT_SCRIPTED_CAMERA", true);
            cameraReference = new Camera(cameraHandle);

            if (!cameraReference.Exists())
            {
                Debug.WriteLine("MapBuilder - Failed to instantiate a new camera.");
                return;
            }

            cameraReference.Position = new Vector3(0.0f, 0.0f, 200.0f);
            cameraReference.Rotation = new Vector3(0.0f, 0.0f, 0.0f);
            cameraReference.NearClip = 0.0001f;
            cameraReference.FieldOfView = self.Config.FieldOfView;
            cameraReference.IsActive = true;

            RenderScriptCams(true, true, 1000, false, false);
        }

        internal Camera GetNativeCamera()
        {
            return cameraReference;
        }

        public void Destroy()
        {
            if (IsValid)
            {
                cameraReference.Delete();
                cameraReference = null;
            }

            RenderScriptCams(false, true, 1000, false, false);
        }

        //@TODO(bma): #freecam-stutter: Use linear interpolation to fill in frames between receiving input and not.
        public void Update()
        {
            //Apply the position of the camera.
            Vector3 newCameraPosition = Position + positionDeltaVector;

            //Apply the rotation of the camera.
            Vector3 cameraRotation = Rotation + rotationDeltaVector;

            float clampedX = MathUtil.Clamp(cameraRotation.X, -89.0f, 89.0f); //Avoid over-rotating.
            Vector3 newCameraRotation = new Vector3(clampedX, 0.0f, cameraRotation.Z);

            //Apply position and rotation
            cameraReference.Position = newCameraPosition;
            cameraReference.Rotation = newCameraRotation;

            positionDeltaVector = Vector3.Zero;
            rotationDeltaVector = Vector3.Zero;
        }

        public void SetRelativePosition(Vector3 input)
        {
            positionDeltaVector += ScaleWithTime(input,
                self.Config.PositionSensitivity,
                self.Config.PositionBase
            );
        }

        public void SetRelativeRotation(Vector2 input)
        {
            Vector3 scaledRotation = new Vector3(input.X, 0.0f, input.Y);

            rotationDeltaVector += ScaleWithTime(scaledRotation, 
                self.Config.RotationSensitivity,
                self.Config.RotationBase
            );
        }

        private Vector3 ScaleWithTime(Vector3 input, float sensitivity, float baseValue)
        {
            return input * GetFrameTime() * (sensitivity * baseValue);
        }
    }
}
