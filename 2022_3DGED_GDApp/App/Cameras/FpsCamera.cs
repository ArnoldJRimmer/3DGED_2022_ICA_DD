using Microsoft.Xna.Framework;
namespace GD.App
{
   public class FpsCamera
   {
        #region Fields
        private Vector3 position = Vector3.Zero;
        //I need a point the camera is going to be looking at, and a way of determining the direction as well
        private Vector3 lookAt;
        private Vector3 basePointOfReference = new Vector3(0, 0, 1);
        private Matrix storedViewMatrix;
        private bool needViewResync = true;
        private float rotation;
        #endregion

        #region Properties
        public Matrix Projection
        {
            get;
            private set;
        }

        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                UpdateLookAt();
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
                rotation = value;
                UpdateLookAt();
            }
        }

        /// <summary>
        /// I have a storing mechanism in place so that everytime the view matrix is calculated,#
        /// i can store it and get at easyily if needed.
        /// The view matirx takes in the camera position, the point we just looked at and a vector telling us the direction of UP for the camera
        /// </summary>
        public Matrix View
        {
            get
            {
                if (needViewResync)
                {
                    storedViewMatrix = Matrix.CreateLookAt(Position, lookAt, Vector3.Up);
                }

                return storedViewMatrix;
            }
        }

        #endregion

        #region Constructor
        public FpsCamera(Vector3 position, float rotation, float aspectRatio, float nearClip, float farClip)
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, nearClip, farClip);
            //This method allows you to specify where the camera is in a single call
            MoveTo(position, rotation);
        }
        #endregion

        #region HelperMethods
        private void UpdateLookAt()
        {
            Matrix rotationMatrix = Matrix.CreateRotationY(rotation);
            Vector3 lookAtOffSet = Vector3.Transform(basePointOfReference, rotationMatrix);
            lookAt = position + lookAtOffSet;
            needViewResync = true;
        }

        public void MoveTo(Vector3 _position, float _rotation)
        {
            this.position = _position;
            this.rotation = _rotation;
            UpdateLookAt();

        }

        public Vector3 PreviewMove(float scale)
        {
            Matrix rotate = Matrix.CreateRotationY(rotation);
            Vector3 forward = new Vector3(0, 0, scale);
            forward = Vector3.Transform(forward, rotate);
            return (position + forward);
        }

        public void MoveForward(float scale)
        {
            MoveTo(PreviewMove(scale), rotation);
        }
        #endregion
    }
}
