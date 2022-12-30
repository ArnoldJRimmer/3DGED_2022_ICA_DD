using GD.App;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
namespace App.Levels.MakingTheMaze
{
    public class Maze
    {
        #region Declarations
        private Random pickWall = new Random();
        public MazeCell[,] theMazeCells = new MazeCell[mazeWidth, mazeHeight];
        GraphicsDevice myDevice;
        VertexBuffer floorBuffer;
        VertexBuffer wallBuffer;

        Vector3[] pointsOfTheWall = new Vector3[8];

        Color[] floorColors = new Color[2]
        {
            Color.White,
            Color.Black,
        };

        Color[] colorOfWalls = new Color[4]
        {
            Color.Green,
            Color.Red,
            Color.Green,
            Color.Red
        };
        #endregion
        #region Fields
        //Appdata this
        public const int mazeWidth = 20;
        public const int mazeHeight = 20;
        #endregion

        #region Constructor
        public Maze(GraphicsDevice _myDevice)
        {
            myDevice = _myDevice;
            BuildFloorBuffer();
            for (int x = 0; x < mazeWidth; x++)
            {
                for (int z = 0; z < mazeHeight; z++)
                {
                    theMazeCells[x, z] = new MazeCell();
                }
            }

            GenerateMaze();

            //I create a cube that i will use to make up any four walls in my maze 
            pointsOfTheWall[0] = new Vector3(0, 1, 0);
            pointsOfTheWall[1] = new Vector3(0, 1, 1);
            pointsOfTheWall[2] = new Vector3(0, 0, 0);
            pointsOfTheWall[3] = new Vector3(0, 0, 1);
            pointsOfTheWall[4] = new Vector3(1, 1, 0);
            pointsOfTheWall[5] = new Vector3(1, 1, 1);
            pointsOfTheWall[6] = new Vector3(1, 0, 0);
            pointsOfTheWall[7] = new Vector3(1, 0, 1);

            BuildWallBuffer();
        }
        #endregion

        #region Generate Maze
        //Here is where we set up the maze, each time we call this we create a maze our of hollowed out cubes, meaning there isn't anywhere to go
        public void GenerateMaze()
        {
            for (int x = 0; x < mazeWidth; x++)
            {
                for (int z = 0; z < mazeHeight; z++)
                {
                    theMazeCells[x, z].Walls[0] = true;
                    theMazeCells[x, z].Walls[1] = true;
                    theMazeCells[x, z].Walls[2] = true;
                    theMazeCells[x, z].Walls[3] = true;
                    theMazeCells[x, z].Visited = false;
                }
            }

            theMazeCells[0, 0].Visited = true;
            EvaluateCell(new Vector2(0, 0));
        }

        private void EvaluateCell(Vector2 cell)
        {
            List<int> theNextCell = new List<int>();
            theNextCell.Add(0);
            theNextCell.Add(1);
            theNextCell.Add(2);
            theNextCell.Add(3);

            while (theNextCell.Count > 0)
            {
                int pick = pickWall.Next(0, theNextCell.Count);
                int selectedNextCell = theNextCell[pick];
                theNextCell.RemoveAt(pick);

                Vector2 nextCell = cell;

                switch (selectedNextCell)
                {
                    case 0:
                        {
                            nextCell += new Vector2(0, -1);
                            break;
                        }

                    case 1:
                        {
                            nextCell += new Vector2(1, 0);
                            break;
                        }

                    case 2:
                        {
                            nextCell += new Vector2(0, 1);
                            break;
                        }

                    case 3:
                        {
                            nextCell += new Vector2(-1, 0);
                            break;
                        }

                }

                if (nextCell.X >= 0 && nextCell.X < mazeHeight && nextCell.Y >= 0 && nextCell.Y < mazeHeight)
                {

                    if (!theMazeCells[(int)nextCell.X, (int)nextCell.Y].Visited)
                    {
                        theMazeCells[(int)nextCell.X, (int)nextCell.Y].Visited = true;
                        theMazeCells[(int)cell.X, (int)cell.Y].Walls[selectedNextCell] = false;
                        theMazeCells[(int)nextCell.X, (int)nextCell.Y].Walls[(selectedNextCell + 2) % 4] = false;
                        EvaluateCell(nextCell);
                    }

                }

            }
        }
        #endregion

