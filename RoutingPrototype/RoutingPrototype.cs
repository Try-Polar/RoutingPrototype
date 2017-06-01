using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Windows.Threading;

namespace RoutingPrototype
{
    enum Simulation { UK, Boat };
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class RoutingPrototype : Game
    {
        
        Simulation currentSimulation = Simulation.UK;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D podTexture;
        Texture2D formationPodTexture;
        Texture2D destinationTexture;
        Texture2D lineTexture; //a 1x1 texture
        Texture2D cityTexture;
        Texture2D collabCityTexture;
        Texture2D UKBackground;
        Rectangle UKBackgroundRectangle;
        Texture2D boatBackground;
        Rectangle boatBackgroundRectangle;

        FormationManager formationManager;
        PodManager UKpodManager;
        RouteManager UKrouteManager;
        CityManager UKcityManager;
        MetricManager UKmetricManager;
        CityPodManager UKcityPodManager;

        PodManager boatPodManager;
        RouteManager boatRouteManager;
        //CityManager boatCityManager;
        //MetricManager boatMetricManager;
        //CityPodManager boatCityPodManager;

        KeyboardState newState;
        KeyboardState oldState;

        LivePlot plot;  // form displaying plot of metrics
        DispatcherTimer dispatcherTimer;    // timer for plotting data every second

        int SCREEN_WIDTH = 800;
        int SCREEN_HEIGHT = 600;
        int MAP_WIDTH;
        int MAP_HEIGHT;

        //FOR CREATION PURPOSES
        bool mouseJustPressed = false;

        float UKKilometerToPixelMultiplier; //Based on distance from london to bristol
        float UKHourToSecondMultiplier; //1 minute of real time is one day of simulation time
        float UKpixelReference = 212.1909f;
        float UKKilometerReference = 172;
        float boatKilometerToPixelMultiplier;
        float boatHourToSecondMultiplier;
        float boatPixelReference = 203.8553f;
        float boatKilometerReference = 5.3f;

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

            UKKilometerToPixelMultiplier = ((UKpixelReference * ((float)SCREEN_WIDTH / (float)1200)) / UKKilometerReference);
            UKHourToSecondMultiplier = (120 / 24);
            boatKilometerToPixelMultiplier = ((boatPixelReference * ((float)SCREEN_HEIGHT / (float)1200)) / boatKilometerReference);
            boatHourToSecondMultiplier = (1200 / 24);

            this.IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.ApplyChanges();

            plot = new LivePlot();
            plot.Show();
            // Sets up timer to plot every second
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            Console.WriteLine(Vector2.Distance(new Vector2(261, 305), new Vector2(227, 506)));

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
            collabCityTexture = Content.Load<Texture2D>("CollabCity");
            UKBackground = Content.Load<Texture2D>("BackgroundRescaled");
            UKBackgroundRectangle = new Rectangle(0, 0, MAP_WIDTH, MAP_HEIGHT);
            boatBackground = Content.Load<Texture2D>("BoatBackground");
            boatBackgroundRectangle = new Rectangle(0, 0, MAP_WIDTH, MAP_HEIGHT);

            formationManager = new FormationManager(formationPodTexture, new Vector2(SCREEN_WIDTH, SCREEN_HEIGHT));
            UKcityManager = new CityManager(cityTexture, MAP_WIDTH, MAP_HEIGHT);
            UKrouteManager = new RouteManager(destinationTexture, lineTexture, MAP_WIDTH, MAP_HEIGHT, Simulation.UK, UKcityManager);
            UKcityPodManager = new CityPodManager(podTexture, SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(1050, SCREEN_HEIGHT / 2), collabCityTexture);
            UKpodManager = new PodManager(podTexture, destinationTexture, UKrouteManager, UKcityManager.Cities[0].Position, UKKilometerToPixelMultiplier, UKHourToSecondMultiplier, UKcityPodManager);
            UKmetricManager = new MetricManager(UKpodManager, UKpixelReference, UKKilometerReference);
       
            
            //boatCityManager = new CityManager(cityTexture, SCREEN_WIDTH, SCREEN_HEIGHT);
            boatRouteManager = new RouteManager(destinationTexture, lineTexture, SCREEN_WIDTH, SCREEN_HEIGHT, Simulation.Boat);
            //boatCityPodManager = new CityPodManager(podTexture, SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(1050, SCREEN_HEIGHT / 2), collabCityTexture);
            boatPodManager = new PodManager(podTexture, destinationTexture, boatRouteManager, new Vector2(0, SCREEN_HEIGHT / 2), boatKilometerToPixelMultiplier, boatHourToSecondMultiplier);
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
            if (currentSimulation == Simulation.UK)
            {
                UKpodManager.Update(gameTime);
                UKrouteManager.Update(gameTime);
                UKmetricManager.Update(gameTime);
                UKcityPodManager.Update(gameTime);
            }
            if (currentSimulation == Simulation.Boat)
            {
                boatPodManager.Update(gameTime);
                boatRouteManager.Update(gameTime);
                //boatMetricManager.Update(gameTime);
                //boatCityManager.Update(gameTime);
            }

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
            if (newState.IsKeyUp(Keys.D1) && oldState.IsKeyDown(Keys.D1))
            {
                currentSimulation = Simulation.UK;
            }
            if (newState.IsKeyUp(Keys.D2) && oldState.IsKeyDown(Keys.D2))
            {
                currentSimulation = Simulation.Boat;
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
            if (currentSimulation == Simulation.UK)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(UKBackground, UKBackgroundRectangle, Color.White);
                spriteBatch.End();
                UKcityManager.Draw(spriteBatch);
                UKpodManager.Draw(spriteBatch);
                UKrouteManager.Draw(spriteBatch);
                UKcityPodManager.Draw(spriteBatch);
            }
            if (currentSimulation == Simulation.Boat)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(boatBackground, boatBackgroundRectangle, Color.White);
                spriteBatch.End();
                //boatCityManager.Draw(spriteBatch);
                boatPodManager.Draw(spriteBatch);
                boatRouteManager.Draw(spriteBatch);
                //boatCityPodManager.Draw(spriteBatch);
            }

            base.Draw(gameTime);
        }

        private void dispatcherTimer_Tick(object sender, EventArgs eventArgs)
        { 
            float distanceTravelled = UKmetricManager.RealDistanceTravelled;

            // Calculate the cost of the journey by converting to miles and using data from report 
            float nonSkeinCost = (float)(distanceTravelled * 0.621371 * 29.64);     // Using cost of non-skein travelling
            float skeinCost = (float)(distanceTravelled * 0.621371 * 28.88);     // Using cost of skein travelling

            plot.updatePlot(skeinCost, nonSkeinCost);
        }
    }
}
