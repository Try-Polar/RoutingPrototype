﻿using System;
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

        bool mInSkein = false;
        bool mIsLeader = false;
        bool mInFormation = false;

        public Skein Skein;

        Color mColor = Color.White;

        Vector2 mTarget;

        Vector2 mCurrentVector;

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
            mCurrentVector = mTarget - Position;
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
                    currentStatus = STATUS.Free;
                    mRecentlyFreed = true;
                    mInSkein = false;
                    mInFormation = false;
                    this.Skein = null;
                    mColor = Color.White;
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
                //mColor = Color.White;
                Vector2 toFormation = Vector2.Zero;
                if (mInSkein)
                {
                    if (!mInFormation)
                    {
                        toFormation = this.Skein.getCurrentCenter() - Position;
                        if (toFormation.Length() < 1)
                        {
                            mInFormation = true;
                            mColor = Color.Green;
                        }
                        else
                        {
                            toFormation.Normalize();
                            toFormation = toFormation * 5;//move towards the center but only effect current velocity by 30 percent of what it currently is 
                        }
                    }
                }
                Velocity += toFormation * (float)gameTime.ElapsedGameTime.TotalSeconds;

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
            spriteBatch.Draw(this.Texture, this.Rect, mColor);
            spriteBatch.End();
        }

        public bool RecentlyFreed
        {
            get { return mRecentlyFreed; }
            set { mRecentlyFreed = value; }
        }

        public bool inSkein
        {
            get { return mInSkein; }
            set { mInSkein = value; }
        }

        public bool isLeader
        {
            get { return mIsLeader; }
            set { mIsLeader = value; }
        }

        public bool inFormation
        {
            get { return mInFormation; }
            set { mInFormation = value; }
        }

        public Route CurrentRoute
        {
            get { return mCurrentRoute; }
        }

        public Vector2 CurrentVector
        {   
            get { return mCurrentVector; }
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
