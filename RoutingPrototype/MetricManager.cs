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
        int mJourneysCompleted = 0;

        Vector2 mTopCorner;

        SpriteFont mSpriteFont;

        PodManager mPodManager;

        public MetricManager(PodManager podManager, float pixelReference, float kilometerReference, SpriteFont spriteFont, Vector2 referencePoint)
        {
            mPodManager = podManager;
            mPixelToKilometerConverter = pixelReference / kilometerReference;
            mSpriteFont = spriteFont;
            mTopCorner = referencePoint;
        }

        public void Update(GameTime gameTime)
        {
            mJourneysCompleted = 0;
            foreach (Pod pod in mPodManager.Pods)
            {
                mJourneysCompleted += pod.JourneysCompleted;
                if (pod.newData)
                {
                    float val = (pod.NonSkeinDistanceTravelled - pod.TheoreticalDistanceTravelled) * mPixelToKilometerConverter;
                    if (val > 3.3f)
                        mKilometresSavedBySkeins += (pod.NonSkeinDistanceTravelled - pod.TheoreticalDistanceTravelled) * mPixelToKilometerConverter; //Seems to misjudge a little so -1 essentially corrects this
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
            spritebatch.Begin();
            spritebatch.DrawString(mSpriteFont, "Kilometers worth of fuel \nsaved: " + (int)mKilometresSavedBySkeins, mTopCorner + new Vector2(5, 20), Color.Black);
            spritebatch.DrawString(mSpriteFont, "Journeys completed: " + mJourneysCompleted, mTopCorner + new Vector2(5, 54), Color.Black);
            spritebatch.End();
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

        public int JourneysCompleted
        {
            get { return mJourneysCompleted; }
            set { mJourneysCompleted = value; }
        }
    }
}
