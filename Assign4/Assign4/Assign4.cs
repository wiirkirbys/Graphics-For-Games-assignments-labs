using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using SimpleEngine;

namespace Assign4
{
    public class Assign4 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // **** TEMPLATE *****
        SpriteFont font;
        Effect effect;
        Texture2D texture , NA, fire, water, smoke;
        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), Vector3.UnitY);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 600f, 0.1f, 100f);
        Vector3 cameraPosition, cameraTarget, lightPosition;
        float angle, angle2, angleL, angleL2, distance;
        MouseState preMouse;
        KeyboardState preKeyboard;
        Matrix invertCamera;
        ParticleManager particleManager;
        Vector3 particlePosition;
        System.Random random;
        Model model;
        bool randomDirection, gravity;
        Vector3 wind, heldWind; //acceleration due to wind
        int emitShape, behaviorState;
        float startVelocity, heldStartVelocity, resilience, heldResilience, friction, heldFriction, lifespan;
        bool infoOn, helpOn;

        public Assign4()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content"; IsMouseVisible = true;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // *** TEMPLATE ***
            font = Content.Load<SpriteFont>("font");
            effect = Content.Load<Effect>("ParticleShader");
            NA = null;
            fire = Content.Load<Texture2D>("fire");
            water = Content.Load<Texture2D>("water");
            smoke = Content.Load<Texture2D>("smoke");
            texture = NA;
            model = Content.Load<Model>("Plane");
            angle = angle2 = 0;
            distance = 30;
            random = new System.Random();
            particleManager = new ParticleManager(GraphicsDevice, 120);
            particlePosition = new Vector3(0, 5, 0); // spawns above surface
            randomDirection = false;
            gravity = false;
            wind = heldWind = new Vector3(0, 0, 0);//no wind
            emitShape = behaviorState = 0;
            startVelocity = heldStartVelocity = 3f;
            resilience = friction = 0;
            heldResilience = heldFriction = .8f;
            infoOn = helpOn = false;
            lifespan = 5f;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            // *** TEMPLATE ***
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
                distance += (Mouse.GetState().Y - preMouse.Y) / 100f;
            }
            if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
            {
                Vector3 ViewRight = Vector3.Transform(Vector3.UnitX,
                    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                Vector3 ViewUp = Vector3.Transform(Vector3.UnitY,
                    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                cameraTarget -= ViewRight * (Mouse.GetState().X - preMouse.X) / 100f;
                cameraTarget += ViewUp * (Mouse.GetState().Y - preMouse.Y) / 100f;
            }

            // Update Camera
            cameraPosition = Vector3.Transform(new Vector3(0, 0, distance),
                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle) * Matrix.CreateTranslation(cameraTarget));
            view = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Transform(Vector3.UnitY,
                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle)));
            // Update Light
            lightPosition = Vector3.Transform(new Vector3(0, 0, 10), Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL));

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                angle = angle2 = 0;
                distance = 10;
                cameraPosition = Vector3.Zero;
                cameraTarget = Vector3.Zero;
            }

            // *** Assignment stuff ***
            // note:...apparently I was supposed to put most of this stuff in Particle and Particle manager, so... whoops?

            //generates particles
            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                Particle particle = particleManager.getNext();

                //emmiter shapes
                if (emitShape == 0)//point
                {
                    particle.Position = particlePosition; //generates from exact point
                }
                else if (emitShape == 1)//square
                {
                    if (random.Next(0, 2) == 1)
                    {
                        if (random.Next(0, 2) == 1)
                        {
                            particle.Position = particlePosition + new Vector3(5, 0, random.Next(-5, 5)); //generates in a 5x5 square
                        }
                        else
                        {
                            particle.Position = particlePosition + new Vector3(-5, 0, random.Next(-5, 5));
                        }
                    }
                    else
                    {
                        if (random.Next(0, 2) == 1)
                        {
                            particle.Position = particlePosition + new Vector3(random.Next(-5, 5), 0, 5);
                        }
                        else
                        {
                            particle.Position = particlePosition + new Vector3(random.Next(-5, 5), 0, -5);
                        }
                    }
                }
                else if (emitShape == 2)//curve
                {
                    particle.Position = particlePosition + new Vector3(0, (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds)*5, 0); //particle position moves along a curve
                }
                else if (emitShape == 3)//ring
                {
                    particle.Position = particlePosition + 5*Vector3.Normalize(new Vector3(random.Next(-5, 5), 0, random.Next(-5, 5))); //generates in a circle radius 5
                }

                //particle movement
                if (randomDirection)
                {
                    particle.Velocity = Vector3.Normalize(new Vector3(random.Next(-5, 5), 2, random.Next(-5, 5))) * startVelocity;
                }
                else
                { 
                    particle.Velocity = new Vector3(0, startVelocity, 0);//straight up (or down)
                }
                if (gravity)
                {
                    particle.Acceleration = new Vector3(0, -5, 0) + wind; //gravity
                }
                else
                {
                    particle.Acceleration = new Vector3(0, 0, 0) + wind;
                }
                particle.resilience = resilience;
                particle.friction = friction;
                particle.MaxAge = lifespan;
                particle.Init();
            }

            //changes particle texture
            if (Keyboard.GetState().IsKeyDown(Keys.D1))
            {
                texture = NA;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D2))
            {
                texture = smoke;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D3))
            {
                texture = water;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D4))
            {
                texture = fire;
            }

            //particle behaviors
            if (Keyboard.GetState().IsKeyDown(Keys.F1)) //fountain basic
            {
                behaviorState = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F2))
            {
                behaviorState = 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F3))
            {
                behaviorState = 2;
                
            }
            if(behaviorState == 0)
            {
                randomDirection = false;
                gravity = false;
                wind = Vector3.Zero;
                startVelocity = 5;
                resilience = friction = 0;
            }
            if(behaviorState == 1)
            {
                randomDirection = true;
                gravity = true;
                wind = Vector3.Zero;
                startVelocity = 5;
                resilience = friction = 0;
            }
            if (behaviorState == 2)
            {
                randomDirection = true;
                gravity = true;
                wind = heldWind;
                startVelocity = heldStartVelocity;
                resilience = heldResilience;
                friction = heldFriction;
            }

                if (Keyboard.GetState().IsKeyDown(Keys.F4) && !preKeyboard.IsKeyDown(Keys.F4))
            {
                emitShape++;
                if(emitShape >= 4)
                {
                    emitShape = 0;
                }
                particlePosition = new Vector3(0,5,0); //resets for curve
            }

                //particle parameter modifications
                if (Keyboard.GetState().IsKeyDown(Keys.R)) //resilience
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    heldResilience -= .01f;
                }
                else
                {
                    heldResilience += .01f;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F)) //friction
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    heldFriction -= .01f;
                }
                else
                {
                    heldFriction += .01f;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.V)) //starting particle velocity
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    heldStartVelocity -= .1f;
                }
                else
                {
                    heldStartVelocity += .1f;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W)) //wind, only modifiable in z direction
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    heldWind.X -= .1f;
                }
                else
                {
                    heldWind.X += .1f;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.L)) //wind, only modifiable in z direction
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    lifespan-= .1f;
                }
                else
                {
                    lifespan += .1f;
                }
            }

            //particle updates (for hitting the floor) are in Particle.cs
            if (Keyboard.GetState().IsKeyDown(Keys.OemQuestion) && !preKeyboard.IsKeyDown(Keys.OemQuestion)) helpOn = !helpOn;
            if (Keyboard.GetState().IsKeyDown(Keys.H) && !preKeyboard.IsKeyDown(Keys.H)) infoOn = !infoOn;


            particleManager.Update(gameTime.ElapsedGameTime.Milliseconds * 0.001f);

            invertCamera = Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle);

            preMouse = Mouse.GetState();
            preKeyboard = Keyboard.GetState();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (texture != null)
            {
                GraphicsDevice.BlendState = BlendState.AlphaBlend;
            }
            else
            {
                GraphicsDevice.BlendState = BlendState.Opaque;
            }


            model.Draw(world, view, projection);

            // *** Step2: Culling OFF
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            // *** Step3: Apply Particle Shader
            effect.CurrentTechnique = effect.Techniques["particle"];
            effect.CurrentTechnique.Passes[0].Apply();

            effect.Parameters["ViewProj"].SetValue(view * projection);
            effect.Parameters["World"].SetValue(Matrix.Identity);
            effect.Parameters["CamIRot"].SetValue(invertCamera);
            if (texture != null)
            {
                effect.Parameters["Texture"].SetValue(texture);
            }

            particleManager.Draw(GraphicsDevice);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            if (helpOn)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(font, "Controls:\nLeft mouse drag = rotate\nRight mouse drag = zoom\nMiddle mouse drag = move object\n" +
                    "H: show all data used in the system\n" +
                    "1-4 = change particle texture\nF1-F3 = change particle behavior\nF4 = change emmiter shape\n--CONTROLS BELOW ONLY FOR BEHAVIOR F3--\n" +
                    "F = increase friction (+ lShift = decrease)\n R = increase resilience (+ lShift = decrease)\nW = increase wind in Z direction (+ lShift = decrease)\n" +
                    "V = increase starting particle velocity (+ lShift = decrease)\nL = increase particle lifespan (+ lShift = decrease)"
                    , Vector2.UnitX + Vector2.UnitY * 12, Color.White);
                spriteBatch.End();
            }
            if (infoOn)
            {
                string emitShapeString = "";
                switch(emitShape)
                {
                    case 0: emitShapeString = "Point"; break;
                    case 1: emitShapeString = "Square"; break;
                    case 2: emitShapeString = "Curve"; break;
                    case 3: emitShapeString = "Circle"; break;
                }
                spriteBatch.Begin();
                spriteBatch.DrawString(font, "Camera Position = " + cameraPosition + "\nLight Position: " + lightPosition + "\nEmit Shape: " + emitShapeString +
                    "\n--DATA BELOW ONLY FOR BEHAVIOR F3--\nWind: " + heldWind + "\nStarting velocity: " + heldStartVelocity + "\nFriction: " + heldFriction +
                    "\nResilience: " + heldResilience + "\nParticle Lifespan: " + lifespan
                    , Vector2.UnitX + Vector2.UnitY * 12, Color.White) ;
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

    }
}
