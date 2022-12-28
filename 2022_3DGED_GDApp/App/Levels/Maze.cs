using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
namespace GD.App
{
    public class Maze
    {
        #region Fields
        public const int mazeWidth = 20;
        public const int mazeHeight = 20;
        GraphicsDevice myDevice;
        VertexBuffer floorBuffer;
        Color[] floorColors = new Color[2]
        {
            Color.White,
            Color.Black,
        };
        #endregion

        #region Constructor
        public Maze(GraphicsDevice _myDevice)
        {
            this.myDevice = _myDevice;
            BuildFloorBuffer();
        }
        #endregion

        #region The Floor
        private void BuildFloorBuffer()
        {
            List<VertexPositionColor> vertexList = new List<VertexPositionColor>();
            int counter = 0;

            for (int x = 0; x < mazeWidth; x++)
            {
                counter++;
                for (int z = 0; z < mazeHeight; z++)
                {
                    foreach (VertexPositionColor vertex in FloorTile(x, z, floorColors[counter % 2]))
                    {
                        vertexList.Add(vertex);
                    }
                }
            }

            floorBuffer = new VertexBuffer(myDevice, VertexPositionColor.VertexDeclaration, vertexList.Count, BufferUsage.WriteOnly);
            floorBuffer.SetData<VertexPositionColor>(vertexList.ToArray());

        }

        private List<VertexPositionColor> FloorTile(int x_OffSet, int z_Offset, Color tileColor)
        {
            List<VertexPositionColor> verticesList = new List<VertexPositionColor>();

            //Tidy up all tile positions using AppData.cs
            verticesList.Add(new VertexPositionColor(new Vector3(0 + x_OffSet, 0, 0 + z_Offset), tileColor));
            verticesList.Add(new VertexPositionColor(new Vector3(1 + x_OffSet, 0, 0 + z_Offset), tileColor));
            verticesList.Add(new VertexPositionColor(new Vector3(0 + x_OffSet, 0, 1 + z_Offset), tileColor));
            verticesList.Add(new VertexPositionColor(new Vector3(1 + x_OffSet, 0, 0 + z_Offset), tileColor));
            verticesList.Add(new VertexPositionColor(new Vector3(1 + x_OffSet, 0, 1 + z_Offset), tileColor));
            verticesList.Add(new VertexPositionColor(new Vector3(0 + x_OffSet, 0, 1 + z_Offset), tileColor));

            return verticesList;

        }

        #endregion


        #region Draw
        public void Draw(FpsCamera fpCamera, BasicEffect effect)
        {
            effect.VertexColorEnabled = true;
            effect.World = Matrix.Identity;
            effect.View = fpCamera.View;
            effect.Projection = fpCamera.Projection;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                myDevice.SetVertexBuffer(floorBuffer);
                myDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, floorBuffer.VertexCount / 3);
            }
        }
        #endregion
    }
}
