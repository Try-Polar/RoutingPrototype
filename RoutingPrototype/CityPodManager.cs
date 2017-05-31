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
        int mScreenHeight;
        int mLeftBoundary;
        int mUpperBoundary;
        int mLowerBoundary;
        Vector2 mCityPosition;
        CollaboratingCity mCity;
        int mCityRadius;
        Random rnd;

        public CityPodManager(Texture2D podTexture, int screenWidth, int screenHeight, Vector2 cityPosition, Texture2D cityTexture )
        {
            mPods = new List<CityPod>();
            mCityPosition = cityPosition;
            mPodTexture = podTexture;
            mScreenWidth = screenWidth;
            mScreenHeight = screenHeight;
            mCityRadius = (mScreenWidth / 12);
            mCity = new CollaboratingCity(cityTexture, cityPosition, mCityRadius);
            mLeftBoundary = (int)(0.75f * mScreenWidth);
            mUpperBoundary = (int)(0.3333f * mScreenHeight);
            mLowerBoundary = (int)(0.6666f * mScreenHeight);
            rnd = new Random();
        }

        public void AddPod(Pod pod)
        {
            mPods.Add(new CityPod(mPodTexture, new Vector2(mLeftBoundary + 10, mUpperBoundary + 10), mScreenWidth, mCityPosition, new Vector2(rnd.Next(-5, 5), rnd.Next(-5, 5)), rnd.Next(-5,5), rnd.Next(-5, 5), pod, mCityRadius));
        }

        public void Update(GameTime gameTime)
        {
            for (int i = mPods.Count-1; i > -1; i--)
            {
                if (inCityArea(mPods[i].Position))
                {
                    mPods[i].Update(gameTime);
                }
                else
                {
                    mPods[i].Pod.leftCity();
                    mPods.RemoveAt(i);
                }
            }
        }

        bool inCityArea(Vector2 pos)
        {
            if (pos.X > mLeftBoundary && pos.X < mScreenWidth)
            {
                if (pos.Y > mUpperBoundary && pos.Y < mLowerBoundary)
                {
                    return true;
                }
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            mCity.Draw(spriteBatch);
            foreach (CityPod cityPod in mPods)
            {
                if (cityPod.Pod.InLondon == true && inCityArea(cityPod.Position))
                {
                    cityPod.Draw(spriteBatch);
                }
            }
        }
    }
}
