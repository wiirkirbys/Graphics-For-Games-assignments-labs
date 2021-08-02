using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab3
{
    public class Assign1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Vector4 ambientColor = new Vector4(0, 0, 0, 0);
        Vector3 lightPosition = new Vector3(1, 1, 1);
        MouseState previousMouseState;
        KeyboardState previousKeyboardState;
        Vector4 specularColor = new Vector4(1, 1, 1, 1);
        float specularIntensity = 1.0f;
        float shininess = 20.0f;
        Vector3 cameraPosition;
        Model model;
        Effect effect;
        Matrix world, view, projection;
        Vector4 ambient = new Vector4(0, 0, 0, 0);
        float ambientIntensity = 0.1f;
        Vector4 diffuseColor = new Vector4(1, 1, 1, 1);
        Vector3 diffuseLightDirection = new Vector3(1, 1, 1);
        float lightX = 1;
        float lightY = 1;
        float diffuseIntensity = 1.0f;
        MouseState lastMouseState;
        KeyboardState lastKeyDown;
        float angle = 0;
        float angle2 = 0;
        float camDistance = 10f;
        float camX = 0;
        float camY = 0;
        int currentModel = 1;
        int currentTexture = 0;
        bool helpOn = false;
        bool infoOn = false;

        public Assign1()
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
            effect = Content.Load<Effect>("Texture");
            model = Content.Load<Model>("box");
            font = Content.Load<SpriteFont>("Font");
            world = Matrix.Identity;
            view = Matrix.CreateLookAt(new Vector3(camX, camY, camDistance), Vector3.Zero, Vector3.Up );
            projection = Matrix.CreatePerspectiveFieldOfView( MathHelper.ToRadians(90), GraphicsDevice.Viewport.AspectRatio, 0.1f, 100f);
        }

        protected override void UnloadContent()
        {
            
        }
        protected override void Update(GameTime gameTime)
        {
            MouseState currentMouseState = Mouse.GetState();

            //rotate if left dragged
            if (currentMouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Pressed)
            {
                angle -= (lastMouseState.X - currentMouseState.X) / 100f;
                angle2 -= (lastMouseState.Y - currentMouseState.Y) / 100f;
            }

            //zoom if right dragged
            if (currentMouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Pressed)
            {
                camDistance -= (lastMouseState.Y - currentMouseState.Y) / 100f;
            }

            //translate if middle dragged
            if (currentMouseState.MiddleButton == ButtonState.Pressed && lastMouseState.MiddleButton == ButtonState.Pressed)
            {
                camX -= (lastMouseState.X - currentMouseState.X) / 100f;
                camY += (lastMouseState.Y - currentMouseState.Y) / 100f;
            }

            //move the light
            if(Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                lightY += .1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                lightY -= .1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                lightX += .1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                lightX -= .1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                lightX = 1;
                lightY = 1;
                camDistance = 10f;
                camX = 0;
                camY = 0;
                angle = 0;
                angle2 = 0;
            }

            lightPosition.X = lightX;
            lightPosition.Y = lightY;
            view = Matrix.CreateRotationY(angle) * Matrix.CreateRotationX(angle2) * Matrix.CreateTranslation(new Vector3(camX, camY, -camDistance));
            cameraPosition = new Vector3(camX, camY, camDistance);
            lastMouseState = currentMouseState;

            //Swtich models
            bool changed = false;
            if(Keyboard.GetState().IsKeyDown(Keys.D1) && currentModel != 1)
            {
                changed = true;
                currentModel = 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D2) && currentModel != 2)
            {
                changed = true;
                currentModel = 2;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D3) && currentModel != 3)
            {
                changed = true;
                currentModel = 3;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D4) && currentModel != 4)
            {
                changed = true;
                currentModel = 4;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D5) && currentModel != 5)
            {
                changed = true;
                currentModel = 5;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F1) && currentTexture != 0)
            {
                changed = true;
                currentTexture = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F2) && currentTexture != 1)
            {
                changed = true;
                currentTexture = 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F3) && currentTexture != 2)
            {
                changed = true;
                currentTexture = 2;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F4) && currentTexture != 3)
            {
                changed = true;
                currentTexture = 3;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F5) && currentTexture != 4)
            {
                changed = true;
                currentTexture = 4;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F6) && currentTexture != 5)
            {
                changed = true;
                currentTexture = 5;
            }

            if (changed)
            {
                Content.Unload();
                switch(currentModel)
                {
                    case 1: model = Content.Load<Model>("box"); break;
                    case 2: model = Content.Load<Model>("sphere"); break;
                    case 3: model = Content.Load<Model>("Torus"); break;
                    case 4: model = Content.Load<Model>("teapot"); break;
                    case 5: model = Content.Load<Model>("bunny"); break;
                }
                effect = Content.Load<Effect>("Texture");
                font = Content.Load<SpriteFont>("Font");
            }

            //changes light
            if(Keyboard.GetState().IsKeyDown(Keys.L))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.RightShift) || Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    ambientIntensity -= .1f;
                }
                else
                {
                    ambientIntensity += .1f;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.RightShift) || Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    ambientColor.X -= .1f;
                }
                else
                {
                    ambientColor.X += .1f;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.G))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.RightShift) || Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    ambientColor.Y -= .1f;
                }
                else
                {
                    ambientColor.Y += .1f;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.B))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.RightShift) || Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    ambientColor.Z -= .1f;
                }
                else
                {
                    ambientColor.Z += .1f;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                if ( Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                {
                    shininess += .1f;
                }
                else
                {
                    specularIntensity += .1f;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                {
                    shininess -= .1f;
                }
                else
                {
                    specularIntensity -= .1f;
                }
            }
            if(Keyboard.GetState().IsKeyDown(Keys.OemQuestion) && !lastKeyDown.IsKeyDown(Keys.OemQuestion))
            {
                helpOn = !helpOn;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.H) && !lastKeyDown.IsKeyDown(Keys.H))
            {
                infoOn = !infoOn;
            }
            lastKeyDown = Keyboard.GetState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            effect.CurrentTechnique = effect.Techniques[currentTexture]; // uses current shader
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        Matrix worldInverseTransposeMatrix = Matrix.Transpose(
                            Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                        effect.Parameters["AmbientColor"].SetValue(ambientColor);
                        effect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);

                        effect.Parameters["DiffuseColor"].SetValue(diffuseColor);
                        effect.Parameters["DiffuseIntensity"].SetValue(diffuseIntensity);

                        effect.Parameters["LightPosition"].SetValue(lightPosition);
                        effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                        effect.Parameters["SpecularColor"].SetValue(specularColor);
                        effect.Parameters["SpecularIntensity"].SetValue(specularIntensity);
                        effect.Parameters["Shininess"].SetValue(shininess);

                        pass.Apply(); // send the data to GPU
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;

                        GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            part.VertexOffset,
                            part.StartIndex,
                            part.PrimitiveCount);
                    }
                }
            }

            if (helpOn)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(font, "Controls:\nLeft mouse drag = rotate\nRight mouse drag = zoom, Middle mouse drag = move object\n" +
                    "L = increase light intensity (+ Shift = decrease)\nR = increase red value of light (+ Shift = decrease)\n" +
                    "G = increase green value of light (+ Shift = decrease)\nB = increase blue value of light (+ Shift = decrease)\n" +
                    "+ = increase specular intensity (+ LCtrl = increase shininess)\n- = decrease specular intensity (+ LCtrl = decrease shininess)\n" +
                    "? = display help\nH = display info", Vector2.UnitX + Vector2.UnitY * 12, Color.White);
                spriteBatch.End();
            }
            if (infoOn)
            {
                string shaderName = "";
                switch(currentTexture)
                {
                    case 0:  shaderName = "Goraud"; break;
                    case 1:  shaderName = "Phong"; break;
                    case 2:  shaderName = "PhongBlinn"; break;
                    case 3:  shaderName = "Schlick"; break;
                    case 4:  shaderName = "Toon"; break;
                    case 5:  shaderName = "HalfLife"; break;
                }
                spriteBatch.Begin();
                spriteBatch.DrawString(font, "Camera Position = " + cameraPosition + "\nLight Position: " + lightPosition + "\nShader Type: " + shaderName + "\nIntensity: "
                    + ambientIntensity + "\nSpecular: " + specularIntensity + "\nShininess: " + shininess + "\nRGB: " + ambientColor
                    , Vector2.UnitX + Vector2.UnitY * 12, Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
