using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RoutingPrototype
{
    class CityPod : MovingEntity, IUpdateDraw
    {

        enum STATUS { landing, centering, leaving, takingOff}
        STATUS mCurrentStatus = STATUS.landing;
        bool mHasLanded = false;
        bool mHasCentered = false;
        bool mHasTakenOff = false;
        
        Vector2 mCityCenter;
        Vector2 mTakeOffTarget;
        Vector2 mExitPoint;

        Pod mPod;

        float mMass = 0.05f;
        float VELOCITY = 50;
        int mCityRadius;

        Vector2 mTarget;

        public CityPod(Texture2D texture, Vector2 pos, float screenWidth, Vector2 cityCenter, Vector2 takeOffDirection, int rndX, int rndY, Pod pod) : base (texture, pos)
        {
            mCityRadius = (int)screenWidth / 12;
            mCityCenter = cityCenter;
            takeOffDirection.Normalize();
            mTakeOffTarget = mCityCenter + (takeOffDirection * mCityRadius);
            takeOffDirection.X += rndX;
            takeOffDirection.Y += rndY;
            mExitPoint = mCityCenter + (takeOffDirection * mCityRadius * 2);
            maxVelocity = 1;
            mTarget = mCityCenter;
            mPod = pod;
        }

        public void Update(GameTime gameTime)
        {
            checkStatus();
            movement(gameTime);
        }

        void checkStatus()
        {
            //Check current status
            if (mCurrentStatus == STATUS.landing && Vector2.Distance(mCityCenter, Position) < mCityRadius)
            {
                Console.WriteLine("Landed");
                mCurrentStatus = STATUS.centering;
                mTarget = mCityCenter;
                VELOCITY = 20;
            }
            if (mCurrentStatus == STATUS.centering && Vector2.Distance(mCityCenter, Position) < 2)
            {
                Console.WriteLine("Centered");
                mCurrentStatus = STATUS.leaving;
                mTarget = mTakeOffTarget;
            }
            if (mCurrentStatus == STATUS.leaving && Vector2.Distance(mTakeOffTarget, Position) < 2)
            {
                Console.WriteLine("TakenOff");
                VELOCITY = 50;
                mCurrentStatus = STATUS.takingOff;
                mTarget = mExitPoint;
            }
            //If left city sim area delete city pod and free up pod in large sim
        }

        void movement(GameTime gameTime)
        {
            Vector2 acceleration = Vector2.Zero;

            if (mCurrentStatus == STATUS.landing)
            {
                acceleration = arrive(mTarget) / mMass;
                Velocity += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                Velocity = seek(mTarget);
            }
            

            if (Velocity.Length() > maxVelocity)
            {
                Velocity *= maxVelocity / Velocity.Length();
            }

            Position = Position + (Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds) * VELOCITY;
            this.updateRectanglePos();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Texture, Rect, Color.White);
            spriteBatch.End();
        }

        public Pod Pod
        {
            get { return mPod; }
        }
    }
}
