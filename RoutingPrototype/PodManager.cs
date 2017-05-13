using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RoutingPrototype
{
    class PodManager : IUpdateDraw
    {
        List<Pod> mPods;
        Texture2D podTexture;
        Texture2D destinationTexture;
        Texture2D lineTexture;

        int initialNumberOfPods = 25;

        int sWidth;
        int sHeight;
        Random rnd = new Random();

        public PodManager(Texture2D podText, Texture2D destinationText, Texture2D lineText, int screenWidth, int screenHeight)
        {
            podTexture = podText;
            destinationTexture = destinationText;
            lineTexture = lineText;

            sWidth = screenWidth;
            sHeight = screenHeight;

            mPods = new List<Pod>();

            //establish some number of pods
            for (int i = 0; i < initialNumberOfPods; i++)
            {
                mPods.Add(new Pod(podTexture, destinationTexture, lineTexture, Vector2.Zero, sWidth, sHeight, rnd.Next()));
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (Pod pod in mPods)
            {
                pod.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Pod pod in mPods)
            {
                pod.Draw(spriteBatch);
            }
        }



    }
}
