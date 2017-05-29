using System;
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

        bool mInSkein = false;
        bool mIsLeader = false;
        bool mInFormation = false;
        bool mInLondon = false;

        public Skein Skein;

        Color mColor = Color.White;

        float mColorSwitchInterval = 0.1f;
        float mColorswitchTimer = 0;

        float mDistTravelledOnJourney;
        float mMaxDistanceTravelled;

        public bool newData = false;

        Vector2 mTarget; //This is where the pod is directly aiming to go at any given moment

        Vector2 mGoal; // This is where the pod actually wants to end up (for example the goal would be the city the pod wants to go to while the target may be the point it is going to meet other pods)

        Vector2 mCurrentVector;


        float mEnergy = 100;
        float mMinimumEnergyThreshold = 5;
        float mSufficientChargeThreshold = 95;
        float mRechargeRate = 60;
        bool mRecharging = false;
        float mDistanceScaler = 0.25f;
        float mDistMulti;
        float maxDistance;
        float mSkeinBonusMultiplier = 0.85f;
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

        Random rnd;
        STATUS currentStatus = STATUS.Free;

        public Pod(Texture2D texture, Texture2D markerTexture, Vector2 initialPosition, CityManager cityManager, int randomSeed, int podId, float distMulti, float timeMulti, CityPodManager cityPodManager) : base(texture, initialPosition)
        {
            id = podId;
            rnd = new Random(randomSeed);
            maxVelocity = 1;
            mGoal = initialPosition;
            mCityManager = cityManager;
            mCityPodManager = cityPodManager;
            maxDistance = 100 / mDistanceScaler;
            VELOCITY = 300 * distMulti / timeMulti;
            mDistMulti = distMulti;
            mLondonLocation = cityManager.findLondon();
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
                    mRecentlyFreed = true;
                    mColor = Color.White;
                    if (rnd.NextDouble() < mChanceToLandInCity)
                    {
                        Console.WriteLine(id + "Landing");
                        //"Land" in city
                        currentStatus = STATUS.inCity;
                        if (Vector2.Distance(Position, mLondonLocation) < 2)
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
            if ((mGoal - Position).Length() < 2)
            {
                if (mJourneyStarted)
                    journeyEnded();
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

                if (distanceTo(mGoal) + 25 > distanceAvailable() && !mRecharging) //Maybe should be target NOT GOAL
                {
                    City chargingPoint = findBestChargingPoint();
                    if (chargingPoint != null)
                    {
                        mGoingToCharge = true;
                        mRecharging = false;
                        mGoal = chargingPoint.Position;
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
                    if (mEnergy > 100)
                    {
                        mOnFinalApproach = false;
                        mEnergy = 100;
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
                        this.Skein.remove(this);
                        this.Skein = null;
                        mInSkein = false;
                        mInFormation = false;
                        mColor = Color.White;
                        mTarget = mGoal;
                    }
                    
                }

                Vector2 acceleration = arrive(mTarget) / mMass;
                //Console.WriteLine("Aceceleration: " + acceleration);
                Velocity += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //Console.WriteLine("Velocity: " + Velocity);

                if (Velocity.Length() > maxVelocity)
                {
                    Velocity *= maxVelocity / Velocity.Length();
                }

                //Update position based on current velocity
                Vector2 distanceMoved = (Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds) * VELOCITY;
                Position = Position + distanceMoved;
                //Console.WriteLine(Position);
                //Deduct energy based on distance moved (going to need to scale things to be appropriate to actual map)
                if (!inFormation)
                {
                    mEnergy -= distanceMoved.Length() * mDistanceScaler;
                    mDistTravelledOnJourney += distanceMoved.Length();
                }
                else
                {
                    mEnergy -= distanceMoved.Length() * mDistanceScaler * mSkeinBonusMultiplier;
                    mDistTravelledOnJourney += distanceMoved.Length() * mSkeinBonusMultiplier;
                }
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

        float distanceAvailable()
        {
            return mEnergy / mDistanceScaler;
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
                if (distanceTo(city.Position) < distanceAvailable())
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

            mDistTravelledOnJourney = 0;
            mMaxDistanceTravelled = (endPoint - Position).Length();
        }

        void journeyEnded()
        {
            newData = true;
            //mTotalDistTravelledOnJourney = mDistTravelledOnJourney * mDistMulti;
            //mTotalMaxDistanceTravelled = mMaxDistanceTravelled * mDistMulti;
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

        public float ActualDistanceTravelled
        {
            get { return mDistTravelledOnJourney; }
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
            Console.WriteLine(id + "LeavingCity");
        }

        public bool onFinalApproach
        {
            get { return mOnFinalApproach; }
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
