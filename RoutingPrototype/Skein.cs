using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RoutingPrototype
{
    class Skein
    {
        List<Pod> mMembers;

        Pod mLeader;

        public Skein(Pod leader, Pod other)
        {
            mMembers = new List<Pod>();
            mLeader = leader;
            mMembers.Add(leader);
            mMembers.Add(other);
        }

        /*public Vector2 getCurrentVector()
        {

        }*/

        public List<Pod> Members
        {
            get { return mMembers; }
        }

        public Pod Leader
        {
            get { return mLeader; } 
            set { mLeader = value; }
        }

        public Vector2 getCurrentCenter()
        {
            int i = 0;
            Vector2 center = Vector2.Zero;
            foreach (Pod pod in mMembers)
            {
                i++;
                center += pod.Position;
            }
            return center / i;
        }
    }
}
