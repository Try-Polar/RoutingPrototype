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

        float mEnergy = 100;
        float mMinimumEnergyThreshold = 5;
        float mSufficientChargeThreshold = 95;
        float mRechargeRate = 60;
        bool mRecharging = false;

        bool mRecentlyFreed = true;

        float VELOCITY = 100;
        float mMass = 0.05f;


        Random rnd;
        STATUS currentStatus = STATUS.Free; //initially Picking up

        public Pod(Texture2D texture, Texture2D markerTexture, Vector2 initialPosition, int randomSeed) : base(texture, initialPosition)
        {
            rnd = new Random(randomSeed);
            maxVelocity = 1;
            mTarget = initialPosition;      
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


            if (mEnergy < mMinimumEnergyThreshold)
            {
                mRecharging = true;
            }
            if (!mRecharging)
            { 
                Vector2 acceleration = arrive(mTarget) / mMass;
                Velocity += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (Velocity.Length() < maxVelocity)
                {
                    Velocity.Normalize();
                    Velocity = Velocity * maxVelocity;
                }

                //Update position based on current velocity
                Vector2 distanceMoved = (Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds) * VELOCITY;
                Position = Position + distanceMoved;
                //Deduct energy based on distance moved (going to need to scale things to be appropriate to actual map)
                mEnergy -= distanceMoved.Length() * 0.17f;
                if (mEnergy < 0)
                    mEnergy = 0;
            }
            else //Recharge
            {
                mEnergy += (float)gameTime.ElapsedGameTime.TotalSeconds * mRechargeRate;
                if (mEnergy > 100)
                    mEnergy = 100;
                if (mEnergy > mSufficientChargeThreshold)
                {
                    mRecharging = false;
                }
            }
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
        
        public void assignRoute(Route newRoute)
        {
            mCurrentRoute = newRoute;
            currentStatus = STATUS.PickingUp;
        }
    }
}
