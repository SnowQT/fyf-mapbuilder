using System;
using System.Linq;
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

        //@TODO(bma): Some of the props have different anchor points, can we back this into the metadata some how?
        private async Task BuildObjectManager_UpdateProp()
        {
            if (!HasPropSelected)
            {
                await Task.FromResult(0);
                return;
            }

            Profiler.Enter("BuildObjectManager_UpdateProp");

            Freecam freecam = camera.Get();
            Camera freecamNative = freecam.GetNativeCamera();

            RaycastResult result = freecam.GetLookat(currentProp);

            if (result.DitHit)
            {
                PlaceCurrentPropOnGround(freecamNative, result.HitPosition);
            }
            else
            {
                PlaceCurrentPropInfrontOfCamera(freecamNative);
            }

            
            Profiler.Exit();
        }

        private void PlaceCurrentPropOnGround(Camera camera, Vector3 hitPosition)
        {
            currentProp.Position = hitPosition;
        }

        private void PlaceCurrentPropInfrontOfCamera(Camera camera)
        {
            Vector3 min = Vector3.Zero;
            Vector3 max = Vector3.Zero;
            GetModelDimensions((uint)modelToLoad.Hash, ref min, ref max);

            BoundingVolume boundingVol = new BoundingVolume(min, max);

            const float minDist = 2.5f;
            float nearClip = camera.NearClip;
            float farClip = camera.FarClip;
            float nearFarDistance = farClip - nearClip;

            float objectSize = boundingVol.GetRadius();
            float objectSizeFraction = (objectSize - nearClip) / (farClip - nearClip);
            objectSizeFraction = MathUtil.Clamp(objectSizeFraction, 0.0f, 1.0f);

            float propDistance = nearClip + (nearFarDistance * objectSizeFraction);
            propDistance = MathUtil.Clamp(propDistance, minDist, farClip);

            Vector3 propPosition = camera.Position + (propDistance * camera.Matrix.Up);
            currentProp.Position = propPosition;
        }

        private async Task SetNewCurrentProp()
        {
            Profiler.Enter("BuilderObjectManager_Update_SetNewCurrentProp");

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

            Profiler.Exit();
        }

        public void OnObjectChanged(string objectName)
        {
            modelToLoad = new Model(objectName);
            isPropLoaded = false;
        }

        public void Cleanup()
        {
            currentProp = null;
            isPropLoaded = false;
            modelToLoad = new Model();
        }
    }
}
