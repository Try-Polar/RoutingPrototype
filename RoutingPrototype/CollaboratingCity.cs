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

        public CollaboratingCity(Texture2D texture, Vector2 pos) : base (texture, pos)
        {
            this.updateRectanglePos();
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
