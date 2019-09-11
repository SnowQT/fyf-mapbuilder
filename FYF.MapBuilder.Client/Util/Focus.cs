using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace FYF.MapBuilder.Client
{
    internal static class Focus
    {
        private static bool _hasCustomFocus = false;

        public static void Set(Vector3 position, Vector3 rotation)
        {
            //NOTE: Convert to a signed angle (0-306 degrees).
            int signedAngle = 180 + (int)rotation.Z; 

            LockMinimapPosition(position.X, position.Y);
            LockMinimapAngle(signedAngle);

            SetRadarZoomLevelThisFrame(100.0f);
            SetFocusArea(position.X, position.Y, position.Z, 0.0f, 0.0f, 0.0f);

            _hasCustomFocus = true;
        }

        public static void Clear()
        {
            if (_hasCustomFocus)
            {
                UnlockMinimapPosition();
                UnlockMinimapAngle();
                ClearFocus();
            }

            _hasCustomFocus = false;
        }

    }
}
