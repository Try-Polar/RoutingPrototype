using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RoutingPrototype
{
    class DestinationMarker : BaseEntity, IUpdateDraw
    {
        Color mColor = Color.Red;

        public DestinationMarker(Texture2D texture, Vector2 initialPosition) : base(texture, initialPosition)
        {

        }

        public void Update(GameTime gameTime)
        {

        }

        public void colorYellow()
        {
            mColor = Color.Yellow;
        }

        public void colorRed()
        {
            mColor = Color.Red;
        }

        public void colorGreen()
        {
            mColor = Color.LightGreen;
        }

        public Color currentColor()
        {
            return mColor;
        }

        public void setPosition(Vector2 pos)
        {
            Position = pos;
            this.setRectanglePos(pos);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(this.Texture, this.Rect, mColor);
            spriteBatch.End();
        }
    }
}
