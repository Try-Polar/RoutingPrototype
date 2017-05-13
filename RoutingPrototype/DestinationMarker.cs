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
        Color color = Color.Red;

        public DestinationMarker(Texture2D texture, Vector2 initialPosition) : base(texture, initialPosition)
        {

        }

        public void Update(GameTime gameTime)
        {

        }

        public void colorYellow()
        {
            color = Color.Yellow;
        }

        public void colorRed()
        {
            color = Color.Red;
        }

        public void colorGreen()
        {
            color = Color.LightGreen;
        }

        public void setPosition(Vector2 pos)
        {
            Position = pos;
            this.setRectanglePos(pos);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(this.Texture, this.Rect, color);
            spriteBatch.End();
        }
    }
}
