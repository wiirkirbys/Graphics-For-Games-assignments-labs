﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalProject
{
    public class FinalProject : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Effect effect;
        Model model;
        SpriteFont font;
        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 30), new Vector3(0, 0, 0), Vector3.UnitY);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 600f, 0.1f, 100f);
        Vector3 cameraPosition, cameraTarget, lightPosition;
        float angle, angle2, angleL, angleL2, distance;
        MouseState preMouse;
        KeyboardState preKeyboardState;
        RenderTarget2D renderTarget;
        Texture2D depthAndNormalMap;
        VertexPositionTexture[] vertices = { new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0)), new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1)), new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)), new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0)), new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0)), new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)) };
        bool drawEdges;
        float offset = .003f, depthDiff = .00005f, normalDiff = .8f;
        bool helpOn = false;

        public FinalProject()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content"; IsMouseVisible = true;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            drawEdges = false;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            model = Content.Load<Model>("objects");
            font = Content.Load<SpriteFont>("Font");
            _resetView();
            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            effect = Content.Load<Effect>("DepthAndNormal");
            renderTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.S)) _resetView();
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) angleL += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) angleL -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) angleL2 += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) angleL2 -= 0.02f;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                angle -= (Mouse.GetState().X - preMouse.X) / 100f;
                angle2 += (Mouse.GetState().Y - preMouse.Y) / 100f;
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                distance += (Mouse.GetState().X - preMouse.X) / 100f;
            }
            if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
            {
                Vector3 ViewRight = Vector3.Transform(Vector3.UnitX,
                    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                Vector3 ViewUp = Vector3.Transform(Vector3.UnitY,
                    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                cameraTarget -= ViewRight * (Mouse.GetState().X - preMouse.X) / 10f;
                cameraTarget += ViewUp * (Mouse.GetState().Y - preMouse.Y) / 10f;
            }
            preMouse = Mouse.GetState();
            // Update Camera
            cameraPosition = Vector3.Transform(new Vector3(0, 0, distance),
                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
            view = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Transform(Vector3.UnitY,
                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle)));
            // Update Light
            lightPosition = Vector3.Transform(new Vector3(0, 0, 10), Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL));
            // Update LightMatrix
            //Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL)));

            if (Keyboard.GetState().IsKeyDown(Keys.E) && !preKeyboardState.IsKeyDown(Keys.E)) drawEdges = !drawEdges;
            if (Keyboard.GetState().IsKeyDown(Keys.OemQuestion) && !preKeyboardState.IsKeyDown(Keys.OemQuestion)) helpOn = !helpOn;
            if (Keyboard.GetState().IsKeyDown(Keys.O))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    offset -= .0001f;
                }
                else
                {
                    offset += .0001f;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.N))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    normalDiff -= .005f;
                }
                else
                {
                    normalDiff += .005f;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    depthDiff *= .8f;
                }
                else
                {
                    depthDiff *= 1.2f;
                }
            }

            preKeyboardState = Keyboard.GetState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            DrawDepthAndNormalMap();
            GraphicsDevice.SetRenderTarget(null);
            depthAndNormalMap = (Texture2D)renderTarget;
            // This block will be used later for Deferred Shading (SSAO)
            if (drawEdges)
            {
                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
                DrawEdge();
            }
            else
            {
                using (SpriteBatch sprite = new SpriteBatch(GraphicsDevice))
                {
                    sprite.Begin();
                    sprite.Draw(depthAndNormalMap, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    sprite.End();
                }
            }
            depthAndNormalMap = null;
            if(helpOn)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(font, "E = toggle edge detection\nD = increase min depth differnce (+ LShift = decrease)\n(depth difference = " + depthDiff + ")" +
                    "\nN = normal = increase min normal difference (+ LShift = decrease)\n(normal difference = " + normalDiff + ")" +
                    "\nO = increase sampler offset (+LShift = decrease)\n(offset = " + offset + ")", Vector2.UnitX + Vector2.UnitY * 12, Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        private void DrawDepthAndNormalMap()
        {
            effect = Content.Load<Effect>("DepthAndNormal");
            effect.CurrentTechnique = effect.Techniques[0];
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
                        pass.Apply();
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, part.StartIndex, part.PrimitiveCount);
                    }
                }
            }
        }
        private void _resetView()
        {
            angle = angle2 = 0;
            angleL = angleL2 = 0;// MathHelper.ToRadians(45);
            distance = 30f;
            cameraTarget = Vector3.Zero;
        }
        private void DrawEdge()
        {
            effect = Content.Load<Effect>("EdgeShader");
            effect.CurrentTechnique = effect.Techniques[0];
            effect.Parameters["DepthAndNormalTexture"].SetValue(depthAndNormalMap);
            effect.Parameters["offset"].SetValue(offset);
            effect.Parameters["depthDiff"].SetValue(depthDiff);
            effect.Parameters["normalDiff"].SetValue(normalDiff);
            effect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3);
        }
    }
}
