using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace fighter
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region PRIVATE_DATA
        // Graphics/Drawing stuff
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Logic/Game stuff
        Level currentLevel;
        Menu currentMenu;


        struct PlayerData {
            public PlayerIndex myIndex;
            public GamePadState oldState;
            public GamePadState newState;
            public Character myChar;
            public bool created;
        }

        PlayerData P1;
        PlayerData P2;
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = constants.SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = constants.SCREEN_HEIGHT;

            currentMenu = new Menu();
            P1.created = false;
            P2.created = false;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            currentMenu.Load(Content);
        }

        protected override void UnloadContent()
        {
        }

        private void findPlayer(PlayerData player)
        {
            int i;
            for (i = 0; i < 4; i++)
            {
                if (GamePad.GetState((PlayerIndex)i).IsButtonDown(Buttons.Start) == true)
                {
                    player.myIndex = (PlayerIndex)i;
                    currentMenu.CurrentState = constants.GAMESTATE.MENU;
                    player.newState = GamePad.GetState((PlayerIndex)i);
                    player.created = true;
                    return;
                }
            }
        }

        void menuInput()
        {
            if ((P1.newState.DPad.Down == ButtonState.Pressed) &&
                    (P1.oldState.DPad.Down == ButtonState.Released))
            {
                currentMenu.CMenuItem += 1;
            }
            else if ((P1.newState.DPad.Up == ButtonState.Pressed) &&
                (P1.oldState.DPad.Up == ButtonState.Released))
            {
                currentMenu.CMenuItem -= 1;
            }

            if ((P1.newState.IsButtonDown(Buttons.A) == true) &&
                    (P1.oldState.IsButtonDown(Buttons.A) == false))
            {
                currentMenu.confirmMenuItem();
            }
            else if ((P1.newState.IsButtonDown(Buttons.B) == true) &&
                    (P1.oldState.IsButtonDown(Buttons.B) == false))
            {
                currentMenu.CurrentState = constants.GAMESTATE.START_SCREEN;
                P1.created = false;
                P2.created = false;
            }
        }

        void charSelectInput()
        {
            if (!P2.created)
            {
                findPlayer(P2);
            }
            if ((P1.newState.DPad.Right == ButtonState.Pressed) &&
                    (P1.oldState.DPad.Right == ButtonState.Released))
            {
                currentMenu.CCharItem += 1;
            }
            else if ((P1.newState.DPad.Left == ButtonState.Pressed) &&
                (P1.newState.DPad.Left == ButtonState.Released))
            {
                currentMenu.CCharItem -= 1;
            }

            if ((P1.newState.IsButtonDown(Buttons.A) == true) &&
                    (P1.oldState.IsButtonDown(Buttons.A) == false))
            {
                // Tell the menu to select the current character that is highlighed
                currentMenu.confirmMenuItem();
                // Set the current state to game
                currentMenu.CurrentState = constants.GAMESTATE.GAME;
                // Initialize the level
                currentLevel = new Level(currentMenu.P1Char, constants.DEFAULT_P2);
                currentLevel.Load(this.Content);

                P1.myChar = currentLevel.pPlayer1;
                P2.myChar = currentLevel.pPlayer2;
            }
            else if ((P1.newState.IsButtonDown(Buttons.B) == true) &&
                    (P1.oldState.IsButtonDown(Buttons.B) == false))
            {
                currentMenu.CurrentState = constants.GAMESTATE.MENU;
            }
        }

        void horizontalMovement(PlayerData player, PlayerData otherPlayer)
        {
            if (player.newState.DPad.Down == ButtonState.Released)
            {
                if (player.oldState.DPad.Down == ButtonState.Pressed)
                {
                    player.myChar.setAnim(constants.AnimType.STANDING);
                }
                // LEFT
                if (player.newState.DPad.Left == ButtonState.Pressed)
                {
                    if (player.myChar.shouldBlock(otherPlayer.myChar) &&
                        player.myChar.IsOnLeft)
                    {
                        if (player.myChar.MySprite.CurrentAnim != constants.AnimType.BLOCKH)
                            player.myChar.setAnim(constants.AnimType.BLOCKH);
                    }
                    else
                    {
                        if (player.myChar.MySprite.CurrentAnim != constants.AnimType.WALKING)
                            player.myChar.setAnim(constants.AnimType.WALKING);
                        player.myChar.incPos(-constants.SIMPLE_SPRITE_SPEED, 0);
                    }
                }
                else if (player.newState.DPad.Left == ButtonState.Released &&
                  player.oldState.DPad.Left == ButtonState.Pressed)
                {
                    player.myChar.setAnim(constants.AnimType.STANDING);
                }

                // RIGHT
                if (player.newState.DPad.Right == ButtonState.Pressed)
                {
                    if (player.myChar.shouldBlock(otherPlayer.myChar) &&
                        !player.myChar.IsOnLeft)
                    {
                        if (player.myChar.MySprite.CurrentAnim != constants.AnimType.BLOCKH)
                            player.myChar.setAnim(constants.AnimType.BLOCKH);
                    }
                    else
                    {
                        if (player.myChar.MySprite.CurrentAnim != constants.AnimType.WALKING)
                            player.myChar.setAnim(constants.AnimType.WALKING);
                        player.myChar.incPos(constants.SIMPLE_SPRITE_SPEED, 0);
                    }
                }
                else if (player.newState.DPad.Right == ButtonState.Released &&
                  player.oldState.DPad.Right == ButtonState.Pressed)
                {
                    player.myChar.setAnim(constants.AnimType.STANDING);
                }
            }
            else // Holding down
            {
                if (player.myChar.IsOnLeft)
                {
                    if (player.newState.DPad.Left == ButtonState.Pressed)
                    {
                        if (player.myChar.shouldBlock(otherPlayer.myChar))
                        {
                            player.myChar.setAnim(constants.AnimType.BLOCKL);
                        }
                        else
                        {
                            if (player.myChar.MySprite.CurrentAnim != constants.AnimType.CROUCHING)
                                player.myChar.setAnim(constants.AnimType.CROUCHING);
                        }
                    }
                    else
                    {
                        if (player.myChar.MySprite.CurrentAnim != constants.AnimType.CROUCHING)
                            player.myChar.setAnim(constants.AnimType.CROUCHING);
                    }
                }
                else // We are on the right
                {
                    if (player.newState.DPad.Right == ButtonState.Pressed)
                    {
                        if (player.myChar.shouldBlock(otherPlayer.myChar))
                        {
                            player.myChar.setAnim(constants.AnimType.BLOCKL);
                        }
                        else
                        {
                            if (player.myChar.MySprite.CurrentAnim != constants.AnimType.CROUCHING)
                                player.myChar.setAnim(constants.AnimType.CROUCHING);
                        }
                    }
                    else
                    {
                        if (player.myChar.MySprite.CurrentAnim != constants.AnimType.CROUCHING)
                            player.myChar.setAnim(constants.AnimType.CROUCHING);
                    }
                }
            }
        }

        void gameInput()
        {
            // Light punch
            if ((P1.newState.IsButtonDown(Buttons.X) == true) &&
                    (P1.oldState.IsButtonDown(Buttons.X) == false))
            {
                if (!P1.myChar.MySprite.IsOnLastFrame &&
                    P1.myChar.MySprite.CurrentAnim != constants.AnimType.DYING)
                {
                    P1.myChar.MySprite.animLoop = false;
                    P1.myChar.setAnim(constants.AnimType.LPUNCH);
                }
            }

            // Light kick
            if ((P1.newState.IsButtonDown(Buttons.A) == true) &&
                    (P1.oldState.IsButtonDown(Buttons.A) == false))
            {
                if (!P1.myChar.MySprite.IsOnLastFrame &&
                    P1.myChar.MySprite.CurrentAnim != constants.AnimType.DYING)
                {
                    P1.myChar.MySprite.animLoop = false;
                    P1.myChar.setAnim(constants.AnimType.LKICK);
                }
            }

            if ((P1.newState.IsButtonDown(Buttons.Y) == true) &&
                    (P1.oldState.IsButtonDown(Buttons.Y) == false))
            {
                if (!P2.myChar.MySprite.IsOnLastFrame &&
                    P2.myChar.MySprite.CurrentAnim != constants.AnimType.DYING)
                {
                    P2.myChar.MySprite.animLoop = false;
                    P2.myChar.setAnim(constants.AnimType.LKICK);
                }
            }

            if (P1.newState.DPad.Up == ButtonState.Pressed)
            {
                if (P1.newState.DPad.Right == ButtonState.Pressed)
                {
                    if (P1.myChar.IsOnLeft)
                        P1.myChar.startJump(constants.jumpType.FORWARD);
                    else if (!P1.myChar.IsOnLeft)
                        P1.myChar.startJump(constants.jumpType.BACK);
                }
                else if (P1.newState.DPad.Left == ButtonState.Pressed)
                {
                    if (P1.myChar.IsOnLeft)
                        P1.myChar.startJump(constants.jumpType.BACK);
                    else if (!P1.myChar.IsOnLeft)
                        P1.myChar.startJump(constants.jumpType.FORWARD);
                }
                else
                {
                    P1.myChar.startJump(constants.jumpType.NEUTRAL);
                }
            }


            // Move the characters around, but only if they're standing.
            if (P1.myChar.canMove())
            {
                horizontalMovement(P1, P2);
            }
        }

        void updateInput()
        {
            P1.newState = GamePad.GetState(P1.myIndex);
            P2.newState = GamePad.GetState(P2.myIndex);

            if (currentMenu.CurrentState == constants.GAMESTATE.MENU)
            {
                menuInput();
            }
            else if (currentMenu.CurrentState == constants.GAMESTATE.CHAR_SELECT)
            {
                charSelectInput();
            }
            else if (currentMenu.CurrentState == constants.GAMESTATE.GAME)
            {
                if (currentLevel.CurrentState == constants.LEVELSTATE.ROUNDWIN)
                {
                    if (P1.newState.IsButtonDown(Buttons.Start))
                    {
                        currentLevel.Reset();
                    }
                }
                else if (currentLevel.CurrentState == constants.LEVELSTATE.GAMEWIN)
                {
                    if (P1.newState.IsButtonDown(Buttons.Start))
                    {
                        currentLevel.GameReset();
                    }
                }
                gameInput();
            }

            P1.oldState = P1.newState;
            P2.oldState = P2.newState;
        }

        protected override void Update(GameTime gameTime)
        {
            // Quit on escape
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) == true)
                this.Exit();

            // Quit if something else wants us to
            if (currentMenu.CurrentState == constants.GAMESTATE.QUIT)
                this.Exit();

            if (currentMenu.CurrentState != constants.GAMESTATE.GAME)
            {
                if (currentMenu.CurrentState == constants.GAMESTATE.START_SCREEN)
                {
                    findPlayer(P1);
                }
                else
                {
                    updateInput();
                }
                currentMenu.Update(gameTime);
            }
            else
            {
                updateInput();

                currentLevel.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            if (currentMenu.CurrentState != constants.GAMESTATE.GAME)
            {
                currentMenu.Draw(spriteBatch);
            }
            else
            {
                currentLevel.Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
