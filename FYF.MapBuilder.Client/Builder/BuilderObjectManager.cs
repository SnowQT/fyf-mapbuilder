using System;
using System.Threading.Tasks;

using CitizenFX.Core;

namespace FYF.MapBuilder.Client
{
    internal class BuilderObjectManager
    {
        private readonly ServiceReference<Freecam> camera;

        private Prop currentProp = null;
        private bool isPropLoaded = true;
        private Model modelToLoad = null;
        

        public BuilderObjectManager()
        {
            var accessor = MapBuilderClient.Accessor;
            var locator = MapBuilderClient.Locator;

            camera = locator.GetServiceReference<Freecam>();

            accessor.RegisterTick(OnTick);
        }

        public async Task OnTick()
        {
            if (!isPropLoaded)
            {
                await SetNewCurrentProp();
            }

            await BaseScript.Delay(100);
        }

        private async Task SetNewCurrentProp()
        {
            //Unload and destroy the current prop and model.
            if (currentProp != null && currentProp.Exists())
            {
                Model propModel = currentProp.Model;
                propModel.MarkAsNoLongerNeeded();

                currentProp.Delete();
                currentProp = null;
            }

            //Check if we need to load the new model.
            if (!modelToLoad.IsLoaded)
            {
                await modelToLoad.Request(500);
            }

            Camera freecamNative = camera.Get()?.GetNativeCamera();
            Vector3 camPos = freecamNative.Position;
            Vector3 camForwardDir = freecamNative.Matrix.Up;

            const float sizeMax = 200.0f;
            const float dropoffCoeff = 200.0f;

            //Compute inverse exponential curve...
            float size = modelToLoad.GetDimensions().LengthSquared();
            float sizeFrac = MathUtil.Clamp(size / sizeMax, 0.05f, 1.0f);
            float dropoffToX = (float)Math.Pow(dropoffCoeff, -sizeFrac);
            float distance = 1.0f - (dropoffToX - (1.0f / dropoffCoeff));

            Vector3 propPos = camPos + (camForwardDir * 33.3f * distance);
            Vector3 propRot = new Vector3(0.0f, 0.0f, freecamNative.Rotation.Z);

            Prop prop = await World.CreateProp(modelToLoad, propPos, propRot, false, false);

            currentProp = prop;
            isPropLoaded = true;
        }

        public void OnObjectChanged(string objectName)
        {
            modelToLoad = new Model(objectName);
            isPropLoaded = false;
        }
    }
}
