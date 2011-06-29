using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace fighter
{
    class HUD
    {
        #region TEXTURES
        Texture2D _MySheet;
        #endregion

        bool _isLeft;
        Vector2 globPos;

        // How much of the healthbar to display at once:
        float disPercent = 1.0f;
        public float DisPercent
        {
            get { return disPercent; }
            set
            {
                disPercent = value;
            }
        }
        Rectangle _ClipSheet;

        constants.HUDSTATE currentState;
        public constants.HUDSTATE CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }

        const int totalHealth = 1000;
        int currentHealth = totalHealth;

        #region CONSTRUCTORS
        public HUD()
        {
            currentState = constants.HUDSTATE.HEALTH;
        }

        /// <summary>
        /// Intermediate constructor which will display with a given state;
        /// </summary>
        /// <param name="newState">What type of gamestate you want this hud to use.</param>
        public HUD(constants.HUDSTATE newState, bool onLeft) 
        {
            currentState = newState;
            _isLeft = onLeft;
            _ClipSheet = new Rectangle(0, 0, 612, 49);

            globPos = new Vector2(20, 10);
            if (!onLeft)
            {
                globPos.X = 648;
                globPos.Y = 10;
            }
        }
        #endregion

        public void doDamage(int damage)
        {
            currentHealth -= damage;
            float value = (float)((float)currentHealth / (float)totalHealth);
            if (value >= 0)
            {
                disPercent = value;
            }
            else
            {
                disPercent = 0.0f;
            }
        }

        public void Reset()
        {
            currentHealth = totalHealth;
            disPercent = 1.0f;
        }

        public void Load(ContentManager globManager)
        {
            _MySheet = globManager.Load<Texture2D>("HUD/healthBar");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (currentState == constants.HUDSTATE.HEALTH)
            {
                // How much of the bar to actually display
                int magicConstant = (int)(597 * disPercent);
                if (_isLeft)
                {
                    // Bar outline
                    spriteBatch.Draw(_MySheet, globPos, _ClipSheet, Color.White, 0f, Vector2.Zero, 1f,
                        SpriteEffects.None, 0f);
                    // Inner part
                    // What is going on here?
                    // Samedi help you.
                    // Texture used, position on the screen, source rectangle (clipping), color isnt used, some float, 
                    spriteBatch.Draw(_MySheet, new Vector2(globPos.X + 9 + (597 - magicConstant), globPos.Y + 8),
                        new Rectangle(7, 49, magicConstant, 35), Color.White, 0f, Vector2.Zero, 1f,
                        SpriteEffects.FlipHorizontally, 0f);
                    // MY CODE IS THE BEST
                }
                else
                {
                    // Bar outline
                    spriteBatch.Draw(_MySheet, globPos, _ClipSheet, Color.White, 0f, Vector2.Zero, 1f,
                        SpriteEffects.FlipHorizontally, 0f);
                    // Actual health
                    spriteBatch.Draw(_MySheet, new Vector2(globPos.X + 6, globPos.Y + 8), new Rectangle(7, 49, magicConstant, 35),
                        Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }
        }

        public void Update(GameTime gametime)
        {
        }
    }
}
