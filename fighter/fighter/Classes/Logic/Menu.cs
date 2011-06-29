using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace fighter
{
    class Menu
    {
        #region TEXTURES
        // Start Screen
        Texture2D pressStartBG;
        SimpleSprite movingTile;
        SimpleSprite pressStartBlink;

        // Menu screen
        String[] menuItems;
        SpriteFont mainFont;
        SimpleSprite mScreenWat;

        ////////////////////////////
        // Character Select screen
        ////////////////////////////
        // Background of the screen
        Texture2D sScreenBG;
        // Holds the portraits for character selection
        Texture2D[] portraits;
        // The actual chaaracter selecting thing
        SimpleSprite playerOneSelect;
        // Animated sprites to be used for animating the characters
        // on the selection screen
        List<AnimatedSprite> playerOneSelected;
        // Characters to be shown on the selection screen
        String[] sCharacters;

        #endregion

        #region PROPERTIES
        constants.GAMESTATE currentState;
        public constants.GAMESTATE CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }

        // This is the the current menu item
        // that is highlighted.
        int cMenuItem = 0;
        public int CMenuItem
        {
            get { return cMenuItem; }
            set
            {
                cMenuItem = value;
                if (value > (constants.MENU_SIZE - 1))
                    cMenuItem = 0;
                else if (value < 0)
                    cMenuItem = (constants.MENU_SIZE - 1);
            }
        }

        // Similar to cMenuItem, except this is used
        // for the charSelect screen
        int cCharItem = 0;
        public int CCharItem
        {
            get { return cCharItem; }
            set
            {
                cCharItem = value;
                if (value > (constants.NUM_CHARACTERS - 1))
                    cCharItem = 0;
                else if (value < 0)
                    cCharItem = (constants.NUM_CHARACTERS - 1);
            }
        }

        // This exists so we can tell the Level instance
        // what character the first player picked.
        String _P1Char;
        public String P1Char
        {
            get { return _P1Char; }
            set { _P1Char = value; }
        }
        #endregion

        #region CONSTRUCTORS
        public Menu()
        {
            currentState = constants.GAMESTATE.START_SCREEN;
            movingTile = new SimpleSprite("Menu/TILEME", constants.SIMPLE_SPRITE_BEHAVIOR.RSCROLL);
            pressStartBlink = new SimpleSprite("Menu/BLINK_START", constants.SIMPLE_SPRITE_BEHAVIOR.BLINKING, new Vector2(258, 676));

            // These are the strings used in the main menu
            menuItems = new String[constants.MENU_SIZE] { "ARCADE", "HORSEWASH", "JABBERWOCKY", "QUIT" };
            mScreenWat = new SimpleSprite("Menu/door", constants.SIMPLE_SPRITE_BEHAVIOR.MANUAL, new Vector2(600, 100));

            // Character select
            portraits = new Texture2D[constants.NUM_CHARACTERS];
            sCharacters = new String[constants.NUM_CHARACTERS] { "john", "john", "jack", "jack" };
            playerOneSelect = new SimpleSprite("Menu/Char_Select/player_select", constants.SIMPLE_SPRITE_BEHAVIOR.CLIPPED, new Vector2(328, 590));
            playerOneSelect.ClipTangle = new Rectangle(0, 0, 160, 80);
            playerOneSelected = new List<AnimatedSprite>();
        }

        /// <summary>
        /// Intermediate constructor which will display with a given state;
        /// </summary>
        /// <param name="newState">What type of gamestate you want this hud to use.</param>
        public Menu(constants.GAMESTATE newState) 
        {
            currentState = newState;
        }
        #endregion

        public void Load(ContentManager globManager)
        {
            // Start screen
            pressStartBG = globManager.Load<Texture2D>("Menu/pressStartbg");
            movingTile.Load(globManager);
            pressStartBlink.Load(globManager);

            // Menu Screen
            mainFont = globManager.Load<SpriteFont>("Menu/mainFont");
            mScreenWat.Load(globManager);

            // character select screen
            int i = 0;
            foreach (String charPortrait in sCharacters)
            {
                portraits[i] = globManager.Load<Texture2D>("Menu/Char_Select/" + charPortrait + "Por");

                AnimatedSprite tempSprite = new AnimatedSprite(charPortrait);
                playerOneSelected.Add(tempSprite);
                tempSprite.Load(globManager);
                tempSprite.GlobPos = new Vector2(135, 132);
                i++;
            }
            sScreenBG = globManager.Load<Texture2D>("Menu/Char_Select/char_select");
            playerOneSelect.Load(globManager);
        }

        public void Update(GameTime gametime)
        {
            if (currentState == constants.GAMESTATE.START_SCREEN) 
            {
                movingTile.Update(gametime);
                pressStartBlink.Update(gametime);
            }
            else if (currentState == constants.GAMESTATE.CHAR_SELECT)
            {
                playerOneSelected[cCharItem].Update(gametime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (currentState == constants.GAMESTATE.START_SCREEN) 
            {
                movingTile.Draw(spriteBatch);
                spriteBatch.Draw(pressStartBG, Vector2.Zero, Color.White);
                pressStartBlink.Draw(spriteBatch);
            } 
            else if (currentState == constants.GAMESTATE.MENU) 
            {
                int i;
                for (i = 0;i < menuItems.Length;i++)
                {
                    if (cMenuItem == i)
                    {
                        spriteBatch.DrawString(mainFont, menuItems[i],
                            new Vector2(5, 36 * i), Color.Yellow);
                    }
                    else
                    {
                        spriteBatch.DrawString(mainFont, menuItems[i],
                            new Vector2(5, 36 * i), Color.Red);
                    }
                }

                mScreenWat.Draw(spriteBatch);
            }
            else if (currentState == constants.GAMESTATE.CHAR_SELECT)
            {
                // Draw the background
                spriteBatch.Draw(sScreenBG, Vector2.Zero, Color.White);
                // Draw the character portraits
                int i = 0;
                Vector2 position = new Vector2(333, 505);
                foreach (Texture2D portrait in portraits)
                {
                    spriteBatch.Draw(portrait, position, Color.White);
                    position.X += 157;

                    i++;
                }
                // Draw the selector thing
                playerOneSelect.GlobPos = new Rectangle(328 + (cCharItem * (160)), 590,
                    playerOneSelect.MyTexture.Width, playerOneSelect.MyTexture.Height);
                playerOneSelect.Draw(spriteBatch);

                // Draw a preview of the character that is selected:
                playerOneSelected[cCharItem].Draw(spriteBatch);
            }
        }

        public void confirmMenuItem()
        {
            if (currentState == constants.GAMESTATE.MENU)
            {
                if (menuItems[cMenuItem].Equals("ARCADE"))
                {
                    currentState = constants.GAMESTATE.CHAR_SELECT;
                    return;
                }
                else if (menuItems[cMenuItem].Equals("QUIT"))
                {
                    currentState = constants.GAMESTATE.QUIT;
                    return;
                }
            } 
            else if (currentState == constants.GAMESTATE.CHAR_SELECT)
            {
                _P1Char = sCharacters[cCharItem];
            }
        }
    }
}
