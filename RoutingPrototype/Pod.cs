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
        DestinationMarker mPickUpEntity;
        Vector2 mDropOff;
        DestinationMarker mDropOffEntity;
        Vector2 target;

        float VELOCITY = 300;
        float mMass = 0.05f;

        int sWidth;
        int sHeight;

        Texture2D lineTexture;

        //Just making these numbers up for the time being
        

        Random rnd;
        STATUS currentStatus = STATUS.PickingUp; //initially Picking up

        public Pod(Texture2D texture, Texture2D markerTexture, Texture2D lineText, Vector2 initialPosition, int screenWidth, int screenHeight, int randomSeed) : base(texture, initialPosition)
        {
            rnd = new Random(randomSeed);
            sWidth = screenWidth;
            sHeight = screenHeight;
            maxVelocity = 1;
            mPickUpEntity = new DestinationMarker(markerTexture, Vector2.Zero);
            mDropOffEntity = new DestinationMarker(markerTexture, Vector2.Zero);
            generateRandomSpawnPoint();
            lineTexture = lineText;
            
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
                mPickUpEntity.colorYellow();
                mDropOffEntity.colorRed();
                Vector2 distance = mPickup - Position;
                if (distance.Length() <= 2) //going to assume 5 to essentially count as being there for now
                {
                    //Velocity = Vector2.Zero; //gonna slow it down properly later
                    currentStatus = STATUS.DroppingOff;
                }
                
            }
            else
            {
                mPickUpEntity.colorGreen();
                mDropOffEntity.colorYellow();
                Vector2 distance = mDropOff - Position;
                if (distance.Length() <= 2) //going to assume 5 to essentially count as being there for now
                {
                    //Velocity = Vector2.Zero; //gonna slow it down properly later
                    currentStatus = STATUS.PickingUp;
                    generateRandomSpawnPoint();
                }
                
            }
        }

        private void movement(GameTime gameTime)
        {
            //Update the velocity
            if (currentStatus == STATUS.PickingUp)
            {
                target = mPickup;
            }
            else
            {
                target = mDropOff;
            }

            
            Vector2 acceleration = arrive(target) / mMass;
            Velocity += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Velocity.Length() < maxVelocity)
            {
                Velocity.Normalize();
                Velocity = Velocity * maxVelocity;
            }

            //Update position based on current velocity
            Position = Position + (Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds) * VELOCITY;
            this.setRectanglePos(Position);
        }

        

        public void Draw(SpriteBatch spriteBatch)
        {
            mPickUpEntity.Draw(spriteBatch);
            mDropOffEntity.Draw(spriteBatch);
            //Check this works with multiple instances
            spriteBatch.Begin();
            spriteBatch.Draw(this.Texture, this.Rect, Color.White);
            drawLine(spriteBatch, mPickup, mDropOff);
            spriteBatch.End();
        }

        private void drawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end)
        {
            start.X += mPickUpEntity.Texture.Width / 2;
            end.X += mDropOffEntity.Texture.Width / 2;
            start.Y += mPickUpEntity.Texture.Height / 2;
            end.Y += mDropOffEntity.Texture.Height / 2;

            Vector2 edge = end - start;

            float angle = (float)Math.Atan2(edge.Y, edge.X);

            spriteBatch.Draw(lineTexture, new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), 1), null, Color.Black, angle, new Vector2(0, 0), SpriteEffects.None, 0);
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
            mPickUpEntity.setPosition(mPickup);
            mDropOffEntity.setPosition(mDropOff);
            Console.WriteLine(mPickup);
            Console.WriteLine(mDropOff);

        }
    }
}
