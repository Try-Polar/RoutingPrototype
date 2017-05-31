using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RoutingPrototype
{
    class CollaboratingCity : BaseEntity, IUpdateDraw
    {
        int mCityRadius;

        public CollaboratingCity(Texture2D texture, Vector2 pos, int cityRadius) : base (texture, pos)
        {
            mCityRadius = cityRadius;
            Position -= new Vector2(cityRadius, cityRadius);
            updateRectanglePos();
            setRectWidthHeight(mCityRadius * 2, mCityRadius * 2);
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //won't let me do this in the constructor :(

            spriteBatch.Begin();
            spriteBatch.Draw(Texture, Rect, Color.White);
            spriteBatch.End();
        }
    }
}
