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
            mCities.Add(new City(cityTexture, new Vector2(711 * mWidthScaler, 691 * mHeightScaler), "London", 50));
            mCities.Add(new City(cityTexture, new Vector2(346 * mWidthScaler, 152 * mHeightScaler), "Glasgow", 5));
            mCities.Add(new City(cityTexture, new Vector2(449 * mWidthScaler, 124 * mHeightScaler), "Edinburgh", 10));
            mCities.Add(new City(cityTexture, new Vector2(586 * mWidthScaler, 266 * mHeightScaler), "Newcastle", 5));
            mCities.Add(new City(cityTexture, new Vector2(590 * mWidthScaler, 387 * mHeightScaler), "Leeds", 5));
            mCities.Add(new City(cityTexture, new Vector2(549 * mWidthScaler, 437 * mHeightScaler), "Manchester", 10));
            mCities.Add(new City(cityTexture, new Vector2(490 * mWidthScaler, 469 * mHeightScaler), "Liverpool", 5));
            mCities.Add(new City(cityTexture, new Vector2(559 * mWidthScaler, 541 * mHeightScaler), "Birmingham", 10));
            mCities.Add(new City(cityTexture, new Vector2(434 * mWidthScaler, 690 * mHeightScaler), "Cardiff", 5));
            mCities.Add(new City(cityTexture, new Vector2(499 * mWidthScaler, 700 * mHeightScaler), "Bristol", 5));
            mCities.Add(new City(cityTexture, new Vector2(161 * mWidthScaler, 367 * mHeightScaler), "Dublin", 5));
            mCities.Add(new City(cityTexture, new Vector2(789 * mWidthScaler, 344 * mHeightScaler), "FloatingPort", 5));

            //Console.WriteLine((new Vector2(711, 691) - new Vector2(499, 700)).Length());

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

        public Vector2 findLondon()
        {
            foreach (City city in mCities)
            {
                if (city.Name == "London")
                {
                    return city.Position;
                }
            }
            return Vector2.Zero;
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
