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

        public CityManager(Texture2D cityText)
        {
            mCities = new List<City>();
            cityTexture = cityText;
            setupCities();
        }

        void setupCities()
        {
            mCities.Add(new City(cityTexture, new Vector2(727, 728), "London", 40));
            mCities.Add(new City(cityTexture, new Vector2(456, 294), "Glasgow", 5));
            mCities.Add(new City(cityTexture, new Vector2(538, 299), "Edinburgh", 10));
            mCities.Add(new City(cityTexture, new Vector2(630, 378), "Newcastle", 5));
            mCities.Add(new City(cityTexture, new Vector2(615, 488), "Leeds", 5));
            mCities.Add(new City(cityTexture, new Vector2(589, 522), "Manchester", 10));
            mCities.Add(new City(cityTexture, new Vector2(568, 554), "Liverpool", 5));
            mCities.Add(new City(cityTexture, new Vector2(610, 636), "Birmingham", 10));
            mCities.Add(new City(cityTexture, new Vector2(529, 727), "Cardiff", 5));
            mCities.Add(new City(cityTexture, new Vector2(559, 749), "Bristol", 5));

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
