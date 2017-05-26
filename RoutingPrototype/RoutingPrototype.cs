using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RoutingPrototype
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class RoutingPrototype : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D podTexture;
        Texture2D formationPodTexture;
        Texture2D destinationTexture;
        Texture2D lineTexture; //a 1x1 texture
        Texture2D cityTexture;
        Texture2D background;
        Rectangle backgroundRectangle;

        FormationManager formationManager;
        PodManager podManager;
        RouteManager routeManager;
        CityManager cityManager;
        MetricManager metricManager;
        CityPodManager cityPodManager;

        KeyboardState newState;
        KeyboardState oldState;

        int SCREEN_WIDTH = 1200;
        int SCREEN_HEIGHT = 900;
        int MAP_WIDTH;
        int MAP_HEIGHT;

        //FOR CREATION PURPOSES
        bool mouseJustPressed = false;

        float kilometerToPixelMultiplier; //Based on distance from london to bristol
        float hourToSecondMultiplier; //1 minute of real time is one day of simulation time
        float UKpixelReference = 212.1909f;
        float UKKilometerReference = 172;

        public RoutingPrototype()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            MAP_WIDTH = (int)(0.75f * SCREEN_WIDTH);
            MAP_HEIGHT = SCREEN_HEIGHT;

            kilometerToPixelMultiplier = (float)((UKpixelReference * (SCREEN_WIDTH / 1200)) / UKKilometerReference);
            hourToSecondMultiplier = (120 / 24);

            this.IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.ApplyChanges();
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

            podTexture = Content.Load<Texture2D>("Pod");
            formationPodTexture = Content.Load<Texture2D>("FormationPod");
            destinationTexture = Content.Load<Texture2D>("Destination");
            lineTexture = Content.Load<Texture2D>("Line");
            cityTexture = Content.Load<Texture2D>("City");
            background = Content.Load<Texture2D>("BackgroundRescaled");
            backgroundRectangle = new Rectangle(0, 0, MAP_WIDTH, MAP_HEIGHT);

            formationManager = new FormationManager(formationPodTexture, new Vector2(SCREEN_WIDTH, SCREEN_HEIGHT));
            cityManager = new CityManager(cityTexture, MAP_WIDTH, MAP_HEIGHT);
            routeManager = new RouteManager(destinationTexture, lineTexture, cityManager, MAP_WIDTH, MAP_HEIGHT);
            cityPodManager = new CityPodManager(podTexture, SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(1050, SCREEN_HEIGHT / 2), cityTexture);
            podManager = new PodManager(podTexture, destinationTexture, routeManager, cityManager.Cities[0].Position, kilometerToPixelMultiplier, hourToSecondMultiplier, cityPodManager);
            metricManager = new MetricManager(podManager, UKpixelReference, UKKilometerReference);
            //cityPodManager = new CityPodManager(podTexture, SCREEN_WIDTH, new Vector2(SCREEN_WIDTH * (7 / 8), SCREEN_HEIGHT / 2), cityTexture);
            

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
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
            formationManager.Update(gameTime);
            podManager.Update(gameTime);
            routeManager.Update(gameTime);
            metricManager.Update(gameTime);
            cityPodManager.Update(gameTime);

            //FOR THE PURPOSES OF SETTING UP CITIES ONLY
            MouseState mouseState = Mouse.GetState();
            newState = Keyboard.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                mouseJustPressed = true;
            }
            if (mouseState.LeftButton == ButtonState.Released && mouseJustPressed)
            {
                mouseJustPressed = false;
                Vector2 mousePosition;
                mousePosition.X = mouseState.X;
                mousePosition.Y = mouseState.Y;
                Console.WriteLine(mousePosition);
            }

            if (newState.IsKeyUp(Keys.P) && oldState.IsKeyDown(Keys.P))
            {
                formationManager.addPod();
            }
            if (newState.IsKeyUp(Keys.O) && oldState.IsKeyDown(Keys.O))
            {
                formationManager.removePod();
            }


            oldState = newState;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            formationManager.Draw(spriteBatch);
            
            spriteBatch.Begin();
            spriteBatch.Draw(background, backgroundRectangle, Color.White);
            spriteBatch.End();
            cityManager.Draw(spriteBatch);
            podManager.Draw(spriteBatch);
            routeManager.Draw(spriteBatch);
            cityPodManager.Draw(spriteBatch);


            base.Draw(gameTime);
        }
    }
}