        /// <Building the Walls>
        /// I create the walls for the maze by using these eight 
        /// points.,from these i create triangles and then offsett their locations to move them to the appropriate
        /// position within my maze. As each wall in my mace can consist of any of these 4 points.
        /// </summary>
        #region Walls
        //Loops through each of the cells in my maze and calls BuildMazeWall
        private void BuildWallBuffer()
        {
            List<VertexPositionColor> wallVertexList = new List<VertexPositionColor>();
            for (int x = 0; x < mazeWidth; x++)
            {
                for (int z = 0; z < mazeHeight; z++)
                {
                    //Need to write this method
                    foreach (VertexPositionColor vertex in BuildMazeWall(x, z))
                    {
                        wallVertexList.Add(vertex);
                    }
                }
            }

            wallBuffer = new VertexBuffer(myDevice, VertexPositionColor.VertexDeclaration, wallVertexList.Count, BufferUsage.WriteOnly);
            wallBuffer.SetData<VertexPositionColor>(wallVertexList.ToArray());
        }

        //Because our graphics cards likes things in groups we get all the trianlges we need together to make the walls, which we then store in the wallBuffer 
        private List<VertexPositionColor> BuildMazeWall(int x, int z)
        {
            List<VertexPositionColor> triangleThatMakeUpWalls = new List<VertexPositionColor>();
            if (theMazeCells[x, z].Walls[0])
            {
                triangleThatMakeUpWalls.Add(CalculatePoints(0, x, z, colorOfWalls[0]));
                triangleThatMakeUpWalls.Add(CalculatePoints(4, x, z, colorOfWalls[0]));
                triangleThatMakeUpWalls.Add(CalculatePoints(2, x, z, colorOfWalls[0]));
                triangleThatMakeUpWalls.Add(CalculatePoints(4, x, z, colorOfWalls[0]));
                triangleThatMakeUpWalls.Add(CalculatePoints(6, x, z, colorOfWalls[0]));
                triangleThatMakeUpWalls.Add(CalculatePoints(2, x, z, colorOfWalls[0]));
            }

            if (theMazeCells[x, z].Walls[1])
            {
                triangleThatMakeUpWalls.Add(CalculatePoints(4, x, z, colorOfWalls[1]));
                triangleThatMakeUpWalls.Add(CalculatePoints(5, x, z, colorOfWalls[1]));
                triangleThatMakeUpWalls.Add(CalculatePoints(6, x, z, colorOfWalls[1]));
                triangleThatMakeUpWalls.Add(CalculatePoints(5, x, z, colorOfWalls[1]));
                triangleThatMakeUpWalls.Add(CalculatePoints(7, x, z, colorOfWalls[1]));
                triangleThatMakeUpWalls.Add(CalculatePoints(6, x, z, colorOfWalls[1]));
            }

            if (theMazeCells[x, z].Walls[2])
            {
                triangleThatMakeUpWalls.Add(CalculatePoints(5, x, z, colorOfWalls[2]));
                triangleThatMakeUpWalls.Add(CalculatePoints(1, x, z, colorOfWalls[2]));
                triangleThatMakeUpWalls.Add(CalculatePoints(7, x, z, colorOfWalls[2]));
                triangleThatMakeUpWalls.Add(CalculatePoints(1, x, z, colorOfWalls[2]));
                triangleThatMakeUpWalls.Add(CalculatePoints(3, x, z, colorOfWalls[2]));
                triangleThatMakeUpWalls.Add(CalculatePoints(7, x, z, colorOfWalls[2]));
            }

            if (theMazeCells[x, z].Walls[3])
            {
                triangleThatMakeUpWalls.Add(CalculatePoints(1, x, z, colorOfWalls[3]));
                triangleThatMakeUpWalls.Add(CalculatePoints(0, x, z, colorOfWalls[3]));
                triangleThatMakeUpWalls.Add(CalculatePoints(3, x, z, colorOfWalls[3]));
                triangleThatMakeUpWalls.Add(CalculatePoints(0, x, z, colorOfWalls[3]));
                triangleThatMakeUpWalls.Add(CalculatePoints(2, x, z, colorOfWalls[3]));
                triangleThatMakeUpWalls.Add(CalculatePoints(3, x, z, colorOfWalls[3]));
            }

            return triangleThatMakeUpWalls;
        }

        private VertexPositionColor CalculatePoints(int wallPoint, int x_OffSet, int z_OffSet, Color colorOfTriangles)
        {
            return new VertexPositionColor(pointsOfTheWall[wallPoint] + new Vector3(x_OffSet, 0, z_OffSet), colorOfTriangles);
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
                    counter++;
                    foreach (VertexPositionColor vertex in FloorTile(x, z, floorColors[counter % 2]))
                    {
                        vertexList.Add(vertex);
                    }
                }
            }

            floorBuffer = new VertexBuffer(myDevice, VertexPositionColor.VertexDeclaration, vertexList.Count, BufferUsage.WriteOnly);
            floorBuffer.SetData(vertexList.ToArray());

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
