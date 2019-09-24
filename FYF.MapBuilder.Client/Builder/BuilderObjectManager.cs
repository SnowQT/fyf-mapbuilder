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

            accessor.RegisterTick(Update);
            accessor.RegisterTick(UpdateProp);
        }

        public async Task Update()
        {
            ProfilerEnterScope("BuilderObjectManager_Update");

            if (NeedsToLoadProp)
            {
                await SetNewCurrentProp();
            }

            ProfilerExitScope();

            await BaseScript.Delay(0);
        }

        private async Task UpdateProp()
        {
            if (!HasPropSelected)
            {
                await BaseScript.Delay(100);
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

            Debug.WriteLine(propPos.ToString());

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
