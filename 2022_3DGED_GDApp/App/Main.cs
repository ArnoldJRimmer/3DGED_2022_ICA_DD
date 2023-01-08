
using GD.Engine;
using GD.Engine.Globals;
using GD.Engine.Managers;
using GD.Engine.Parameters;
using Microsoft.VisualBasic.Devices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;
using Mouse = Microsoft.Xna.Framework.Input.Mouse;

namespace GD.App
{
    public class Main : Game
    {
        #region Declarations
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont scoreFont;
        private FpsCamera playerCamera;
        private Curve_Camera curveCamera;
        TheCollectable floatyCube;
        private Maze theLevel;
        private BasicEffect basicEffect;
        private Texture2D startMenu;
        private Song startSong;
        private Song pickUp;
        Point centerOfScreen;
        Point saveMousePoint;
        float scrollRate = 1f;
        MouseState prevMouse;
        #endregion

        #region Fields
        private int score;
        private float time = 3f;
        private float moveAmount;
        private bool isActive = false;
        private bool stopDrawing = false;
        private bool allowMouseMove = false;
        private bool enableMouseView = false;
        #endregion

        #region Main
        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
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
            #region Curve Camera
            curveCamera = new Curve_Camera(new Vector3(0f, 0f, 0f),
               MathHelper.ToRadians(2),
               0f,
               10f,
               60f,
               20f,
               GraphicsDevice.Viewport.AspectRatio,
               0.1f,
               512f);

            centerOfScreen.X = this.Window.ClientBounds.Width / 2;
            centerOfScreen.Y = this.Window.ClientBounds.Height / 2;

            this.IsMouseVisible = true;
            prevMouse = Mouse.GetState();
            Mouse.SetPosition(centerOfScreen.X, centerOfScreen.Y);
            #endregion

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

            pickUp = Content.Load<Song>("Assets/Audio/Diegetic/Pick_up-sound");
            LoadGraphicMedia();
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

        #region LoadMedia
        private void LoadGraphicMedia()
        {
            //Title Screen
            startMenu = Content.Load<Texture2D>("Assets/Textures/MyTextures/StartMenu");
            //Score Font
            scoreFont = Content.Load<SpriteFont>("Assets/Fonts/menu");
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
            if (keyState.IsKeyDown(Keys.Space) && score!=MyGameVariable.END_SCORE && time > 0)
            {
                isActive = true;
            }

            #region CoreGameFunctionailty
            //The core fucntionality of the game
            if (isActive == true)
            {
                moveAmount = 0;
                time -= (float)gameTime.ElapsedGameTime.TotalMinutes;
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

                #region Press F1 To See Mouse View
                if (keyState.IsKeyDown(Keys.F1))
                {
                    enableMouseView = true;
                    MouseState mouse = Mouse.GetState();
                    if (allowMouseMove)
                    {
                        curveCamera.Rotation += MathHelper.ToRadians((mouse.X - centerOfScreen.X) / 2f);
                        curveCamera.Elevation += MathHelper.ToRadians((mouse.Y - centerOfScreen.Y) / 2f);
                        Mouse.SetPosition(centerOfScreen.X, centerOfScreen.Y);
                    }

                    if (mouse.RightButton == ButtonState.Pressed)
                    {
                        if (!allowMouseMove && prevMouse.RightButton == ButtonState.Released)
                        {
                            if (_graphics.GraphicsDevice.Viewport.Bounds.Contains(new Point(mouse.X, mouse.Y)))
                            {
                                allowMouseMove = true;
                                saveMousePoint.X = mouse.X;
                                saveMousePoint.Y = mouse.Y;
                                Mouse.SetPosition(centerOfScreen.X, centerOfScreen.Y);
                                this.IsMouseVisible = false;
                            }
                        }
                    }
                    else
                    {
                        if (allowMouseMove)
                        {
                            allowMouseMove = false;
                            Mouse.SetPosition(saveMousePoint.X, saveMousePoint.Y);
                            this.IsMouseVisible = true;
                        }
                    }

                    if (mouse.ScrollWheelValue - prevMouse.ScrollWheelValue != 0)
                    {
                        float wheelChange = mouse.ScrollWheelValue - prevMouse.ScrollWheelValue;
                        curveCamera.ViewDistance -= (wheelChange / 120) * scrollRate;
                    }
                    prevMouse = mouse;
                }
                else
                {
                    enableMouseView = false;
                }
                #endregion

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
                        CalculateScore();
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
        private int CalculateScore()
        {
           return score += MyGameVariable.PICK_UP_SCORE;
        }
        #endregion

        #region CheckState
        private void CheckGameState()
        {
            //Start menu
            if (isActive == false && score == 0 && time > 0)
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(startMenu, Vector2.Zero, Color.White);
                _spriteBatch.End();
            }
            else
            {
                
                if (stopDrawing && score != MyGameVariable.END_SCORE && time > 0)
                {
                    //theLevel.Draw(playerCamera, basicEffect);
                    floatyCube.Draw(playerCamera, basicEffect);
                }
                else
                {

                    if (score != MyGameVariable.END_SCORE && time > 0)
                    {
                        theLevel.Draw(playerCamera, basicEffect);
                        floatyCube.Draw(playerCamera, basicEffect);
                        _spriteBatch.Begin();
                        _spriteBatch.DrawString(scoreFont, "Score: " + score.ToString(), Vector2.Zero, Color.White);
                        _spriteBatch.DrawString(scoreFont, "Time: " + time.ToString("0.00"), new Vector2(500, 0), Color.White);
                        _spriteBatch.End();
                    }
                    else
                    {
                        if (time <= 0)
                        {
                            //Draw in GameOver
                            _spriteBatch.Begin();
                            _spriteBatch.DrawString(scoreFont, "GameOver - Ran out of time", Vector2.Zero, Color.White);
                            _spriteBatch.End();
                            isActive = false;
                        }

                        //Draw in the "You Won State"
                        _spriteBatch.Begin();
                        _spriteBatch.DrawString(scoreFont, "You won", Vector2.Zero, Color.White);
                        _spriteBatch.End();

                        isActive = false;
                    }

                }

                //Mouse View
                if (enableMouseView)
                {
                    theLevel.Draw(curveCamera, basicEffect);
                }

            }

           

        }
        #endregion

    }

}