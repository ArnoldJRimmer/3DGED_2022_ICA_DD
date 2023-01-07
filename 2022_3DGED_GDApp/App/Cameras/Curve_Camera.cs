using Microsoft.Xna.Framework;
using System;

namespace GD.App
{
    public class Curve_Camera
    {
        #region Fields
        private Vector3 cameraPosition = Vector3.Zero;
        private Vector3 theTargetPosition = Vector3.Zero;

        private float elevation;
        private float rotation;

        private float minDistance;
        private float maxDistance;
        private float viewDistance = 12f;

        private Vector3 baseCameraRef = new Vector3(0, 0, 1);
        private bool needViewResync = true;

        private Matrix cachedViewMatrix;
        #endregion

        #region Properties
        public Matrix Projection 
        {
            get;
            private set;
           
        }

        public Vector3 Target 
        {
            get
            {
                return theTargetPosition;
            }
            set
            {
                theTargetPosition = value;
                needViewResync = true;
            }
        }

        public Vector3 Position
        {
            get
            {
                return cameraPosition;
            }
        }

        public float Elevation
        {
            get
            {
                return elevation;
            }
            set
            {
                elevation = MathHelper.Clamp(value,MathHelper.ToRadians(-70),MathHelper.ToRadians(-10));
                needViewResync = true;
                
            }
        }

        public float Rotation 
        {
            get 
            {
                return rotation; 
            }

            set
            {
                rotation = MathHelper.WrapAngle(value);
                needViewResync = true;
            }
        }
        public float ViewDistance
        {
            get 
            { 
                return viewDistance; 
            }
            set
            {
                viewDistance = MathHelper.Clamp(value,minDistance,maxDistance);
            }
        }

        public Matrix View
        {
            get
            {
                if (needViewResync)
                {
                    Matrix transformMatrix = Matrix.CreateFromYawPitchRoll(rotation, elevation, 0.0f);
                    cameraPosition = Vector3.Transform(baseCameraRef, transformMatrix);
                    cameraPosition *= viewDistance;
                    cameraPosition += theTargetPosition;
                    cachedViewMatrix = Matrix.CreateLookAt(cameraPosition, theTargetPosition, Vector3.Up);
                }

                return cachedViewMatrix;
            }
        }
        #endregion

        #region Constructor
        public Curve_Camera(Vector3 targetPosition, float intialElevation, float intialRotation,float minDistance, float maxDistance, float initialDistance, float aspectRatio, float nearClip, float farClip)
        {
            Target = targetPosition;
            Elevation = intialElevation;
            Rotation = intialRotation;
            this.minDistance = minDistance;
            this.maxDistance = maxDistance;
            ViewDistance = initialDistance;

            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, nearClip, farClip);
            needViewResync = true;
        }
        #endregion
    }
}
