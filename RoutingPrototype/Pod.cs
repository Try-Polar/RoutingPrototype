using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

enum STATUS {  PickingUp, DroppingOff, Free} //might have more statuses later

namespace RoutingPrototype
{
    class Pod : MovingEntity, IUpdateDraw
    {        
        Route mCurrentRoute;

        Vector2 mTarget;

        bool mRecentlyFreed = true;

        float VELOCITY = 300;
        float mMass = 0.05f;

        int sWidth;
        int sHeight;

        Texture2D lineTexture;

        //Just making these numbers up for the time being
        

        Random rnd;
        STATUS currentStatus = STATUS.Free; //initially Picking up

        public Pod(Texture2D texture, Texture2D markerTexture, Texture2D lineText, Vector2 initialPosition, int screenWidth, int screenHeight, int randomSeed) : base(texture, initialPosition)
        {
            rnd = new Random(randomSeed);
            sWidth = screenWidth;
            sHeight = screenHeight;
            maxVelocity = 1;

            //generateRandomSpawnPoint();
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
                Vector2 distance = mCurrentRoute.PickUp - Position;
                if (distance.Length() <= 2) //going to assume 5 to essentially count as being there for now
                {
                    //Velocity = Vector2.Zero; //gonna slow it down properly later
                    currentStatus = STATUS.DroppingOff;
                    mCurrentRoute.pickUpComplete();
                }
                
            }
            else if (currentStatus == STATUS.DroppingOff)
            {
                Vector2 distance = mCurrentRoute.DropOff - Position;
                if (distance.Length() <= 2) //going to assume 5 to essentially count as being there for now
                {
                    //Velocity = Vector2.Zero; //gonna slow it down properly later
                    currentStatus = STATUS.Free;
                    mRecentlyFreed = true;
                    //generateRandomSpawnPoint();
                }  
            }
        }

        private void movement(GameTime gameTime)
        {
            //Update the velocity
            if (currentStatus == STATUS.PickingUp)
            {
                mTarget = mCurrentRoute.PickUp;
            }
            else if (currentStatus == STATUS.DroppingOff)
            {
                mTarget = mCurrentRoute.DropOff;
            }
            //Maybe move back to some centralised point if free?

            
            Vector2 acceleration = arrive(mTarget) / mMass;
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

        public bool isFree()
        {
            if (currentStatus == STATUS.Free)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            //Check this works with multiple instances
            spriteBatch.Begin();
            spriteBatch.Draw(this.Texture, this.Rect, Color.White);
            spriteBatch.End();
        }

        public bool RecentlyFreed
        {
            get { return mRecentlyFreed; }
            set { mRecentlyFreed = value; }
        }

        public Route CurrentRoute
        {
            get { return mCurrentRoute; }
        }

        public void clearRoute()
        {
            mCurrentRoute = null;
        }
        

        /*private void generateRandomSpawnPoint()
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
        }*/

        public void assignRoute(Route newRoute)
        {
            mCurrentRoute = newRoute;
            currentStatus = STATUS.PickingUp;
        }
    }
}
