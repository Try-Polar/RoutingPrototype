using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RoutingPrototype
{
    class City : BaseEntity, IUpdateDraw
    {
        string mName;

        int mWeighting;

        Vector2 mOffset;

        public City(Texture2D texture, Vector2 initialPosition, string cityName, int weighting) : base(texture, initialPosition)
        {
            mName = cityName;
            mWeighting = weighting;
            this.updateRectanglePos();
            mOffset.X = texture.Width / 2;
            mOffset.Y = texture.Height / 2;
        }

        public string Name
        {
            get { return mName; }
        }

        public int Weighting
        {
            get { return mWeighting; }
        }

        public Vector2 Offset
        {
            get { return mOffset; }
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Texture, Rect, Color.White);
            spriteBatch.End();
        }
    }
}
