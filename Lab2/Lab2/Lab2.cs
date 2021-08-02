using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab2 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Effect effect;
        float angle = 0;
        float distance = 2;

        VertexPositionTexture[] vertices =
        {
            new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(0.5f, 0)),
            new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(1, 1)),
            new VertexPositionTexture(new Vector3(-1, 0, 0), new Vector2(0, 1))
        };

        public Lab2()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            // *********************************************
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            //**********************************************
        }
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }
        protected override void LoadContent()
        {
            
            spriteBatch = new SpriteBatch(GraphicsDevice);

            effect = Content.Load<Effect>("texture");
            effect.Parameters["MyTexture"].SetValue(Content.Load<Texture2D>("logo_mg"));

            Matrix world = Matrix.Identity;
            Matrix view = Matrix.CreateLookAt(new Vector3(1, 0, 1), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), GraphicsDevice.Viewport.AspectRatio, 0.1f, 100);

            effect.Parameters["World"].SetValue(world);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
            // TODO: use this.Content to load your game content here
        }
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                angle += 0.02f;
                Vector3 cameraPosition = distance * new Vector3( (float) System.Math.Sin(angle), 0, (float)System.Math.Cos(angle));
                Matrix view = Matrix.CreateLookAt(cameraPosition, new Vector3(0, 0, 0), new Vector3(0, 1, 0));
                effect.Parameters["View"].SetValue(view);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                angle -= 0.02f;
                Vector3 cameraPosition = distance * new Vector3((float)System.Math.Sin(angle), 0, (float)System.Math.Cos(angle));
                Matrix view = Matrix.CreateLookAt(cameraPosition, new Vector3(0, 0, 0), new Vector3(0, 1, 0));
                effect.Parameters["View"].SetValue(view);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                distance -= 0.02f;
                Vector3 cameraPosition = distance * new Vector3((float)System.Math.Sin(angle), 0, (float)System.Math.Cos(angle));
                Matrix view = Matrix.CreateLookAt(cameraPosition, new Vector3(0, 0, 0), new Vector3(0, 1, 0));
                effect.Parameters["View"].SetValue(view);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                distance += 0.02f;
                Vector3 cameraPosition = distance * new Vector3((float)System.Math.Sin(angle), 0, (float)System.Math.Cos(angle));
                Matrix view = Matrix.CreateLookAt(cameraPosition, new Vector3(0, 0, 0), new Vector3(0, 1, 0));
                effect.Parameters["View"].SetValue(view);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(
                //GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                  PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3);
            }

            base.Draw(gameTime);
        }
    }
}
