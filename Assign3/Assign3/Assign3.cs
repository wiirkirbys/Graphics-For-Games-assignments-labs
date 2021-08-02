using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SimpleEngine;

namespace Assign3
{
    public class Assign3 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Effect effect;
        Model model;
        Texture2D texture;
        Skybox skybox;
        Matrix world;
        Matrix view;
        Matrix projection;
        Vector3 cameraPosition = new Vector3(0, 0, 10);
        Vector3 lightPosition = new Vector3(0, 0, 10);
        float angle = 0;
        float angle2 = 0;
        float distance = 30;
        float angleL = 0;
        float angleL2 = 0;
        MouseState previousMouseState;
        KeyboardState previousKey;
        bool mipmap = true;
        Vector3 UVW = new Vector3(2.5f, 2.5f, 1f);
        bool helpOn = false, infoOn = false;
        bool image = false;

        public Assign3()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            model = Content.Load<Model>("Torus");
            effect = Content.Load<Effect>("SimpleShader");
            texture = Content.Load<Texture2D>("NormalMaps/round");
            string[] skyboxTextures = new string[] { "skybox/nvlobby_new_negx", "skybox/nvlobby_new_posx", "skybox/nvlobby_new_negy", "skybox/nvlobby_new_posy", "skybox/nvlobby_new_negz", "skybox/nvlobby_new_posz" };
            skybox = new Skybox(skyboxTextures, Content, graphics.GraphicsDevice);
            font = Content.Load<SpriteFont>("font");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseState currentMouseState = Mouse.GetState();
            if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed)
            {
                angle += (previousMouseState.X - currentMouseState.X) / 100f;
                angle2 += (previousMouseState.Y - currentMouseState.Y) / 100f;
            }
            world = Matrix.Identity;
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 800f / 600f, .1f, 100f);
            cameraPosition = Vector3.Transform(new Vector3(0, 0, distance), Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
            view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Transform(Vector3.Up, Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle)));

            if (Keyboard.GetState().IsKeyDown(Keys.Left)) angleL += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) angleL -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) angleL2 += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) angleL2 -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                angleL2 = 0;
                angleL = 0;
                angle = 0;
                angle2 = 0;
                cameraPosition = Vector3.Transform(new Vector3(0, 0, distance), Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Transform(Vector3.Up, Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle)));
            }
            lightPosition = Vector3.Transform(new Vector3(0, 0, 10), Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL));

            //changes normal maps
            if (Keyboard.GetState().IsKeyDown(Keys.D1)) texture = Content.Load<Texture2D>("NormalMaps/round");
            if (Keyboard.GetState().IsKeyDown(Keys.D2)) texture = Content.Load<Texture2D>("NormalMaps/BumpTest");
            if (Keyboard.GetState().IsKeyDown(Keys.D3)) texture = Content.Load<Texture2D>("NormalMaps/crossHatch");
            if (Keyboard.GetState().IsKeyDown(Keys.D4)) texture = Content.Load<Texture2D>("NormalMaps/square");
            if (Keyboard.GetState().IsKeyDown(Keys.D5)) texture = Content.Load<Texture2D>("NormalMaps/art");
            if (Keyboard.GetState().IsKeyDown(Keys.D5)) texture = Content.Load<Texture2D>("NormalMaps/science");
            if (Keyboard.GetState().IsKeyDown(Keys.D6)) texture = Content.Load<Texture2D>("NormalMaps/monkey");
            if (Keyboard.GetState().IsKeyDown(Keys.D7)) texture = Content.Load<Texture2D>("NormalMaps/saint");

            //changes shaders
            if (Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                effect = Content.Load<Effect>("SimpleShader");
                image = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F2))
            {
                effect = Content.Load<Effect>("RGB");
                image = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F3))
            {
                effect = Content.Load<Effect>("BumpMap");
                image = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F4))
            {
                effect = Content.Load<Effect>("Reflect");
                image = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F5))
            {
                effect = Content.Load<Effect>("Refract");
                image = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F6))
            {
                effect = Content.Load<Effect>("AdvancedShaders/F6");
                image = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F7))
            {
                effect = Content.Load<Effect>("AdvancedShaders/F7");
                image = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F8))
            {
                effect = Content.Load<Effect>("AdvancedShaders/F8");
                image = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F9))
            {
                effect = Content.Load<Effect>("AdvancedShaders/F9");
                image = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F10)) image = true;

            if (Keyboard.GetState().IsKeyDown(Keys.M) && !previousKey.IsKeyDown(Keys.M)) mipmap = !mipmap;

            if (Keyboard.GetState().IsKeyDown(Keys.U))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    UVW.X -= .1f;
                }
                else UVW.X += .1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.V))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    UVW.Y -= .1f;
                }
                else UVW.Y += .1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    UVW.Z -= .1f;
                }
                else UVW.Z += .1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemQuestion) && !previousKey.IsKeyDown(Keys.OemQuestion)) helpOn = !helpOn;
            if (Keyboard.GetState().IsKeyDown(Keys.H) && !previousKey.IsKeyDown(Keys.H)) infoOn = !infoOn;

            previousKey = Keyboard.GetState();
            previousMouseState = Mouse.GetState();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();
            effect.CurrentTechnique = effect.Techniques[0];
            RasterizerState originalRasterizerState = graphics.GraphicsDevice.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphics.GraphicsDevice.RasterizerState = rasterizerState;
            skybox.Draw(view, projection, cameraPosition);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        Matrix worldInverseTranspose = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTranspose);
                        effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                        effect.Parameters["LightPosition"].SetValue(lightPosition);
                        effect.Parameters["normalMap"].SetValue(texture);
                        effect.Parameters["DiffuseColor"].SetValue(new Vector4(1, 1, 1, 1));
                        effect.Parameters["DiffuseIntensity"].SetValue(1.0f);
                        effect.Parameters["SpecularColor"].SetValue(new Vector4(1, 1, 1, 1));
                        effect.Parameters["SpecularIntensity"].SetValue(1.0f);
                        effect.Parameters["Shininess"].SetValue(100f);
                        if (effect.Name == "Reflect" || effect.Name == "Refract" || effect.Name == "Skybox")
                        {
                            effect.Parameters["environmentMap"].SetValue(skybox.skyBoxTexture);
                        }
                        effect.Parameters["UVW"].SetValue(UVW);
                        pass.Apply();
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, part.StartIndex, part.PrimitiveCount);
                    }
                }
            }

            if (helpOn)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(font, "Controls:\nLeft mouse drag = rotate\nRight mouse drag = zoom, Middle mouse drag = move object\n" +
                    "U/V/W = increase the scale of the bump map (+ LShift = decrease)\nM = turn mipmapping on or off", Vector2.UnitX + Vector2.UnitY * 12, Color.White);
                spriteBatch.End();
            }
            if (infoOn)
            {
                string mipString = "";
                if (mipmap)
                {
                    mipString = "on";
                }
                else mipString = "off";
                spriteBatch.Begin();
                spriteBatch.DrawString(font, "Camera Position = " + cameraPosition + "\nLight Position: " + lightPosition + "\nUVW: " + UVW + "\nMipmap: " + mipString
                    , Vector2.UnitX + Vector2.UnitY * 12, Color.White);
                spriteBatch.End();
            }
            if(image)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(texture, new Vector2(150,0), Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
