using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RoutingPrototype
{
    class RouteManager : IUpdateDraw
    {
        int mScreenWidth;
        int mScreenHeight;

        float mSpawnInterval = 0.12f;
        float mSpawnTimer = 0;

        List<Route> mUnassignedRoutes;
        List<Route> mAssignedRoutes;

        Texture2D mMarkerTexture, mLineTexture;

        Random rnd;

        public RouteManager(Texture2D markerText, Texture2D lineText, int screenWidth, int screenHeight)
        {
            mScreenWidth = screenWidth;
            mScreenHeight = screenHeight;
            mMarkerTexture = markerText;
            mLineTexture = lineText;
            rnd = new Random();
            mUnassignedRoutes = new List<Route>();
            mAssignedRoutes = new List<Route>();
        }

        public List<Route> UnassignedRoutes
        {
            get { return mUnassignedRoutes; }
        }

        public List<Route> AssignedRoutes
        {
            get { return mAssignedRoutes; }
        }

        private Route generateRandomRoute()
        {
            //generate pick up
            Vector2 pickUplocation;

            pickUplocation.X = rnd.Next(0, mScreenWidth);
            pickUplocation.Y = rnd.Next(0, mScreenHeight);

            Vector2 dropOfflocation;

            //generate Drop off
            dropOfflocation.X = rnd.Next(0, mScreenWidth);
            dropOfflocation.Y = rnd.Next(0, mScreenHeight);
            

            Route route = new Route(mMarkerTexture, mLineTexture, pickUplocation, dropOfflocation);

            return route;
        }

        //Generate a route based on locations of city objects
        private void generateRealisticRoute()
        {

        }

        public void Update(GameTime gameTime)
        {
            mSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (mSpawnTimer > mSpawnInterval)
            {
                mSpawnTimer = 0;
                //Spawn a new route
                mUnassignedRoutes.Add(generateRandomRoute());
            }
        }

        //This will most likely be redundant but oh well
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(Route route in mUnassignedRoutes)
            {
                route.Draw(spriteBatch);
            }
            foreach(Route route in mAssignedRoutes)
            {
                route.Draw(spriteBatch);
            }
        }
    }
}
