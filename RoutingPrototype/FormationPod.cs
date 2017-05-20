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
        Vector2 mLeaderPostion;
        Vector2 mOffset;
        int mFormationIndex;
        float mOffsetSpacing;
        float mMass = 0.05f;
        float VELOCITY = 50;

        public FormationPod(Texture2D texture, Vector2 initialPosition, int formationIndex) : base(texture, initialPosition)
        {
            mFormationIndex = formationIndex;
            mOffset = calculateOffset();
        }

        public void Update(GameTime gameTime)
        {
            Vector2 acceleration = arrive(mOffset) / mMass;
            Velocity += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Velocity.Length() > maxVelocity)
            {
                Velocity *= maxVelocity / Velocity.Length();
            }

            Position = Position + (Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds) * VELOCITY;
        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }

        void changeFormationIndex(int newIndex)
        {
            mFormationIndex = newIndex;
            mOffset = calculateOffset();
        }

        Vector2 calculateOffset()
        {
            Vector2 offset = new Vector2(1, 1);
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

    }
}
