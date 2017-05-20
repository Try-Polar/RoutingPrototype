using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RoutingPrototype
{
    class FormationManager
    {
        Skein mCurrentSkein;

        Texture2D mFormationPodTexture;

        FormationPod[] mFormationPods; //could use a list but I I feel limiting the size of the skein shown (so that it can fit in the area I have assigned to it) would be beneficial
                                       //I'm also going to use it to do work something similar to a data array into the function of the formation manager
        List<int> mFreeIndices;

        public FormationManager(Texture2D texture)
        {
            mCurrentSkein = null;
            mFormationPodTexture = texture;
            mFormationPods = new FormationPod[10];
            mFreeIndices = new List<int>();
        }

        //Add new pod to the formation (this one should be easy, just spawn a new pod offscreen and have it move into place)
        public void addPod()
        {

        }

        //Remove a randomly chosen pod from the formation (this might be slightly more difficult since I can't just make the pod disapear I've gotta make it move out of formation and offscreen
        // and then I have to make the others reform, but that should be easy  I'll just readjust the formation indices)
        public void removePod()
        {

        }



    }
}
