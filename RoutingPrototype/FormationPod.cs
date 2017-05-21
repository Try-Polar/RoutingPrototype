using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RoutingPrototype
{
    class FormationPod : MovingEntity, IUpdateDraw
    {
        Vector2 mLeaderPosition;
        Vector2 mOffset;
        int mFormationIndex;
        float mOffsetSpacing = 50;
        float mMass = 0.05f;
        float VELOCITY = 100;

        bool mIsExiting = false;

        public FormationPod(Texture2D texture, Vector2 initialPosition, Vector2 leaderPosition, int formationIndex) : base(texture, initialPosition)
        {
            mFormationIndex = formationIndex;
            mLeaderPosition = leaderPosition;
            mOffset = mLeaderPosition + calculateOffset();
            maxVelocity = 1;
        }

        public void Update(GameTime gameTime)
        {
            Vector2 acceleration = Vector2.Zero;
            if (!isExiting)
            {
                acceleration = arrive(mOffset) / mMass;
            }
            else
            {
                //Decide some place offscreen to exit too, then Seek to that destination
            }
            Velocity += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Velocity.Length() > maxVelocity)
            {
                Velocity *= maxVelocity / Velocity.Length();
            }

            Position = Position + (Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds) * VELOCITY;
            this.updateRectanglePos();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(this.Texture, this.Rect, Color.White);
            spriteBatch.End();
        }

        void changeFormationIndex(int newIndex)
        {
            mFormationIndex = newIndex;
            mOffset = mLeaderPosition + calculateOffset();
        }

        Vector2 calculateOffset()
        {
            Vector2 offset = new Vector2(1, 1) * mOffsetSpacing;
            //if index is 0 then pod is leader and so does not have any offset
            if (mFormationIndex == 0)
            {
                return Vector2.Zero;
            }
            else
            {
                int side = 1;
                int offsetMultiplier = 0;
                if ((mFormationIndex % 2) == 1) //if index is odd then offset will be put to the left else it will be on the right
                {
                    side *= -1;
                    offsetMultiplier = (mFormationIndex / 2) + 1; //Pretty sure C# will always round down, so I'm adding 1 (I want 1/2 to be 1 and 3/2 to be 2 etc)
                }
                else
                {
                    offsetMultiplier = mFormationIndex / 2;
                }
                offset.X *= side;
                offset *= offsetMultiplier;
            }
            return offset;
        }

        public int FormationIndex
        {
            get { return mFormationIndex; }
            set { mFormationIndex = value; }
        }

        public bool isExiting
        {
            get { return mIsExiting; }
            set { mIsExiting = value; }
        }

    }
}
