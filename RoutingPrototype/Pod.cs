﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

enum STATUS {  PickingUp, DroppingOff, Free, GoingToCharge, inCity} //might have more statuses later

namespace RoutingPrototype
{
    class Pod : MovingEntity, IUpdateDraw
    {        
        Route mCurrentRoute;

        const float PI = 3.14159265359f;

        bool mInSkein = false;
        bool mIsLeader = false;
        bool mInFormation = false;
        bool mInLondon = false;

        int mJourneysCompleted = 0;

        public Skein Skein;

        Color mColor = Color.White;

        float mColorSwitchInterval = 0.1f;
        float mColorswitchTimer = 0;

        float mTheoreticalDistTravelledOnJourney;   // The theoretical distance travelled (will be smaller than actual distance travelled because of skein savings)
        float mRealDistTravelledOnJourney;          // The actual distance travelled (regadless of whether in skein or not)
        float mMaxDistanceTravelled;

        public bool newData = false;

        Vector2 mTarget; //This is where the pod is directly aiming to go at any given moment

        Vector2 mGoal; // This is where the pod actually wants to end up (for example the goal would be the city the pod wants to go to while the target may be the point it is going to meet other pods)

        Vector2 mCurrentVector;

        Vector2 mJourneyStartLocation;


        float mEnergy = 300;
        float mMinimumEnergyThreshold = 5;
        float mSufficientChargeThreshold = 295;
        float mRechargeRate;
        bool mRecharging = false;
        float mDistMulti;
        float maxDistance;
        float mSkeinBonusMultiplier = 0.795f;
        float mChanceToLandInCity = 0.05f;

        int id;

        bool mRecentlyFreed = true;
        bool mGoingToCharge = false;
        bool mOnFinalApproach = false;
        bool mJourneyStarted = false;

        float VELOCITY;
        float mMass = 0.05f;

        CityManager mCityManager;
        CityPodManager mCityPodManager;
        Vector2 mLondonLocation;
        Vector2 mStartLocation;

        Random rnd;
        STATUS currentStatus = STATUS.Free;

        public Pod(Texture2D texture, Texture2D markerTexture, Vector2 initialPosition, CityManager cityManager, int randomSeed, int podId, float distMulti, float timeMulti, CityPodManager cityPodManager) : base(texture, initialPosition)
        {
            id = podId;
            rnd = new Random(randomSeed);
            mStartLocation = initialPosition;
            maxVelocity = 1;
            mGoal = initialPosition;
            mCityManager = cityManager;
            mCityPodManager = cityPodManager;
            maxDistance = 300 / mDistMulti;
            VELOCITY = 300 * distMulti / timeMulti;
            mDistMulti = distMulti;
            mRechargeRate = 300 / timeMulti;
            try
            {
                mLondonLocation = cityManager.findLondon();
            }
            catch (NullReferenceException)
            {
                mLondonLocation = Vector2.Zero;
            }
        }

        //This will be abstract in this class, just using this here to make a very quick demonstration
        public void Update(GameTime gameTime)
        {
            mColorswitchTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            mCurrentVector = mGoal - Position;
            if (inSkein)
            {
                this.Skein.checkLegalSkein(this);
            }
            checkStatus();
            movement(gameTime);
        }

        private void checkStatus()
        {
            if (currentStatus == STATUS.PickingUp)
            {
                float distance = distanceTo(mCurrentRoute.PickUp);
                if (distance <= 2)
                {
                    mOnFinalApproach = false;
                    currentStatus = STATUS.DroppingOff;
                    mCurrentRoute.pickUpComplete();
                }                
            }
            else if (currentStatus == STATUS.DroppingOff)
            {
                float distance = distanceTo(mCurrentRoute.DropOff);
                if (distance <= 2)
                {
                    mOnFinalApproach = false;
                    currentStatus = STATUS.Free;
                    if (mJourneyStarted)
                    {
                        mJourneysCompleted++;
                    }
                    mRecentlyFreed = true;
                    mColor = Color.White;
                    if (mCityManager != null)
                    {
                        if (rnd.NextDouble() < mChanceToLandInCity)
                        {
                            //"Land" in city
                            currentStatus = STATUS.inCity;
                            if (Vector2.Distance(Position, mLondonLocation) < 10)
                            {
                                mInLondon = true;
                            }
                            else
                            {
                                mInLondon = false;
                            }
                            mCityPodManager.AddPod(this);
                            mColor = Color.Blue;
                        }
                    }
                }  
            }
            if ((mGoal - Position).Length() < 2)// if arrived at current goal
            {
                if (mJourneyStarted) //if journey had been started
                    journeyEnded(); //end journey
                mJourneyStarted = false;
            }
            else
            {
                if (!mJourneyStarted)
                {
                    startJourney(mGoal);
                }
                mJourneyStarted = true;
            }
        }

