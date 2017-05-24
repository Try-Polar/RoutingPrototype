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
        int mEvenIndex = 0;
        int mOddIndex = 1;

        List<FormationPod> mExitingPods;

        Vector2 mLeaderPosition;

        Random rnd;
        int exitRange = 200;

        public FormationManager(Texture2D texture, Vector2 screenDimensions)
        {
            mCurrentSkein = null;
            mFormationPodTexture = texture;
            mFormationPods = new FormationPod[11];
            mFreeIndices = new List<int>();
            mExitingPods = new List<FormationPod>();
            mScreenDimensions = screenDimensions;
            rnd = new Random();
            mLeaderPosition = new Vector2((screenDimensions.X * 0.125f) + (screenDimensions.X * 0.75f), 25);

            /*//TESTING PURPOSES ONLY-----------------
            mFormationPods[nextIndex] = new FormationPod(mFormationPodTexture, mScreenDimensions, mLeaderPosition, nextIndex);
            nextIndex++;
            count++;
            mFormationPods[nextIndex] = new FormationPod(mFormationPodTexture, new Vector2(800,20), mLeaderPosition, nextIndex);
            nextIndex++;
            count++;
            mFormationPods[nextIndex] = new FormationPod(mFormationPodTexture, new Vector2(800, 200), mLeaderPosition, nextIndex);
            nextIndex++;
            count++;
            //--------------------------------------*/
        }

        //Add new pod to the formation (this one should be easy, just spawn a new pod offscreen and have it move into place)
        public void addPod()
        {
            if (count < mFormationPods.Length)
            {
                int formationIndex;
                if (mEvenIndex > mOddIndex)
                {
                    formationIndex = mOddIndex;
                    mOddIndex += 2;
                }
                else
                {
                    formationIndex = mEvenIndex;
                    mEvenIndex += 2;
                }
                if (mFreeIndices.Count == 0)
                {
                    mFormationPods[nextIndex] = new FormationPod(mFormationPodTexture, generateEntranceLocation(), mLeaderPosition, formationIndex, rnd.Next());
                    nextIndex++;
                }
                else
                {
                    int index = mFreeIndices.First();
                    mFreeIndices.RemoveAt(0);
                    mFormationPods[index] = new FormationPod(mFormationPodTexture, generateEntranceLocation(), mLeaderPosition, formationIndex, rnd.Next());
                }
                Console.WriteLine("Add");
                Console.WriteLine("Even: " + mEvenIndex);
                Console.WriteLine("Odd: " + mOddIndex);
                count++;
            }
        }

        //Remove a randomly chosen pod from the formation (this might be slightly more difficult since I can't just make the pod disapear I've gotta make it move out of formation and offscreen
        // and then I have to make the others reform, but that should be easy  I'll just readjust the formation indices)
        public void removePod()
        {
            if (count > 0)
            {
                int index = 0;
                do
                {
                    index = rnd.Next(0, mFormationPods.Length);
                } while (mFormationPods[index] == null);
                mFormationPods[index].setExiting(generateExitLocation());
                mExitingPods.Add(mFormationPods[index]);

                int formationIndex = mFormationPods[index].FormationIndex;
                
                mFreeIndices.Add(index);
                mFormationPods[index] = null;
                adjustIndices(formationIndex);


                Console.WriteLine("Remove");
                Console.WriteLine("Even: " + mEvenIndex);
                Console.WriteLine("Odd: " + mOddIndex);
                count--;
            }
        }

        Vector2 generateExitLocation()
        {
            Vector2 result;
            result.X = rnd.Next((int)((mScreenDimensions.X * 0.125f) + (mScreenDimensions.X * 0.75f) - exitRange), (int)((mScreenDimensions.X * 0.125f) + (mScreenDimensions.X * 0.75f) + exitRange));
            result.Y = -50;

            return result;
        }

        Vector2 generateEntranceLocation()
        {
            Vector2 result;
            result.X = rnd.Next((int)((mScreenDimensions.X * 0.125f) + (mScreenDimensions.X * 0.75f) - exitRange), (int)((mScreenDimensions.X * 0.125f) + (mScreenDimensions.X * 0.75f) + exitRange));
            result.Y = 350;

            return result;
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
                        if (mFormationPods[i].FormationIndex % 2 == 1)
                        {
                            if (mFormationPods[i].FormationIndex > removedIndex)
                            {
                                mFormationPods[i].changeFormationIndex(-2);
                            }
                        }
                    }
                    else
                    {//Even
                        if (mFormationPods[i].FormationIndex % 2 == 0)
                        {
                            if (mFormationPods[i].FormationIndex > removedIndex)
                            {
                                mFormationPods[i].changeFormationIndex(-2);
                            }
                        }
                    }
                }
            }
            setHighestIndices();
        }

        void setHighestIndices()
        {
            mEvenIndex = 0;
            mOddIndex = 1;
            for (int i = 0; i < mFormationPods.Length; i++)
            {
                if (mFormationPods[i] != null)
                {
                    if (mFormationPods[i].FormationIndex % 2 == 1)
                    {
                        if (mFormationPods[i].FormationIndex >= mOddIndex)
                        {
                            mOddIndex = mFormationPods[i].FormationIndex + 2;
                        }
                    }
                    else
                    {
                        if (mFormationPods[i].FormationIndex >= mEvenIndex)
                        {
                            mEvenIndex = mFormationPods[i].FormationIndex + 2;
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
            for (int i = mExitingPods.Count - 1; i >= 0; i--)
            {
                if (mExitingPods[i].Position.Y > -20)
                {
                    mExitingPods[i].Update(gameTime);
                }
                else
                {
                    mExitingPods.RemoveAt(i);
                }
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
