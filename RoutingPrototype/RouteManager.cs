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
        float mXResolutionScaler;
        float mYResolutionScaler;
        int mMapWidth;

        float mSpawnInterval = 0.05f;
        float mSpawnTimer = 0;

        List<Route> mUnassignedRoutes;
        List<Route> mAssignedRoutes;

        Vector2 mPortLocation;

        Texture2D mMarkerTexture, mLineTexture;

        CityManager cityManager;

        Random rnd;

        Simulation simulation;

        public RouteManager(Texture2D markerText, Texture2D lineText, int screenWidth, int screenHeight, Simulation sim, CityManager cityManager = null)
        {
            mScreenWidth = screenWidth;
            mScreenHeight = screenHeight;
            mXResolutionScaler = screenWidth / 800;
            mYResolutionScaler = screenHeight / 600;
            mMapWidth = (int)(mScreenWidth * 0.75f);
            mMarkerTexture = markerText;
            mLineTexture = lineText;
            rnd = new Random();
            mUnassignedRoutes = new List<Route>();
            mAssignedRoutes = new List<Route>();
            this.cityManager = cityManager;
            mPortLocation = new Vector2(mScreenWidth / 6, screenHeight / 2);
            simulation = sim;

            for (int i = 0; i < 50; i++)
            {
                if (simulation == Simulation.UK)
                {
                    mUnassignedRoutes.Add(generateRealisticRoute());
                }
                if (simulation == Simulation.Boat)
                {
                    mUnassignedRoutes.Add(generateRouteForBoatSim());
                }
            }
            //For testing purposes only---
            //mUnassignedRoutes.Add(new Route(mMarkerTexture, mLineTexture, new Vector2(615, 488), new Vector2(727, 728)));
            //mUnassignedRoutes.Add(new Route(mMarkerTexture, mLineTexture, new Vector2(589, 522), new Vector2(727, 728)));
            //----------------------------
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
        private Route generateRealisticRoute()
        {
            Vector2 pickUpLocation = Vector2.Zero;
            Vector2 dropOffLocation = Vector2.Zero;
            string a = "UNASSIGNED"; //default names to stop errors due to reference to unassigned variables, in practice these are always assigned to before use
            string b = "ALSO UNASSIGNED";
            int value = rnd.Next(0, cityManager.CombinedWeights);
            foreach(City city in cityManager.Cities)
            {
                value -= city.Weighting;

                if (value <= 0)
                {
                    pickUpLocation = city.Position;
                    a = city.Name;
                    break;
                }
            }

            do
            {
                value = rnd.Next(0, cityManager.CombinedWeights);
                foreach (City city in cityManager.Cities)
                {
                    value -= city.Weighting;
                    if (value <= 0)
                    {
                        dropOffLocation = city.Position;
                        b = city.Name;
                        break;
                    }
                }
            } while (a == b);

            return new Route(mMarkerTexture, mLineTexture, pickUpLocation, dropOffLocation);
        }
        
        private Route generateRouteForBoatSim()
        {
            Vector2 pickUpLoc = mPortLocation;
            Vector2 dropOffLoc = Vector2.Zero;

            int y = rnd.Next(0, mScreenHeight);
            int x = 0;
            if (y < 50 * mYResolutionScaler)
            {
                x = rnd.Next((int)(225 * mXResolutionScaler), mMapWidth);
            }
            else if (y >= 50 * mYResolutionScaler && y < 200 * mYResolutionScaler)
            {
                x = rnd.Next((int)(300 * mXResolutionScaler), mMapWidth);
            }
            else if ( y >= 200 * mYResolutionScaler)
            {
                x = rnd.Next((int)(240 * mXResolutionScaler), mMapWidth);
            }
            dropOffLoc.X = x;
            dropOffLoc.Y = y;

            return new Route(mMarkerTexture, mLineTexture, pickUpLoc, dropOffLoc);
        }

        public void Update(GameTime gameTime)
        {
            mSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (mSpawnTimer > mSpawnInterval)
            {
                mSpawnTimer = 0;
                //Spawn a new route
                //mUnassignedRoutes.Add(generateRandomRoute());
                if (mUnassignedRoutes.Count < 20)
                {
                    if (simulation == Simulation.UK)
                    {
                        mUnassignedRoutes.Add(generateRealisticRoute());
                    }
                    if (simulation == Simulation.Boat)
                    {
                        mUnassignedRoutes.Add(generateRouteForBoatSim());
                    }
                }
            }
        }

        public CityManager CityManager
        {
            get { return cityManager; }
        }
        

        public Vector2 PortLocation
        {
            get { return mPortLocation; }
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
