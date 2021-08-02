using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SimpleEngine;

namespace Assign2
{
    public class Assign2 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Vector4 ambientColor = new Vector4(0, 0, 0, 0);
        Vector3 lightPosition = new Vector3(1, 1, 1);
        MouseState previousMouseState;
        KeyboardState previousKeyboardState;
        Vector4 specularColor = new Vector4(1, 1, 1, 1);
        //float specularIntensity;
        float reflectivity = 0.99f;
        Vector3 etaRatio = new Vector3(.9f, .9f, .9f);
        float fresnelPower = 0f;
        float fresnelScale = 0f;
        float fresnelBias = 0f;
        float shininess = 20.0f;
        Vector3 cameraPosition;
        Model model;
        Effect effect;
        Matrix world, view, projection;
        Vector4 ambient = new Vector4(0, 0, 0, 0);
        //float ambientIntensity = 0.1f;
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
        int currentShader = 0;
        int currentSky = 1;
        bool helpOn = false;
        bool infoOn = false;
        Skybox skybox;

        public Assign2()
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
            string[] skyboxTextures = { "skybox/test_negx", "skybox/test_posx", "skybox/test_negy", "skybox/test_posy", "skybox/test_negz", "skybox/test_posz"};
            skybox = new Skybox(skyboxTextures, Content, graphics.GraphicsDevice);
            effect = Content.Load<Effect>("shader");
            model = Content.Load<Model>("box");
            font = Content.Load<SpriteFont>("Font");
            world = Matrix.Identity;
            view = Matrix.CreateLookAt(new Vector3(camX, camY, camDistance), Vector3.Zero, Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), GraphicsDevice.Viewport.AspectRatio, 0.1f, 100f);
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
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
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


            bool changed = false;
            bool skyChanged = false;
            //models
            if (Keyboard.GetState().IsKeyDown(Keys.D1) && currentModel != 1)
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
            if (Keyboard.GetState().IsKeyDown(Keys.D6) && currentModel != 6)
            {
                changed = true;
                currentModel = 6;
            }
            //skyboxes
            if (Keyboard.GetState().IsKeyDown(Keys.D7) && currentModel != 7)
            {
                skyChanged = true;
                currentSky = 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D8) && currentModel != 8)
            {
                skyChanged = true;
                currentSky = 2;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D9) && currentModel != 9)
            {
                skyChanged = true;
                currentSky = 3;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D0) && currentModel != 0)
            {
                skyChanged = true;
                currentSky = 4;
            }
            //textures
            /*if (Keyboard.GetState().IsKeyDown(Keys.F1) && currentShader != 0)
            {
                changed = true;
                currentShader = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F2) && currentShader != 1)
            {
                changed = true;
                currentShader = 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F3) && currentShader != 2)
            {
                changed = true;
                currentShader = 2;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F4) && currentShader != 3)
            {
                changed = true;
                currentShader = 3;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F5) && currentShader != 4)
            {
                changed = true;
                currentShader = 4;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F6) && currentShader != 5)
            {
                changed = true;
                currentShader = 5;
            }*/
            if (Keyboard.GetState().IsKeyDown(Keys.F7) && currentShader != 0)
            {
                changed = true;
                currentShader = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F8) && currentShader != 1)
            {
                changed = true;
                currentShader = 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F9) && currentShader != 2)
            {
                changed = true;
                currentShader = 2;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F10) && currentShader != 3)
            {
                changed = true;
                currentShader = 3;
            }

            //changes model
            if (changed)
            {
                //Content.Unload();
                switch (currentModel)
                {
                    case 1: model = Content.Load<Model>("box"); break;
                    case 2: model = Content.Load<Model>("sphere"); break;
                    case 3: model = Content.Load<Model>("Torus"); break;
                    case 4: model = Content.Load<Model>("teapot"); break;
                    case 5: model = Content.Load<Model>("bunnyUV"); break;
                    case 6: model = Content.Load<Model>("Helicopter"); Content.Load<Texture2D>("HelicopterTexture");  break;
                }
                effect = Content.Load<Effect>("shader");
                font = Content.Load<SpriteFont>("Font");
            }
            //changes skybox
            if (skyChanged)
            {
                string[] skyboxTextures;
                switch (currentSky)
                {
                    case 1:
                        skyboxTextures = new string[] {"skybox/test_negx", "skybox/test_posx", "skybox/test_negy", "skybox/test_posy", "skybox/test_negz", "skybox/test_posz"};
                        skybox = new Skybox(skyboxTextures, Content, graphics.GraphicsDevice); break;
                    case 2:
                        skyboxTextures = new string[] { "skybox/nvlobby_new_negx", "skybox/nvlobby_new_posx", "skybox/nvlobby_new_negy", "skybox/nvlobby_new_posy", "skybox/nvlobby_new_negz", "skybox/nvlobby_new_posz" };
                        skybox = new Skybox(skyboxTextures, Content, graphics.GraphicsDevice); break;
                    case 3:
                        skyboxTextures = new string[] { "skybox/hills_negx", "skybox/hills_posx", "skybox/hills_negy", "skybox/hills_posy", "skybox/hills_negz", "skybox/hills_posz" };
                        skybox = new Skybox(skyboxTextures, Content, graphics.GraphicsDevice); break;
                    case 4:
                        skyboxTextures = new string[] { "skybox/dice_negx", "skybox/dice_posx", "skybox/dice_negy", "skybox/dice_posy", "skybox/dice_negz", "skybox/dice_posz" };
                        skybox = new Skybox(skyboxTextures, Content, graphics.GraphicsDevice); break;
                }
            }

            //changes light
            /*if (Keyboard.GetState().IsKeyDown(Keys.L))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.RightShift) || Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    ambientIntensity -= .1f;
                }
                else
                {
                    ambientIntensity += .1f;
                }
            }*/
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.RightShift) || Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    etaRatio.X -= .01f;
                }
                else
                {
                    etaRatio.X += .01f;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.G))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.RightShift) || Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    etaRatio.Y -= .01f;
                }
                else
                {
                    etaRatio.Y += .01f;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.B))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.RightShift) || Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    etaRatio.Z -= .01f;
                }
                else
                {
                    etaRatio.Z += .01f;
                }
            }
            //Fresnel stats
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.RightShift) || Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    fresnelPower -= .01f;
                }
                else
                {
                    fresnelPower += .01f;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.RightShift) || Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    fresnelScale -= .01f;
                }
                else
                {
                    fresnelScale += .01f;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.RightShift) || Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    fresnelBias -= .01f;
                }
                else
                {
                    fresnelBias += .01f;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                reflectivity += .1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                reflectivity -= .1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemQuestion) && !lastKeyDown.IsKeyDown(Keys.OemQuestion))
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

            RasterizerState originalRasterizerState = graphics.GraphicsDevice.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphics.GraphicsDevice.RasterizerState = rasterizerState;
            skybox.Draw(view, projection, cameraPosition);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            effect.CurrentTechnique = effect.Techniques[currentShader]; // uses current shader
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                        /*effect.Parameters["AmbientColor"].SetValue(ambientColor);
                        effect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);
                        effect.Parameters["DiffuseColor"].SetValue(diffuseColor);
                        effect.Parameters["DiffuseIntensity"].SetValue(diffuseIntensity);
                        effect.Parameters["LightPosition"].SetValue(lightPosition);
                        effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                        effect.Parameters["SpecularColor"].SetValue(specularColor);
                        effect.Parameters["SpecularIntensity"].SetValue(specularIntensity);
                        effect.Parameters["Shininess"].SetValue(shininess);*/
                        effect.Parameters["environmentMap"].SetValue(skybox.skyBoxTexture);
                        effect.Parameters["reflectivity"].SetValue(reflectivity);
                        effect.Parameters["fresnelPower"].SetValue(fresnelPower);
                        effect.Parameters["fresnelScale"].SetValue(fresnelScale);
                        effect.Parameters["fresnelBias"].SetValue(fresnelBias);
                        effect.Parameters["etaRatio"].SetValue(etaRatio);

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
                    "R = increase etaRatio of red (+ Shift = decrease)\nG = increase etaRatio of green (+ Shift = decrease)\n" +
                    "B = increase etaRatio of blue (+ Shift = decrease)\nQ = increase Fresnel Power (+ Shift = decrease)\nW = increase Fresnel Scale (+ Shift = decrease)\n" +
                    "E = increase Fresnel Bias (+ Shift = decrease)\n? = display help\nH = display info", Vector2.UnitX + Vector2.UnitY * 12, Color.White);
                spriteBatch.End();
            }
            if (infoOn)
            {
                string shaderName = "";
                switch (currentShader)
                {
                    /*case 0: shaderName = "Goraud"; break;
                    case 1: shaderName = "Phong"; break;
                    case 2: shaderName = "PhongBlinn"; break;
                    case 3: shaderName = "Schlick"; break;
                    case 4: shaderName = "Toon"; break;
                    case 5: shaderName = "HalfLife"; break;*/
                    case 0: shaderName = "Reflection"; break;
                    case 1: shaderName = "Refraction"; break;
                    case 2: shaderName = "Refraction & Dispersion"; break;
                    case 3: shaderName = "Fresnel"; break;
                }
                spriteBatch.Begin();
                spriteBatch.DrawString(font, "Camera Position = " + cameraPosition + "\nLight Position: " + lightPosition + "\nShader Type: " + shaderName /*+ \nIntensity: "
                    + ambientIntensity + "\nSpecular: " + specularIntensity + "\nShininess: " + shininess + "\nRGB: " + ambientColor*/ + "\nReflectivity: " + reflectivity +
                    "\netaRatio: " + etaRatio + "\n Fresnel Scale: " + fresnelScale + "\nFresnel Power: " + fresnelPower + "\nFresnel Bias: " + fresnelBias
                    , Vector2.UnitX + Vector2.UnitY * 12, Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
