using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RoutingPrototype
{
    class MetricManager : IUpdateDraw
    {
        float kilometersSavedBySkeins = 0;

        PodManager mPodManager;

        public MetricManager(PodManager podManager)
        {
            mPodManager = podManager;
        }

        public void Update(GameTime gameTime)
        {
            
            foreach (Pod pod in mPodManager.Pods)
            {
                if (pod.newData)
                {
                    kilometersSavedBySkeins += (pod.NonSkeinDistanceTravelled - pod.ActualDistanceTravelled - 1.5f); //Seems to misjudge a little so -1 essentially corrects this
                    pod.newData = false;
                }
            }
            Console.WriteLine(kilometersSavedBySkeins);
        }

        public void Draw(SpriteBatch spritebatch)
        {

        }

    }
}
