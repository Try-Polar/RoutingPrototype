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

        const float PI = 3.14159265359f;

        enum STATUS { landing, centering, leaving, takingOff}
        STATUS mCurrentStatus = STATUS.landing;
        bool mHasLanded = false;
        bool mHasCentered = false;
        bool mHasTakenOff = false;
        
        Vector2 mCityCenter;
        Vector2 mTakeOffTarget;
        Vector2 mExitPoint;

        Texture2D mCityPodTexture;

        Pod mPod;

        float mMass = 0.05f;
        float VELOCITY = 50;
        int mCityRadius;

        Vector2 mTarget;

        public CityPod(Texture2D texture, Vector2 pos, float screenWidth, Vector2 cityCenter, Vector2 takeOffDirection, int rndX, int rndY, Pod pod, int cityRadius, Texture2D cityPodTexture) : base (texture, pos)
        {
            mCityRadius = cityRadius;
            mCityCenter = cityCenter;
            takeOffDirection.Normalize();
            mTakeOffTarget = mCityCenter + (takeOffDirection * mCityRadius);
            takeOffDirection.X += rndX;
            takeOffDirection.Y += rndY;
            mExitPoint = mCityCenter + (takeOffDirection * mCityRadius * 2);
            maxVelocity = 1;
            mTarget = mCityCenter;
            mPod = pod;
            mCityPodTexture = cityPodTexture;
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
                mCurrentStatus = STATUS.centering;
                mTarget = mCityCenter;
                VELOCITY = 20;
            }
            if (mCurrentStatus == STATUS.centering && Vector2.Distance(mCityCenter, Position) < 2)
            {
                mCurrentStatus = STATUS.leaving;
                mTarget = mTakeOffTarget;
            }
            if (mCurrentStatus == STATUS.leaving && Vector2.Distance(mTakeOffTarget, Position) < 2)
            {
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
            //spriteBatch.Draw(Texture, Rect, Color.White);
            if (mCurrentStatus == STATUS.centering || mCurrentStatus == STATUS.leaving)
            {
                Vector2 origin;
                origin.X = mCityPodTexture.Width / 2;
                origin.Y = mCityPodTexture.Height / 2;
                spriteBatch.Draw(mCityPodTexture, this.Position, new Rectangle(0, 0, mCityPodTexture.Width, mCityPodTexture.Height), Color.White, getCurrentAngle() + PI, origin, 1, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(this.Texture, this.Position, new Rectangle(0, 0, Texture.Width, Texture.Height), Color.White, getCurrentAngle(), this.Origin(), 1, SpriteEffects.None, 0);
            }
            spriteBatch.End();
        }

        float getCurrentAngle()
        {
            return (float)Math.Atan2(Velocity.Y, Velocity.X) + (PI / 2);
        }

        public Pod Pod
        {
            get { return mPod; }
        }
    }
}
