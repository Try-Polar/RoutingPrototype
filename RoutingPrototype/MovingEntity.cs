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

        public MovingEntity(Texture2D texture, Vector2 initialPosition) : base(texture, initialPosition)
        {
            this.mVelocity = Vector2.Zero;
        }

        public Vector2 Velocity
        {
            get { return mVelocity; }
            set { mVelocity = value; }
        }
    }
}
