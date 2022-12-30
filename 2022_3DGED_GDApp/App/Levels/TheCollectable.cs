
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GD.App
{
  public class TheCollectable
  {

        #region Fields
        GraphicsDevice myDevice;
        private Texture2D myTexture;
        private Vector3 location;
        private VertexBuffer collectableVertexBuffer;
        private List<VertexPositionTexture> verts = new List<VertexPositionTexture>();
        private float rotation = 0f;
        private float z_Rotation = 0f;
        private Random randPos = new Random();
        private const float collisionRadius = 0.25f;
        #endregion

        #region BoundingSphereColliable 
        public BoundingSphere colliable
        {
            get
            {
                return new BoundingSphere(location, collisionRadius);
            }
        }
        #endregion

        #region Constructor
        public TheCollectable(GraphicsDevice _graphicsDevice, Vector3 playerLocation, float minDistance, Texture2D _texture)
        {
            myDevice = _graphicsDevice;
            this.myTexture = _texture;
            PositionCollectable(playerLocation, minDistance);

            //Create the colleactables vertical faces
            BuildVerticalFace(new Vector3(0, 0, 0), new Vector3(0, 1, 1));
            BuildVerticalFace(new Vector3(0, 0, 1), new Vector3(1, 1, 1));
            BuildVerticalFace(new Vector3(1, 0, 1), new Vector3(1, 1, 0));
            BuildVerticalFace(new Vector3(1, 0, 0), new Vector3(0, 1, 0));


            //Create the colleactables horizontal faces
            BuildHorizontalFace(new Vector3(0, 1, 0), new Vector3(1, 1, 1));
            BuildHorizontalFace(new Vector3(0, 0, 1), new Vector3(1, 0, 0));

            collectableVertexBuffer = new VertexBuffer(myDevice, VertexPositionTexture.VertexDeclaration, verts.Count, BufferUsage.WriteOnly);
            collectableVertexBuffer.SetData<VertexPositionTexture>(verts.ToArray());

        }
        #endregion

        #region HelperMethods
        private void BuildVerticalFace(Vector3 point1, Vector3 point2)
        {
            verts.Add(BuildVertex(point1.X, point1.Y, point1.Z, 1, 0));
            verts.Add(BuildVertex(point1.X, point2.Y, point1.Z, 1, 1));
            verts.Add(BuildVertex(point2.X, point2.Y, point2.Z, 0, 1));
            verts.Add(BuildVertex(point2.X, point2.Y, point2.Z, 0, 1));
            verts.Add(BuildVertex(point2.X, point1.Y, point2.Z, 0, 0));
            verts.Add(BuildVertex(point1.X, point1.Y, point1.Z, 1, 0));
        }

        private void BuildHorizontalFace(Vector3 point1, Vector3 point2)
        {
            verts.Add(BuildVertex(point1.X, point1.Y, point1.Z, 0, 1));
            verts.Add(BuildVertex(point2.X, point1.Y, point1.Z, 1, 1));
            verts.Add(BuildVertex(point2.X, point2.Y, point2.Z, 1, 0));
            verts.Add(BuildVertex(point1.X, point1.Y, point1.Z, 0, 1));
            verts.Add(BuildVertex(point2.X, point2.Y, point2.Z, 1, 0));
            verts.Add(BuildVertex(point1.X, point1.Y, point2.Z, 0, 0));
        }

        private VertexPositionTexture BuildVertex(float x, float y, float z, float u, float v)
        {
            return new VertexPositionTexture(new Vector3(x, y, z), new Vector2(u, v));
        }

        //Starting position of the cube
        //public void PositionCollectable(Vector3 playerLocation, float minDistance)
        //{
        //    location = new Vector3(1.5f, 0.5f, 1.5f);
        //}

        public void PositionCollectable(Vector3 playerLocation, float minDistance)
        {
            Vector3 newLocation;
            do
            {
                newLocation = new Vector3(randPos.Next(0, MyGameVariable.MAZE_WIDTH) + 0.5f, 0.5f, randPos.Next(0, MyGameVariable.MAZE_HEIGHT) + 0.5f);
            }
            while (Vector3.Distance(playerLocation, newLocation) < minDistance);
            {
                location = newLocation;
            }

        }
        #endregion

        #region Draw
        public void Draw(FpsCamera fpCamera, BasicEffect effect)
        {
            effect.VertexColorEnabled = false;
            effect.TextureEnabled = true;
            effect.Texture = myTexture;

            Matrix center = Matrix.CreateTranslation(new Vector3(-0.5f, -0.5f, -0.5f));
            Matrix scale = Matrix.CreateScale(0.5f);
            Matrix translate = Matrix.CreateTranslation(location);
            Matrix y_Rotation = Matrix.CreateRotationY(rotation);
            Matrix z_Rot = Matrix.CreateRotationZ(z_Rotation);

            effect.World = center * y_Rotation * z_Rot * scale * translate;
            effect.View = fpCamera.View;
            effect.Projection = fpCamera.Projection;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                myDevice.SetVertexBuffer(collectableVertexBuffer);
                myDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, collectableVertexBuffer.VertexCount / 3);
            }

        }
        #endregion

        #region Update
        public void Update(GameTime gameTime)
        {
            //Divide by a number to slow the rotaion down, because with out this it rotates on every frame
            rotation = MathHelper.WrapAngle(rotation + 0.5f / 30f);
            z_Rotation = MathHelper.WrapAngle(z_Rotation + 0.025f / 30f);
        }
        #endregion
    }
}

