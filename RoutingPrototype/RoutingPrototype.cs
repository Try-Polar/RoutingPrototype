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
        Texture2D destinationTexture;
        Texture2D lineTexture; //a 1x1 texture

        PodManager podManager;
        RouteManager routeManager;

        int SCREEN_WIDTH = 800;
        int SCREEN_HEIGHT = 600;

        

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
            destinationTexture = Content.Load<Texture2D>("Destination");
            lineTexture = Content.Load<Texture2D>("Line");

            routeManager = new RouteManager(destinationTexture, lineTexture, SCREEN_WIDTH, SCREEN_HEIGHT);
            podManager = new PodManager(podTexture, destinationTexture, lineTexture, routeManager, SCREEN_WIDTH, SCREEN_HEIGHT);

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

            podManager.Update(gameTime);
            routeManager.Update(gameTime);
            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            podManager.Draw(spriteBatch);
            routeManager.Draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}