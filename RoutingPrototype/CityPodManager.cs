using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RoutingPrototype
{
    class CityPodManager : IUpdateDraw
    {
        List<CityPod> mPods;
        Texture2D mPodTexture;
        int mScreenWidth;
        CollaboratingCity mCity;

        public CityPodManager(Texture2D podTexture, int screenWidth, Vector2 cityPosition, Texture2D cityTexture )
        {
            mPods = new List<CityPod>();
            mPodTexture = podTexture;
            mScreenWidth = screenWidth;
            mCity = new CollaboratingCity(cityTexture, cityPosition);
            //TESTING ONLY
            mPods.Add(new CityPod(mPodTexture, new Vector2(780, 400), mScreenWidth, mCity.Position, new Vector2(1, 1), 10, -5));
        }

        public void Update(GameTime gameTime)
        {
            foreach (CityPod pod in mPods)
            {
                pod.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            mCity.Draw(spriteBatch);
            foreach (CityPod pod in mPods)
            {
                pod.Draw(spriteBatch);
            }
        }
    }
}
