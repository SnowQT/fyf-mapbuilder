using System;
using System.Threading.Tasks;

using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace FYF.MapBuilder.Client
{
    internal class BuilderObjectManager
    {
        private readonly ServiceReference<Freecam> camera;

        private Prop currentProp = null;
        private bool isPropLoaded = false;
        private Model modelToLoad = new Model();

        public bool HasPropSelected
        {
            get
            {
                return isPropLoaded && currentProp != null;
            }
        }

        public bool NeedsToLoadProp
        {
            get
            {
                return modelToLoad.Hash != 0 && !isPropLoaded;
            }
        }
        

        public BuilderObjectManager()
        {
            var accessor = MapBuilderClient.Accessor;
            var locator = MapBuilderClient.Locator;

            camera = locator.GetServiceReference<Freecam>();

            accessor.OnUpdateTick(BuildObjectManager_UpdateTick);
            accessor.OnRenderTick(BuildObjectManager_UpdateProp);
        }

        public async Task BuildObjectManager_UpdateTick()
        {
            if (NeedsToLoadProp)
            {
                await SetNewCurrentProp();
                return;
            }
        }

        private async Task BuildObjectManager_UpdateProp()
        {
            //@TODO(bma): This is still jank, does not line up the object into the camera frustum, especially on larger objects.
            if (!HasPropSelected)
            {
                await Task.FromResult(0);
                return;
            }

            ProfilerEnterScope("BuildObjectManager_UpdateProp");

            Camera freecam = camera.Get()?.GetNativeCamera();
            Vector3 dimension = modelToLoad.GetDimensions();
            Vector3 camForwardDir = freecam.Matrix.Up;

            float objectMaxSize = Math.Max(Math.Max(dimension.X, dimension.Y), dimension.Z) / 2.0f; //No overload for 3 variables...? really?
            float cameraView = 2.0f * (float)Math.Tan(0.5f * (Math.PI * freecam.FieldOfView / 180.0f));
            float camerDistanceNoOffset = cameraView * objectMaxSize * 2.0f;
            float cameraDistance = camerDistanceNoOffset + 0.5f * camerDistanceNoOffset;

            Vector3 propPos = freecam.Position + (cameraDistance * camForwardDir);
            currentProp.Position = propPos;

            Vector3 min = Vector3.Zero;
            Vector3 max = Vector3.Zero;

            GetModelDimensions((uint)modelToLoad.Hash, ref min, ref max);

            min += propPos;
            max += propPos;

            DrawBox(min.X, min.Y, min.Z, max.X, max.Y, max.Z, 255, 0, 0, 100);

            ProfilerExitScope();
        }

        private async Task SetNewCurrentProp()
        {
            ProfilerEnterScope("BuilderObjectManager_Update_SetNewCurrentProp");

            //Unload and destroy the current prop and model.
            if (currentProp != null && currentProp.Exists())
            {
                Model propModel = currentProp.Model;
                propModel.MarkAsNoLongerNeeded();

                currentProp.Delete();
            }

            //Check if we need to load the new model.
            if (!modelToLoad.IsLoaded)
            {
                await modelToLoad.Request(500);
            }

            currentProp = await World.CreateProp(modelToLoad, Vector3.Zero, Vector3.Zero, false, false);
            isPropLoaded = true;

            ProfilerExitScope();
        }

        public void OnObjectChanged(string objectName)
        {
            modelToLoad = new Model(objectName);
            isPropLoaded = false;
        }
    }
}