        private void movement(GameTime gameTime)
        {
            
            if (currentStatus == STATUS.inCity)
            {
                return;
            }
            if (!mGoingToCharge)
            {
                if (currentStatus == STATUS.PickingUp)
                {
                    mGoal = mCurrentRoute.PickUp;
                    mTarget = mGoal;
                }
                else if (currentStatus == STATUS.DroppingOff)
                {
                    mGoal = mCurrentRoute.DropOff;
                    mTarget = mGoal;
                }

                if (mCityManager != null)
                {   //UK SIM
                    if (((distanceTo(mGoal) + 10)) > distanceAvailable() && !mRecharging) //Maybe should be target NOT GOAL
                    {
                        City chargingPoint = findBestChargingPoint();
                        if (chargingPoint != null)
                        {
                            mGoingToCharge = true;
                            mRecharging = false;
                            mColor = Color.White;
                            mGoal = chargingPoint.Position + chargingPoint.Offset;
                        }
                        
                    }
                }
                else
                {   //BOAT SIM
                    if (((distanceTo(mGoal) + 10)) * 2.2f > distanceAvailable() && !mRecharging) //Maybe should be target NOT GOAL
                    {
                        mGoingToCharge = true;
                        mRecharging = false;
                        mColor = Color.White;
                        mGoal = mStartLocation;
                    }
                }
                mTarget = mGoal; //Might seem redundant, to do this right after setting mGoal, but mTarget is likely to change while mGoal will not
            }
            else
            {
                if (distanceTo(mGoal) <= 2)
                {
                    
                    if (mJourneyStarted)
                        journeyEnded();
                    mJourneyStarted = false;
                    mRecharging = true;
                    mEnergy += (float)gameTime.ElapsedGameTime.TotalSeconds * mRechargeRate;
                    if (mEnergy > 300)
                    {
                        mOnFinalApproach = false;
                        mEnergy = 300;
                        mGoingToCharge = false;
                        mRecharging = false;
                    }
                }
            }
            
            //Maybe move back to some centralised point if free?


            if (mEnergy < mMinimumEnergyThreshold)
            {
                mRecharging = true;

            }
            if (!mRecharging)
            {
                //Console.WriteLine(distanceTo(mGoal) / mDistMulti + ", " + distanceAvailable());
                if (mInSkein && currentStatus != STATUS.Free)
                {
                    float angle = angleBetweenVectors((mGoal - Position), this.Skein.getCurrentVector());
                    //if ((mGoal - Position).Length() > 50 || ((angle < 0.349066 && angle >= 0) ||  (angle > 5.93412 && angle <= 6.28319)))
                    if ((mGoal - Position).Length() > 50 || ((angle < 0.698132 && angle >= 0) || (angle > 5.58505 && angle <= 6.28319)))
                    { 
                        if (Vector2.Distance(Position, this.Skein.getCurrentCenter()) > 5)
                        {
                            colorYellow();
                            mInFormation = false;
                            //Find meeting point
                            Vector2 meetingVector = (Normalize((this.Skein.getCurrentCenter() - Position))) * 1.1f;
                            meetingVector += (Normalize(mGoal - Position));
                            meetingVector *= Vector2.Distance(mGoal, Position);
                            mTarget = meetingVector + Position;       
                        }
                        else
                        {
                            mInFormation = true;
                            colorGreen();
                            mTarget = Position + (Normalize(this.Skein.getCurrentVector()) * Vector2.Distance(mGoal, Position));
                        }
                    }
                    else
                    {
                        //Leave Skein
                        mOnFinalApproach = true;
                        leaveSkein();
                    }
                    
                }

                Vector2 acceleration = arrive(mTarget) / mMass;
                Velocity += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (Velocity.Length() > maxVelocity)
                {
                    Velocity *= maxVelocity / Velocity.Length();
                }

                //Update position based on current velocity
                Vector2 distanceMoved = (Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds) * VELOCITY;
                Position = Position + distanceMoved;
                //Deduct energy based on distance moved (going to need to scale things to be appropriate to actual map)
                if (!inFormation)
                {
                    mEnergy -= distanceMoved.Length() / mDistMulti;
                    mTheoreticalDistTravelledOnJourney += distanceMoved.Length();
                    mRealDistTravelledOnJourney += distanceMoved.Length();
                }
                else
                {
                    mEnergy -= distanceMoved.Length() * mSkeinBonusMultiplier / mDistMulti;
                    mTheoreticalDistTravelledOnJourney += distanceMoved.Length() * mSkeinBonusMultiplier;
                    mRealDistTravelledOnJourney += distanceMoved.Length();
                }
                if (mEnergy < 0)
                    mEnergy = 0;
            }
            else //Recharge
            {
                if (inSkein)
                    leaveSkein();
                mEnergy += (float)gameTime.ElapsedGameTime.TotalSeconds * mRechargeRate;
                if (mEnergy > 300)
                    mEnergy = 300;
                if (mEnergy > mSufficientChargeThreshold)
                {
                    mRecharging = false;
                    mColor = Color.White;
                }
            }
            if (mRecharging == true)
            {
                mColor = Color.Orange;
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

        public bool isInCity()
        {
            if (currentStatus == STATUS.inCity)
            {
                return true;
            }
            return false;
        }

        public void skeinDispersed()
        {
            inSkein = false;
            inFormation = false;
            this.Skein = null;
            mColor = Color.White;
            mTarget = mGoal;
        }

        void leaveSkein()
        {

            this.Skein.remove(this);
            this.Skein = null;
            mInSkein = false;
            mInFormation = false;
            mColor = Color.White;
            mTarget = mGoal;
        }

        float distanceAvailable()
        {
            return mEnergy * mDistMulti;
        }

        float distanceTo(Vector2 loc)
        {
            return (loc - Position).Length();
        }

        float distanceBetween(Vector2 a, Vector2 b)
        {
            return (a - b).Length();
        }

        City findBestChargingPoint()
        {
            float bestDistanceToTarget = 9999999;
            City bestCity = null;
            foreach (City city in mCityManager.Cities)
            {
                if ((distanceTo(city.Position) + 10)  < distanceAvailable())
                {
                    if (distanceBetween(city.Position, mGoal) < bestDistanceToTarget)
                    {
                        bestCity = city;
                        bestDistanceToTarget = distanceBetween(city.Position, mGoal);
                    }
                }
            }

            return bestCity;
        }

        Vector2 relativeSeek(Vector2 targetPos, Vector2 otherPos)
        {
            Vector2 direction = targetPos - otherPos;
            direction.Normalize();
            direction = direction * maxVelocity;
            return direction;
        }

        Vector2 Normalize(Vector2 V) //This should be totally unecessary but from what I've tested I am unsure that the built-in normalize function actually works
        {
            return V / V.Length();
        }

        Vector2 findMeetingPoint()
        {
            Vector2 center = this.Skein.getCurrentCenter();
            return center + (Normalize(this.Skein.getCurrentVector()) * Vector2.Distance(Position, center));
        }

        void startJourney(Vector2 endPoint)
        {

            mTheoreticalDistTravelledOnJourney = 0;
            mRealDistTravelledOnJourney = 0;
            mJourneyStartLocation = Position;
            //mMaxDistanceTravelled = (endPoint - Position).Length();
        }

        void journeyEnded()
        {
            mMaxDistanceTravelled = (mJourneyStartLocation - Position).Length();
            newData = true;
            //mTotalDistTravelledOnJourney = mTheoreticalDistTravelledOnJourney * mDistMulti;
            //mTotalMaxDistanceTravelled = mMaxDistanceTravelled * mDistMulti;
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            //Check this works with multiple instances
            spriteBatch.Begin();
            //spriteBatch.Draw(this.Texture, this.Rect, mColor);
            spriteBatch.Draw(this.Texture, this.Position, new Rectangle(0, 0, Texture.Width, Texture.Height), mColor, getCurrentAngle(), this.Origin(), 1, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        float getCurrentAngle()
        {
            return (float)Math.Atan2(Velocity.Y, Velocity.X) + (PI / 2);
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

        public bool InLondon
        {
            get { return mInLondon; }
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

        public float TheoreticalDistanceTravelled
        {
            get { return mTheoreticalDistTravelledOnJourney; }
        }

        public bool Recharging
        {
            get { return mRecharging; }
        }

        public float RealDistanceTravelled
        {
            get { return mRealDistTravelledOnJourney; }
        }

        public float NonSkeinDistanceTravelled
        {
            get { return mMaxDistanceTravelled; }
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

        public void leftCity()
        {
            currentStatus = STATUS.Free;
            mColor = Color.White;
            mTarget = mGoal;
        }

        public bool onFinalApproach
        {
            get { return mOnFinalApproach; }
        }

        public int JourneysCompleted
        {
            get { return mJourneysCompleted; }
            set { mJourneysCompleted = value; }
        }

        void colorGreen()
        {
            if (mColorswitchTimer > mColorSwitchInterval)
            {
                mColor = Color.Green;
                mColorswitchTimer = 0;
            }
        }

        void colorYellow()
        {
            if (mColorswitchTimer > mColorSwitchInterval)
            {
                mColor = Color.Yellow;
                mColorswitchTimer = 0;
            }
        }

        float angleBetweenVectors(Vector2 a, Vector2 b)
        {
            a = a / a.Length();
            b = b / b.Length();
            return (float)Math.Acos(Vector2.Dot(a, b));
        }
    }
}
