using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RoutingPrototype
{
    class MovingEntity : BaseEntity
    {
        Vector2 mVelocity;

        public MovingEntity(Texture2D texture, Vector2 initialPosition) : base(texture, initialPosition)
        {
            this.mVelocity = new Vector2(100, 100);//Vector2.Zero;
        }

        public Vector2 Velocity
        {
            get { return mVelocity; }
            set { mVelocity = value; }
        }

        //This will be abstract in this class, just using this here to make a very quick demonstration
        public override void Update(GameTime gameTime)
        {
            Console.WriteLine(gameTime.ElapsedGameTime.TotalSeconds);
            //Update position based on current velocity
            Position = Position + (mVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            this.setRectanglePos(Position);
            Console.WriteLine(Position);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Check this works with multiple instances
            spriteBatch.Begin();
            spriteBatch.Draw(this.Texture, this.Rect, Color.White);
            spriteBatch.End();
        }
    }
}
