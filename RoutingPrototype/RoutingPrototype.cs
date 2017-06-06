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
        Texture2D skyBackground;
        Rectangle skyBackgroundRectangle;
        Texture2D cityPodTexture;
        Texture2D backgroundPane;

        SpriteFont metricFont;

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
            boatHourToSecondMultiplier = (1800 / 24);

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

            podTexture = Content.Load<Texture2D>("LiliumJetRescaled");
            formationPodTexture = Content.Load<Texture2D>("LiliumJetRescaled");
            destinationTexture = Content.Load<Texture2D>("Destination");
            lineTexture = Content.Load<Texture2D>("Line");
            cityTexture = Content.Load<Texture2D>("City");
            collabCityTexture = Content.Load<Texture2D>("CollabCity");
            UKBackground = Content.Load<Texture2D>("BackgroundRescaled");
            UKBackgroundRectangle = new Rectangle(0, 0, MAP_WIDTH, MAP_HEIGHT);
            boatBackground = Content.Load<Texture2D>("BoatBackground");
            boatBackgroundRectangle = new Rectangle(0, 0, MAP_WIDTH, MAP_HEIGHT);
            skyBackground = Content.Load<Texture2D>("SkyBackground");
            skyBackgroundRectangle = new Rectangle((int)((float)SCREEN_WIDTH * 0.75f), (int)((float)SCREEN_HEIGHT / 3), 1000, 1000);
            cityPodTexture = Content.Load<Texture2D>("PodWithoutWingsRecaled"); //I know this is spelt wrong I typo'ed on the image itself and its easier to just leave it
            backgroundPane = Content.Load<Texture2D>("BackgroundPane");

            metricFont = Content.Load<SpriteFont>("Metric");

            formationManager = new FormationManager(formationPodTexture, new Vector2(SCREEN_WIDTH, SCREEN_HEIGHT));
            UKcityManager = new CityManager(cityTexture, MAP_WIDTH, MAP_HEIGHT);
            UKrouteManager = new RouteManager(destinationTexture, lineTexture, MAP_WIDTH, MAP_HEIGHT, Simulation.UK, UKcityManager);
            UKcityPodManager = new CityPodManager(podTexture, SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2((SCREEN_WIDTH * 7) / 8, SCREEN_HEIGHT / 2), collabCityTexture, cityPodTexture);
            UKpodManager = new PodManager(podTexture, destinationTexture, UKrouteManager, Vector2.Zero, UKKilometerToPixelMultiplier, UKHourToSecondMultiplier, UKcityPodManager, UKcityManager);
            UKmetricManager = new MetricManager(UKpodManager, UKpixelReference, UKKilometerReference, metricFont, new Vector2(SCREEN_WIDTH * 0.75f, SCREEN_HEIGHT * 2 / 3));
       
            
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
            if (newState.IsKeyUp(Keys.Space) && oldState.IsKeyDown(Keys.Space))
            {
                UKpodManager.clearJourneysCompleted();
                UKmetricManager.resetMetrics();
                if (UKpodManager.CreatingSkeins)
                    UKpodManager.CreatingSkeins = false;
                else
                    UKpodManager.CreatingSkeins = true;
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
            GraphicsDevice.Clear(Color.LightSkyBlue);

            formationManager.Draw(spriteBatch);
            spriteBatch.Begin();
            spriteBatch.Draw(skyBackground, skyBackgroundRectangle, Color.White);
            spriteBatch.End();
            if (currentSimulation == Simulation.UK)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(UKBackground, UKBackgroundRectangle, Color.White);
                spriteBatch.End();
                UKcityManager.Draw(spriteBatch);
                UKpodManager.Draw(spriteBatch);
                UKrouteManager.Draw(spriteBatch);
                UKcityPodManager.Draw(spriteBatch);
                UKmetricManager.Draw(spriteBatch);
            }
            if (currentSimulation == Simulation.Boat)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(backgroundPane, new Rectangle((int)((float)SCREEN_WIDTH * 0.75f), 0, SCREEN_WIDTH, SCREEN_HEIGHT), Color.White);
                spriteBatch.Draw(boatBackground, boatBackgroundRectangle, Color.White);
                spriteBatch.Draw(cityTexture, new Rectangle((int)boatRouteManager.PortLocation.X, (int)boatRouteManager.PortLocation.Y, cityTexture.Width, cityTexture.Height), Color.White);
                spriteBatch.End();
                //boatCityManager.Draw(spriteBatch);
                boatRouteManager.Draw(spriteBatch);
                boatPodManager.Draw(spriteBatch);
                //boatCityPodManager.Draw(spriteBatch);
            }
            

            base.Draw(gameTime);
        }

        private void dispatcherTimer_Tick(object sender, EventArgs eventArgs)
        { 
            float distanceTravelled = UKmetricManager.RealDistanceTravelled;

            // Calculate the cost of the journey by converting to miles and using data from report 
            float nonSkeinCost = (float)(distanceTravelled * 0.621371 * 29.64);     // Using cost of non-skein travelling
            float worstSkeinCost = (float)(distanceTravelled * 0.621371 * 28.88);     // Using cost of skein travelling (worst case)
            float bestSkeinCost = (float)(distanceTravelled * 0.621371 * 27.80);     // Using cost of skein travelling (best case)
            plot.updatePlot(UKpodManager.CreatingSkeins, worstSkeinCost, bestSkeinCost, nonSkeinCost);
        }
    }
}
