﻿
using GD.Engine;
using GD.Engine.Globals;
using Microsoft.VisualBasic.Devices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;

namespace GD.App
{
    public class Main : Game
    {
        #region Declarations
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont scoreFont;
        private FpsCamera playerCamera;
        TheCollectable floatyCube;
        private Maze theLevel;
        private BasicEffect basicEffect;
        private Texture2D startMenu;
        private Song startSong;
        private Song pickUp;
        enum GameState
        {
            TitleScreen,
            Playing,
            GameOver
        }

        GameState currentGameState = GameState.TitleScreen;
        #endregion

        #region Fields
        private int score = 3000;
        private float moveAmount;
        private bool isActive = false;
        private bool stopDrawing = false;
        #endregion

        #region Main
        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        #endregion

        #region Initialize
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            playerCamera = new FpsCamera(MyGameVariable.FIRST_PERSON_DEFAULT_CAMERA_POSITION,
                0, 
                GraphicsDevice.Viewport.AspectRatio, 
                MyGameVariable.FIRST_PERSON_CAMERA_NCP,
                MyGameVariable.FIRST_PERSON_CAMERA_FCP);
            basicEffect = new BasicEffect(GraphicsDevice);
            theLevel = new Maze(GraphicsDevice);
            base.Initialize();
        }
        #endregion

        #region LoadContent
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            //The collectable
            floatyCube = new TheCollectable(this.GraphicsDevice, playerCamera.Position, MyGameVariable.MINIUM_DISTANCE, Content.Load<Texture2D>("Assets/Textures/MyTextures/collectable"));

            //Title Screen
            startMenu = Content.Load<Texture2D>("Assets/Textures/MyTextures/StartMenu");
            pickUp = Content.Load<Song>("Assets/Audio/Diegetic/Pick_up-sound");
            scoreFont = Content.Load<SpriteFont>("Assets/Fonts/menu");
            LoadSounds();
        }
        #endregion

        #region LoadSounds
        private void LoadSounds()
        {
            startSong = Content.Load<Song>("Assets/Audio/Non-Diegetic/StartMenu_Audio");
            MediaPlayer.Play(startSong);
            MediaPlayer.IsRepeating = true;
        }
        #endregion

        #region Update
        protected override void Update(GameTime gameTime)
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyState = Keyboard.GetState();

            //Starts the game
            if (keyState.IsKeyDown(Keys.Space))
            {
                isActive = true;
            }

            #region CoreGameFunctionailty
            //The core fucntionality of the game
            if (isActive == true)
            {

                moveAmount = 0;
                //Rotates the camera to the right
                if (keyState.IsKeyDown(Keys.Right))
                {
                    //The camera has an angle and a speed at which it will rotate the mathhelper works this to one full revolution
                    //The WrapAngle handles going over 360 and under 0 for us and returns a value that traverses this boundary
                    //(ie. It does the maths for me because i'm an idiot and also i don't have time to do it myself :) )
                    playerCamera.Rotation = MathHelper.WrapAngle(playerCamera.Rotation - (MyGameVariable.ROTATE_SCALE * timeElapsed));
                }

                //Rotates the camera to the Left
                if (keyState.IsKeyDown(Keys.Left))
                {
                    playerCamera.Rotation = MathHelper.WrapAngle(playerCamera.Rotation + (MyGameVariable.ROTATE_SCALE * timeElapsed));
                }

                //Allows the camera to move forward
                if (keyState.IsKeyDown(Keys.Up))
                {
                    moveAmount = MyGameVariable.MOVE_SCALE * timeElapsed;
                }

                //Allows the camera to move backward
                if (keyState.IsKeyDown(Keys.Down))
                {
                    moveAmount = -MyGameVariable.MOVE_SCALE * timeElapsed;
                }

                //Turns off the maze to make it easier
                if (keyState.IsKeyDown(Keys.M))
                {
                    stopDrawing = true;
                }
                else if (keyState.IsKeyUp(Keys.M))
                {
                    stopDrawing = false;
                }

                //As long as the player is moving 
                if (moveAmount != 0)
                {
                    //we create a new vector 3 that holds the direction the player is facing and where they will end up if they moved forward
                    Vector3 newLocation = playerCamera.PreviewMove(moveAmount);
                    bool allowMovement = true;

                    //We check to see if the player is within the bounds of the floor
                    //If they are they can move, else they can't
                    if (newLocation.X < 0 || newLocation.X > MyGameVariable.MAZE_WIDTH)
                    {
                        allowMovement = false;
                    }

                    if (newLocation.Z < 0 || newLocation.Z > MyGameVariable.MAZE_HEIGHT)
                    {
                        allowMovement = false;
                    }

                    //Here I add each bounding box to each of the walls, if a wall exists in this location i also don't allow the player to move here
                    foreach (BoundingBox box in theLevel.GetBoundingForMazeCell((int)newLocation.X, (int)newLocation.Z))
                    {
                        if (box.Contains(newLocation) == ContainmentType.Contains)
                        {
                            allowMovement = false;
                        }
                    }

                    if (allowMovement)
                    {
                        playerCamera.MoveForward(moveAmount);
                    }

                    if (floatyCube.colliable.Contains(playerCamera.Position) == ContainmentType.Contains)
                    {
                        floatyCube.PositionCollectable(playerCamera.Position, 5f);
                        //Every time the player picks up a cube, the position of the cube changes aswell as the layout of the maze
                        CalculateScore(gameTime);
                        theLevel = new Maze(GraphicsDevice);
                    }

                    


                }

                floatyCube.Update(gameTime);
            }

            base.Update(gameTime);
            #endregion

        }
        #endregion

        #region Draw
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            CheckGameState();
            base.Draw(gameTime);
        }
        #endregion

        #region Helper Methods
        private void CalculateScore(GameTime gameTime)
        {
            score += MyGameVariable.PICK_UP_SCORE;
        }

        private void CheckGameState()
        {
            if (isActive == false && score == 0)
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(startMenu, Vector2.Zero, Color.White);
                _spriteBatch.End();
            }
            else
            {
                if (stopDrawing && score != MyGameVariable.END_SCORE)
                {
                    //theLevel.Draw(playerCamera, basicEffect);
                    floatyCube.Draw(playerCamera, basicEffect);
                }
                else
                {
                    if (score != MyGameVariable.END_SCORE)
                    {
                        theLevel.Draw(playerCamera, basicEffect);
                        floatyCube.Draw(playerCamera, basicEffect);
                        _spriteBatch.Begin();
                        _spriteBatch.DrawString(scoreFont, "Score: " + score.ToString(), Vector2.Zero, Color.White);
                        _spriteBatch.End();
                    }
                    else
                    {
                        isActive = false;
                    }

                }

            }

        }
        #endregion

    }

}