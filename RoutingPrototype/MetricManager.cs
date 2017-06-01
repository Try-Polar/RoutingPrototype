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
        float mKilometresNonSkeinTravelled = 0; // The actual distance travelled not in a skein
        float mKilometresSkeinTravelled = 0;    // The theoretical (smaller) distance travelled in skeins
        float mKilometresTravelled = 0;         // The total actual distance travelled (regardless of in skein or not)

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
                    mKilometresSavedBySkeins += (pod.NonSkeinDistanceTravelled - pod.TheoreticalDistanceTravelled - 1.5f) * mPixelToKilometerConverter; //Seems to misjudge a little so -1 essentially corrects this
                    mKilometresNonSkeinTravelled += (pod.NonSkeinDistanceTravelled - 1.5f) * mPixelToKilometerConverter;
                    mKilometresSkeinTravelled += (pod.TheoreticalDistanceTravelled - 1.5f) * mPixelToKilometerConverter;
                    mKilometresTravelled += (pod.RealDistanceTravelled - 1.5f) * mPixelToKilometerConverter;

                    pod.newData = false;
                }
            }
            //Console.WriteLine(mKilometresSavedBySkeins);
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

        public float RealDistanceTravelled
        {
            get { return mKilometresTravelled; }
        }
    }
}
