using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

enum STATUS {  PickingUp, DroppingOff} //might have more statuses later

namespace RoutingPrototype
{
    class Pod : MovingEntity, IUpdateDraw
    {
        Vector2 mPickup;
        Vector2 mDropOff;

        int sWidth;
        int sHeight;

        //Just making these numbers up for the time being
        float maxVelocity = 10;
        float acceleration = 3;

        Random rnd = new Random();
        STATUS currentStatus = STATUS.PickingUp; //initially Picking up

        public Pod(Texture2D texture, Vector2 initialPosition, int screenWidth, int screenHeight) : base(texture, initialPosition)
        {
            sWidth = screenWidth;
            sHeight = screenHeight;
            generateRandomSpawnPoint();
        }

        //This will be abstract in this class, just using this here to make a very quick demonstration
        public void Update(GameTime gameTime)
        {
            checkStatus();
            movement(gameTime);
            
        }

        private void checkStatus()
        {
            if (currentStatus == STATUS.PickingUp)
            {
                Vector2 distance = mPickup - Position;
                if (distance.Length() <= 5) //going to assume 5 to essentially count as being there for now
                {
                    Velocity = Vector2.Zero; //gonna slow it down properly later
                    currentStatus = STATUS.DroppingOff;
                }
                
            }
            else
            {
                Vector2 distance = mDropOff - Position;
                if (distance.Length() <= 5) //going to assume 5 to essentially count as being there for now
                {
                    Velocity = Vector2.Zero; //gonna slow it down properly later
                    currentStatus = STATUS.PickingUp;
                    generateRandomSpawnPoint();
                }
                
            }
        }

        private void movement(GameTime gameTime)
        {
            Vector2 direction = Vector2.Zero;
            //Update the velocity
            if (currentStatus == STATUS.PickingUp)
            {
                direction = mPickup - Position;
            }
            else
            {
                direction = mDropOff - Position;
            }

            direction.Normalize();
            direction = direction * acceleration;
            Velocity += direction;
            if (Velocity.Length() < maxVelocity)
            {
                Velocity.Normalize();
                Velocity = Velocity * maxVelocity;
            }

            //Update position based on current velocity
            Position = Position + (Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            this.setRectanglePos(Position);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Check this works with multiple instances
            spriteBatch.Begin();
            spriteBatch.Draw(this.Texture, this.Rect, Color.White);
            spriteBatch.End();
        }

        private void generateRandomSpawnPoint()
        {
            //generate Pick up
            int x = rnd.Next(0, sWidth); //gonna change this to not allow spawning on the side windows later
            int y = rnd.Next(0, sHeight); //gonna change this to not allow spawning on the side windows later
            mPickup.X = x;
            mPickup.Y = y;

            //generate Drop off
            x = rnd.Next(0, sWidth);
            y = rnd.Next(0, sHeight);
            mDropOff.X = x;
            mDropOff.Y = y;
            Console.WriteLine(mPickup);
            Console.WriteLine(mDropOff);

        }
    }
}
