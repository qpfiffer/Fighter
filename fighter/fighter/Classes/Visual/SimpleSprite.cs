using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace fighter
{
    class SimpleSprite
    {
        #region DATA
        String myTextureName;
        Vector2 myVelocity;

        int curTBlinkNum = 0;
        bool showMe = true;
        #endregion

        #region PROPERTIES
        constants.SIMPLE_SPRITE_BEHAVIOR myBehavior;
        public constants.SIMPLE_SPRITE_BEHAVIOR MyBehavior
        {
            get { return myBehavior; }
            set { myBehavior = value; }
        }

        Texture2D myTexture;
        public Texture2D MyTexture
        {
            get { return myTexture; }
            set { myTexture = value; }
        }

        Rectangle globPos;
        public Rectangle GlobPos
        {
            get { return globPos; }
            set { globPos = value; }
        }

        Rectangle clipTangle;
        public Rectangle ClipTangle 
        {
            get { return clipTangle; }
            set { clipTangle = value;}
        }
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a simple sprite with the default "bouncing" beahvior.
        /// </summary>
        /// <param name="textureName">The name of the texture for this sprite.</param>
        public SimpleSprite(String textureName)
        {
            myTextureName = textureName;
            myVelocity = new Vector2(constants.SIMPLE_SPRITE_SPEED, 0);
            myBehavior = constants.SIMPLE_SPRITE_BEHAVIOR.BOUNCING;
        }

        /// <summary>
        /// Another simple sprite.
        /// </summary>
        /// <param name="textureName">Texture for this sprite (the name of)</param>
        /// <param name="behaviorType">Lets you specify a behavior.</param>
        public SimpleSprite(String textureName, constants.SIMPLE_SPRITE_BEHAVIOR behaviorType)
        {
            myTextureName = textureName;
            myVelocity = new Vector2(constants.SIMPLE_SPRITE_SPEED, 0);
            myBehavior = behaviorType;
        }

        /// <summary>
        /// Another simple sprite.
        /// </summary>
        /// <param name="textureName">Lets you specify the name of a texture to load.</param>
        /// <param name="behaviorType">Specify the behavior type.</param>
        /// <param name="newPos">Where the sprite will be on the screen.</param>
        public SimpleSprite(String textureName, constants.SIMPLE_SPRITE_BEHAVIOR behaviorType, Vector2 newPos)
        {
            myTextureName = textureName;
            myVelocity = new Vector2(constants.SIMPLE_SPRITE_SPEED, 0);
            myBehavior = behaviorType;
            globPos = new Rectangle((int)newPos.X, (int)newPos.Y, 0, 0);
        }

        /// <summary>
        /// Same as the previous.
        /// </summary>
        /// <param name="toUse">Lets the sprite use a pre-loaded texture.</param>
        /// <param name="behaviorType">Specify the sprite behavior.</param>
        /// <param name="newPos">Where the sprite will be on the screen.</param>
        public SimpleSprite(Texture2D toUse, constants.SIMPLE_SPRITE_BEHAVIOR behaviorType, Vector2 newPos)
        {
            myTextureName = null;
            myTexture = toUse;
            myVelocity = new Vector2(constants.SIMPLE_SPRITE_SPEED, 0);
            myBehavior = behaviorType;
            globPos = new Rectangle((int)newPos.X, (int)newPos.Y, 0, 0);
        }
        #endregion

        #region METHODS
        public void Load(ContentManager globManager)
        {
            if (myTextureName != null)
            {
                myTexture = globManager.Load<Texture2D>(myTextureName);

                if (globPos.Width == 0)
                    globPos.Width = myTexture.Width;
                if (globPos.Height == 0)
                    globPos.Height = myTexture.Height;
            }
            else
            {
                if (myTexture == null)
                {
                    globPos = Rectangle.Empty;
                }
                else
                {
                    if (myBehavior == constants.SIMPLE_SPRITE_BEHAVIOR.CLIPPED)
                    {
                        globPos.Width = clipTangle.Width;
                        globPos.Height = clipTangle.Height;
                    }
                    else
                    {
                        globPos.Width = myTexture.Width;
                        globPos.Height = myTexture.Height;
                    }
                }
            }
        }

        public void refreshClipping()
        {
            globPos.Width = clipTangle.Width;
            globPos.Height = clipTangle.Height;
        }

        public void Update(GameTime gametime)
        {
            if (myBehavior == constants.SIMPLE_SPRITE_BEHAVIOR.BOUNCING) {
                if (globPos.X + myVelocity.X < 0)
                    myVelocity.X *= -1;

                if ((globPos.X + myVelocity.X + globPos.Width) > constants.SCREEN_WIDTH)
                    myVelocity.X *= -1;

                if (globPos.Y + myVelocity.Y < 0)
                    myVelocity.Y *= -1;

                if ((globPos.Y + myVelocity.Y + globPos.Height) > constants.SCREEN_HEIGHT)
                    myVelocity.Y *= -1;

                globPos.X += (int)myVelocity.X;
                globPos.Y += (int)myVelocity.Y;
            }
            else if (myBehavior == constants.SIMPLE_SPRITE_BEHAVIOR.RSCROLL)
            {
                globPos.X += (int)myVelocity.X;

                if (globPos.X > constants.SCREEN_WIDTH)
                    globPos.X = (1-myTexture.Width);
            }
            else if (myBehavior == constants.SIMPLE_SPRITE_BEHAVIOR.BLINKING)
            {
                curTBlinkNum++;
                if (curTBlinkNum > constants.BLINK_FRAME_COUNT)
                {
                    showMe = !showMe;
                    curTBlinkNum = 0;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (myBehavior == constants.SIMPLE_SPRITE_BEHAVIOR.RSCROLL)
            {
                int i;
                for (i = 0; i <= (constants.SCREEN_WIDTH / myTexture.Width); i++)
                {
                    Vector2 drawVector = new Vector2(globPos.X + (myTexture.Width*i), globPos.Y);

                    if (drawVector.X > constants.SCREEN_WIDTH)
                        drawVector.X -= (constants.SCREEN_WIDTH + myTexture.Width);

                    spriteBatch.Draw(myTexture, drawVector, Color.White);
                }
            }
            else if (myBehavior == constants.SIMPLE_SPRITE_BEHAVIOR.BLINKING)
            {
                if (showMe)
                    spriteBatch.Draw(myTexture, globPos, Color.White);
            }
            else if (myBehavior == constants.SIMPLE_SPRITE_BEHAVIOR.MANUAL)
            {
                spriteBatch.Draw(myTexture, globPos, Color.White);
            }
            else if (myBehavior == constants.SIMPLE_SPRITE_BEHAVIOR.CLIPPED)
            {
                spriteBatch.Draw(myTexture, globPos, clipTangle, Color.White);
            }
        }
        #endregion
    }
}
