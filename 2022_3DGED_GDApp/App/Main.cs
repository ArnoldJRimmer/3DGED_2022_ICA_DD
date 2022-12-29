#region Pre-compiler directives

#define DEMO
#define SHOW_DEBUG_INFO

#endregion
using App.Levels.MakingTheMaze;
using GD.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GD.App
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private FpsCamera playerCamera;
        private Maze theLevel;
        private BasicEffect basicEffect;
        float moveScale = 1.5f;
        float rotateScale = MathHelper.PiOver2;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            playerCamera = new FpsCamera(new Vector3(0.5f, 0.5f, 0.5f), 0, GraphicsDevice.Viewport.AspectRatio, 0.05f, 100f);
            basicEffect = new BasicEffect(GraphicsDevice);
            theLevel = new Maze(GraphicsDevice);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyState = Keyboard.GetState();
            float moveAmount = 0;

            //Rotates the camera to the right
            if (keyState.IsKeyDown(Keys.Right))
            {

                //The camera has an angle and a speed at which it will rotate the mathhelper works this to one full revolution
                //The WrapAngle handles going over 360 and under 0 for us and returns a value that traverses this boundary
                //(ie. It does the maths for me because i'm an idiot and also i don't have time or to do it myself :) )
                playerCamera.Rotation = MathHelper.WrapAngle(playerCamera.Rotation - (rotateScale * timeElapsed));
            }

            //Rotates the camera to the Left
            if (keyState.IsKeyDown(Keys.Left))
            {
                playerCamera.Rotation = MathHelper.WrapAngle(playerCamera.Rotation + (rotateScale * timeElapsed));
            }

            //Allows the camera to move forward
            if (keyState.IsKeyDown(Keys.Up))
            {
                moveAmount = moveScale * timeElapsed;
            }

            //Allows the camera to move backward
            if (keyState.IsKeyDown(Keys.Down))
            {
                moveAmount = -moveScale * timeElapsed;
            }

            //As long as the player is moving 
            if (moveAmount != 0)
            {
                //we create a new vector 3 that holds the direction the player is facing and where they will end up if they moved forward
                Vector3 newLocation = playerCamera.PreviewMove(moveAmount);
                bool allowMovement = true;

                //We check to see if the player is within the bounds of the floor
                //If they are they can move, else they can't
                if (newLocation.X < 0 || newLocation.X > Maze.mazeWidth)
                {
                    allowMovement = false;
                }

                if (newLocation.Z < 0 || newLocation.Z > Maze.mazeHeight)
                {
                    allowMovement = false;
                }

                if (allowMovement)
                {
                    playerCamera.MoveForward(moveAmount);
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            theLevel.Draw(playerCamera, basicEffect);
            // TODO: Add your drawing code here
            base.Draw(gameTime);
        }
    }
}