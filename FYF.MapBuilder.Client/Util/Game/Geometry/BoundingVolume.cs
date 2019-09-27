using System.Collections.Generic;

using CitizenFX.Core;

namespace FYF.MapBuilder.Client
{
    public class BoundingVolume
    {
        public Vector3 Min;
        public Vector3 Max;
        public Vector3 Center;
        public IList<Vector3> Corners;

        public BoundingVolume(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
            Center = (Min + Max) * 0.5f;
            Corners = GetCorners();
        }

        public float GetRadius()
        {
            float maxDistance = 0.0f;

            foreach (Vector3 corner in Corners)
            {
                float cornerCenterDis = Vector3.Distance(Center, corner);

                if (cornerCenterDis > maxDistance)
                {
                    maxDistance = cornerCenterDis;
                }
            }

            return maxDistance;
        }

        private IList<Vector3> GetCorners()
        {
            return new[] {
                new Vector3( Min.X, Min.Y, Min.Z ),
                new Vector3( Max.X, Min.Y, Min.Z ),
                new Vector3( Min.X, Max.Y, Min.Z ),
                new Vector3( Min.X, Min.Y, Max.Z ),
                new Vector3( Max.X, Max.Y, Min.Z ),
                new Vector3( Max.X, Min.Y, Max.Z ),
                new Vector3( Min.X, Max.Y, Max.Z ),
                new Vector3( Max.X, Max.Y, Max.Z ),
            };
        }
    }
}
