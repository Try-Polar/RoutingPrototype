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

        public Vector2 getCurrentVector()
        {
            Vector2 currentVector = Vector2.Zero;
            foreach (Pod pod in mMembers)
            {
                currentVector += pod.CurrentVector;
            }

            return (currentVector / mMembers.Count);
        }

        public void remove(Pod pod)
        {
            mMembers.Remove(pod);
            if (pod == mLeader)
            {
                if (mMembers.Count > 2)
                {
                    //assign new leader
                    mLeader = mMembers.First();
                    mLeader.isLeader = true;
                }
                else if (mMembers.Count == 1)
                {
                    mMembers.First().skeinDispersed();
                }
            }
        }

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
            Vector2 center = Vector2.Zero;
            foreach (Pod pod in mMembers)
            {
                center += pod.Position;
            }
            return center / mMembers.Count;
        }
    }
}
