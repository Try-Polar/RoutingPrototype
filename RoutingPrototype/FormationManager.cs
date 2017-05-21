using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RoutingPrototype
{
    class FormationManager : IUpdateDraw
    {
        Skein mCurrentSkein;

        Texture2D mFormationPodTexture;

        FormationPod[] mFormationPods; //could use a list but I feel useing something similar to a data array will work better
        List<int> mFreeIndices;
        int nextIndex = 0;
        Vector2 mScreenDimensions;
        int count = 0;

        List<FormationPod> mExitingPods;

        Vector2 mLeaderPosition;

        Random rnd;

        public FormationManager(Texture2D texture, Vector2 screenDimensions)
        {
            mCurrentSkein = null;
            mFormationPodTexture = texture;
            mFormationPods = new FormationPod[10];
            mFreeIndices = new List<int>();
            mExitingPods = new List<FormationPod>();
            mScreenDimensions = screenDimensions;
            rnd = new Random();
            mLeaderPosition = new Vector2((screenDimensions.X * 0.125f) + (screenDimensions.X * 0.75f), screenDimensions.Y * 0.25f);

            //TESTING PURPOSES ONLY-----------------
            mFormationPods[nextIndex] = new FormationPod(mFormationPodTexture, mScreenDimensions, mLeaderPosition, nextIndex);
            nextIndex++;
            count++;
            mFormationPods[nextIndex] = new FormationPod(mFormationPodTexture, new Vector2(800,20), mLeaderPosition, nextIndex);
            nextIndex++;
            count++;
            mFormationPods[nextIndex] = new FormationPod(mFormationPodTexture, new Vector2(800, 200), mLeaderPosition, nextIndex);
            nextIndex++;
            count++;
            //--------------------------------------
        }

        //Add new pod to the formation (this one should be easy, just spawn a new pod offscreen and have it move into place)
        public void addPod()
        {
            if (count < 10)
            {
                if (mFreeIndices.Count == 0)
                {
                    mFormationPods[nextIndex] = new FormationPod(mFormationPodTexture, mScreenDimensions, mLeaderPosition, nextIndex);
                    nextIndex++;
                }
                else
                {
                    int index = mFreeIndices.First();
                    mFreeIndices.RemoveAt(0);
                    mFormationPods[index] = new FormationPod(mFormationPodTexture, mScreenDimensions, mLeaderPosition, index);
                }
                count++;
            }
        }

        //Remove a randomly chosen pod from the formation (this might be slightly more difficult since I can't just make the pod disapear I've gotta make it move out of formation and offscreen
        // and then I have to make the others reform, but that should be easy  I'll just readjust the formation indices)
        public void removePod()
        {
            if (count > 0)
            {
                int index = rnd.Next(0, count);
                //mFormationPods[index].setExiting();
                mExitingPods.Add(mFormationPods[index]);

                mFreeIndices.Add(index);
                mFormationPods[index] = null;

                //Adjust formation indices of remain pods
                adjustIndices(index);

                count--;
            }
        }

        //Adjust the remaining indices, if odd was removed move all later odd indices down by two
        void adjustIndices(int removedIndex)
        {         
            for (int i = 0; i < mFormationPods.Length; i ++)
            {
                if ( mFormationPods[i] != null)
                {
                    if (removedIndex % 2 == 1)
                    {//Odd
                        if (mFormationPods[i].FormationIndex % 2 == 1 && mFormationPods[i].FormationIndex > removedIndex)
                        {
                            mFormationPods[i].FormationIndex -= 2;
                        }
                    }
                    else
                    {//Even
                        if (mFormationPods[i].FormationIndex % 2 == 0 && mFormationPods[i].FormationIndex > removedIndex)
                        {
                            mFormationPods[i].FormationIndex -= 2;
                        }
                    }
                }
            }


        }

        public void Update(GameTime gameTime)
        {
            for (int i=0; i < mFormationPods.Length; i++)
            {
                if (mFormationPods[i] != null)
                {
                    mFormationPods[i].Update(gameTime);
                }
            }

            foreach (FormationPod fPod in mExitingPods)
            {
                fPod.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < mFormationPods.Length; i++)
            {
                if (mFormationPods[i] != null)
                {
                    mFormationPods[i].Draw(spriteBatch);
                }
            }

            foreach (FormationPod fPod in mExitingPods)
            {
                fPod.Draw(spriteBatch);
            }
        }
    }
}
