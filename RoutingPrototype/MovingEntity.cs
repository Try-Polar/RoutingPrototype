using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RoutingPrototype
{
    abstract class MovingEntity : BaseEntity
    {
        Vector2 mVelocity;
        float mMaxVelocity;

        public MovingEntity(Texture2D texture, Vector2 initialPosition) : base(texture, initialPosition)
        {
            this.mVelocity = Vector2.Zero;
        }

        public Vector2 Velocity
        {
            get { return mVelocity; }
            set { mVelocity = value; }
        }

        public float maxVelocity
        {
            get { return mMaxVelocity; }
            set { mMaxVelocity = value; }
        }



        protected Vector2 seek(Vector2 targetPos)
        {
            Vector2 direction = targetPos - Position;
            direction.Normalize();
            direction = direction * maxVelocity;
            return direction;
        }

        protected Vector2 arrive(Vector2 targetPos)
        {
            float deceleration = 100; //can tweak this to alter the deceleration speed

            Vector2 toTarget = targetPos - Position;

            float distance = toTarget.Length();

            if (distance > 0)
            {
                float speed = distance / (deceleration * 0.3f);
                speed = Math.Min(speed, maxVelocity);

                Vector2 desiredVelocity = toTarget * (speed / distance);
                return (desiredVelocity - Velocity);
            }

            return Vector2.Zero;
        }
    }
}
