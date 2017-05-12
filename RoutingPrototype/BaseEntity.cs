using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace RoutingPrototype
{
    abstract class BaseEntity
    {
        private Texture2D mTexture;
        private String mTexturePath;
        private Vector2 mPos;
        private Rectangle mRectangle;

        public BaseEntity(Texture2D texture, Vector2 initialPosition)
        {
            this.mTexture = texture;
            this.mPos = initialPosition;
            this.mRectangle = new Rectangle((int)mPos.X, (int)mPos.Y, mTexture.Width, mTexture.Height);
        }

        public Vector2 Position
        {
            get { return mPos; }
            set { mPos = value; }
        }

        public Texture2D Texture
        {
            get { return mTexture; }
            set { mTexture = value; }
        }

        public Rectangle Rect
        {
            get { return mRectangle; }
            set { mRectangle = value; }
        }

        public void setRectanglePos(Vector2 pos)
        {
            mRectangle.X = (int)pos.X;
            mRectangle.Y = (int)pos.Y;
        }


        public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch spriteBatch);


    }
}
