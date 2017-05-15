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

        public City(Texture2D texture, Vector2 initialPosition, string cityName, int weighting) : base(texture, initialPosition)
        {
            mName = cityName;
            mWeighting = weighting;
            this.updateRectanglePos();
        }

        public string Name
        {
            get { return mName; }
        }

        public int Weighting
        {
            get { return mWeighting; }
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
