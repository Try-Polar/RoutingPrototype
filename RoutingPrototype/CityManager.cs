using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RoutingPrototype
{
    class CityManager : IUpdateDraw
    {

        List<City> mCities;

        Texture2D cityTexture;

        int mCombinedWeights = 100;

        int mMapWidth, mMapHeight;
        float mWidthScaler, mHeightScaler;

        public CityManager(Texture2D cityText, int mapWidth, int mapHeight)
        {
            mCities = new List<City>();
            cityTexture = cityText;
            mWidthScaler = (float)mapWidth / 900;
            Console.WriteLine(mWidthScaler);
            mHeightScaler = (float)mapHeight / 900;
            Console.WriteLine(mHeightScaler);
            setupCities();
        }

        void setupCities()
        {
            mCities.Add(new City(cityTexture, new Vector2(727 * mWidthScaler, 728 * mHeightScaler), "London", 40));
            mCities.Add(new City(cityTexture, new Vector2(456 * mWidthScaler, 294 * mHeightScaler), "Glasgow", 5));
            mCities.Add(new City(cityTexture, new Vector2(538 * mWidthScaler, 299 * mHeightScaler), "Edinburgh", 10));
            mCities.Add(new City(cityTexture, new Vector2(630 * mWidthScaler, 378 * mHeightScaler), "Newcastle", 5));
            mCities.Add(new City(cityTexture, new Vector2(615 * mWidthScaler, 488 * mHeightScaler), "Leeds", 5));
            //mCities.Add(new City(cityTexture, new Vector2(589 * mWidthScaler, 522 * mHeightScaler), "Manchester", 10));
            mCities.Add(new City(cityTexture, new Vector2(568 * mWidthScaler, 554 * mHeightScaler), "Liverpool", 5));
            mCities.Add(new City(cityTexture, new Vector2(610 * mWidthScaler, 636 * mHeightScaler), "Birmingham", 10));
            mCities.Add(new City(cityTexture, new Vector2(529 * mWidthScaler, 727 * mHeightScaler), "Cardiff", 5));
            mCities.Add(new City(cityTexture, new Vector2(559 * mWidthScaler, 749 * mHeightScaler), "Bristol", 5));
            mCities.Add(new City(cityTexture, new Vector2(350 * mWidthScaler, 546 * mHeightScaler), "Dublin", 5));
            mCities.Add(new City(cityTexture, new Vector2(827 * mWidthScaler, 518 * mHeightScaler), "FloatingPort", 5));

            //mCities.Add(new City(cityTexture, new Vector2(500 * mWidthScaler, 700 * mHeightScaler), "TestA", 10));
            //mCities.Add(new City(cityTexture, new Vector2(500 * mWidthScaler, 500 * mHeightScaler), "TestB", 10));

            mCombinedWeights = getCombinedWeights();
        }

        int getCombinedWeights()
        {
            int total = 0;
            foreach (City city in mCities)
            {
                total += city.Weighting;
            }
            return total;
        }

        public List<City> Cities
        {
            get { return mCities; }
        }

        public int CombinedWeights
        {
            get { return mCombinedWeights; }
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (City city in mCities)
            {
                city.Draw(spriteBatch);
            }
        }


    }
}
