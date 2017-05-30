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
        float mKilometresSavedBySkeins = 0;
        float mKilometresNonSkeinTravelled = 0;
        float mKilometresSkeinTravelled = 0;
        float mPixelToKilometerConverter;

        PodManager mPodManager;

        public MetricManager(PodManager podManager, float pixelReference, float kilometerReference)
        {
            mPodManager = podManager;
            mPixelToKilometerConverter = pixelReference / kilometerReference;
        }

        public void Update(GameTime gameTime)
        {
            
            foreach (Pod pod in mPodManager.Pods)
            {
                if (pod.newData)
                {
                    mKilometresSavedBySkeins += (pod.NonSkeinDistanceTravelled - pod.ActualDistanceTravelled - 1.5f) * mPixelToKilometerConverter; //Seems to misjudge a little so -1 essentially corrects this
                    mKilometresNonSkeinTravelled += (pod.NonSkeinDistanceTravelled - 1.5f) * mPixelToKilometerConverter;
                    mKilometresSkeinTravelled += (pod.ActualDistanceTravelled - 1.5f) * mPixelToKilometerConverter;
                    pod.newData = false;
                }
            }
            //Console.WriteLine(kilometersSavedBySkeins);
        }

        public void Draw(SpriteBatch spritebatch)
        {

        }

        public float KilometresSavedBySkeins
        {
            get { return mKilometresSavedBySkeins; }
        }

        public float NonSkeinKilometresTravelled
        {
            get { return mKilometresNonSkeinTravelled; }
        }

        public float SkeinKilometresTravelled
        {
            get { return mKilometresSkeinTravelled; }
        }
    }
}
