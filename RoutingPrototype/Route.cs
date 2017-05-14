using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RoutingPrototype
{
    class Route : IUpdateDraw
    {
        Vector2 mPickUp;
        Vector2 mDropOff;

        DestinationMarker mPickUpEntity;
        DestinationMarker mDropOffEntity;

        Texture2D lineTexture;

        bool mAssigned;
        bool mCompleted = false;

        public Route(Texture2D markerTexture, Texture2D lineText, Vector2 pickUpLoc, Vector2 dropOffLoc, bool assigned = false)
        {
            mPickUp = pickUpLoc;
            mDropOff = dropOffLoc;
            mAssigned = assigned;
            mPickUpEntity = new DestinationMarker(markerTexture, pickUpLoc);
            mDropOffEntity = new DestinationMarker(markerTexture, dropOffLoc);
            lineTexture = lineText;
            mPickUpEntity.colorRed();
            mDropOffEntity.colorRed();
        }

        public Vector2 PickUp
        {
            get { return mPickUp; }
            set { mPickUp = value; }
        }

        public Vector2 DropOff
        {
            get { return mDropOff; }
            set { mDropOff = value; }
        }

        public bool isAssigned
        {
            get { return mAssigned; }
        }

        public bool Completed
        {
            get { return mCompleted; }
        }


        public void assigned()
        {
            mPickUpEntity.colorYellow();
            mAssigned = true;
        }

        public void pickUpComplete()
        {
            mPickUpEntity.colorGreen();
            mDropOffEntity.colorYellow();
        }

        public void dropOffComplete()
        {
            //route probably will be deleted on completion so will probably never call this, the object will be deleted instead (deletion will occur in routeManager)
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            drawLine(spriteBatch);
            spriteBatch.Draw(mPickUpEntity.Texture, mPickUpEntity.Rect, mPickUpEntity.currentColor());
            spriteBatch.Draw(mDropOffEntity.Texture, mDropOffEntity.Rect, mDropOffEntity.currentColor());
            spriteBatch.End();
        }

        private void drawLine(SpriteBatch spriteBatch)
        {
            Vector2 pickUp = mPickUp;
            Vector2 dropOff = mDropOff;
            pickUp.X += mPickUpEntity.Texture.Width / 2;
            dropOff.X += mDropOffEntity.Texture.Width / 2;
            pickUp.Y += mPickUpEntity.Texture.Height / 2;
            dropOff.Y += mDropOffEntity.Texture.Height / 2;

            Vector2 edge = dropOff - pickUp;

            float angle = (float)Math.Atan2(edge.Y, edge.X);

            spriteBatch.Draw(lineTexture, new Rectangle((int)pickUp.X, (int)pickUp.Y, (int)edge.Length(), 1), null, Color.Black, angle, new Vector2(0, 0), SpriteEffects.None, 0);
        }
    }
}
