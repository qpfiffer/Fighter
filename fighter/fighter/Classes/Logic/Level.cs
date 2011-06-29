using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace fighter
{
    class Level
    {
        #region PRIVATE_DATA
        constants.LEVELSTATE currentState = constants.LEVELSTATE.PLAYING;

        Character Player1;
        Character Player2;

        Texture2D levelBG;
        Rectangle bgRect;
        SpriteFont mainFont;

        Texture2D round_ind;
        SimpleSprite[] roundIndicators;

        string wintext;
        Vector2 winTextPos;
        #endregion

        #region properties
        /// <summary>
        /// Player 2
        /// </summary>
        public Character pPlayer2
        {
            get { return Player2; }
            set { }
        }

        /// <summary>
        /// Player 1
        /// </summary>
        public Character pPlayer1
        {
            get { return Player1; }
            set { }
        }

        /// <summary>
        /// Current state of the level.
        /// </summary>
        public constants.LEVELSTATE CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }
        #endregion

        public Level(String Player1Name, String Player2Name)
        {
            Player1 = new Character(Player1Name, true);
            Player2 = new Character(Player2Name, false);
            //Player2 = new Character(Player2Name, false, constants.ROBOT_BEHAVIOR.ATTACKING);

            Player1.MySprite.GlobPos = new Vector2(0,
                (int)(constants.GROUND_HEIGHT - (constants.SPRITE_SIZE * constants.SPRITE_SCALE)));
            Player2.MySprite.GlobPos = new Vector2(constants.SCREEN_WIDTH - (int)(constants.SPRITE_WIDTH * constants.SPRITE_SCALE),
                (int)(constants.GROUND_HEIGHT - (constants.SPRITE_SIZE * constants.SPRITE_SCALE)));

            bgRect = new Rectangle(0, 0, constants.SCREEN_WIDTH, constants.SCREEN_HEIGHT);
            roundIndicators = new SimpleSprite[constants.NUM_ROUNDS];
        }


        private void checkDamage(Character attacker, Character victim)
        {
            if (attacker.MyHitbox.Intersects(victim.MyHitbox))
            {
                if (attacker.isActiveFrame())
                {
                    if (!victim.isBlocking())
                    {
                        victim.doDamage(attacker.MySprite.damageValues[(int)attacker.MySprite.CurrentAnim]);
                        if (attacker.IsOnLeft)
                        {
                            attacker.incPos(-(attacker.MySprite.pushBack[(int)attacker.MySprite.CurrentAnim]), 0);
                            victim.incPos(attacker.MySprite.pushBack[(int)attacker.MySprite.CurrentAnim], 0);
                        }
                        else
                        {
                            attacker.incPos(attacker.MySprite.pushBack[(int)attacker.MySprite.CurrentAnim], 0);
                            victim.incPos(-attacker.MySprite.pushBack[(int)attacker.MySprite.CurrentAnim], 0);
                        }
                    }
                    else
                    {
                        if (attacker.IsOnLeft)
                        {
                            attacker.incPos(-(attacker.MySprite.blockedPushback[(int)attacker.MySprite.CurrentAnim]), 0);
                            victim.incPos(attacker.MySprite.blockedPushback[(int)attacker.MySprite.CurrentAnim], 0);
                        }
                        else
                        {
                            attacker.incPos(attacker.MySprite.blockedPushback[(int)attacker.MySprite.CurrentAnim], 0);
                            victim.incPos(-attacker.MySprite.blockedPushback[(int)attacker.MySprite.CurrentAnim], 0);
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(levelBG, bgRect, Color.White);

            Player1.DrawHud(spriteBatch);
            Player2.DrawHud(spriteBatch);

            Player1.DrawCharacter(spriteBatch);
            Player2.DrawCharacter(spriteBatch);

            checkDamage(Player1, Player2);
            checkDamage(Player2, Player1);

            if (Player1.IsDead)
            {
                if (Player1.MySprite.CurrentAnim != constants.AnimType.DYING)
                    Player1.setAnim(constants.AnimType.DYING);
                Player1.MySprite.StayOnLastFrame = true;
                Player1.MySprite.animLoop = false;
            }

            if (Player2.IsDead)
            {
                if (Player2.MySprite.CurrentAnim != constants.AnimType.DYING)
                    Player2.setAnim(constants.AnimType.DYING);
                Player2.MySprite.StayOnLastFrame = true;
                Player2.MySprite.animLoop = false;
            }

            foreach (SimpleSprite index in roundIndicators)
            {
                index.Draw(spriteBatch);
            }

            if (currentState != constants.LEVELSTATE.PLAYING)
            {
                spriteBatch.DrawString(mainFont, wintext, winTextPos, Color.Red);
            }
        }

        public void Load(ContentManager globManager)
        {
            Player1.Load(globManager);
            Player2.Load(globManager);

            levelBG = globManager.Load<Texture2D>("Level/testBG");
            mainFont = globManager.Load<SpriteFont>("Menu/mainFont");

            round_ind = globManager.Load<Texture2D>("Level/round_ind");

            // sets up the round indicators.
            const int INDICATORS_HEIGHT = 60;
            for (int i = 0; i < roundIndicators.Length; i++)
            {
                if (i < (roundIndicators.Length / 2))
                {
                    roundIndicators[i] = new SimpleSprite(round_ind, constants.SIMPLE_SPRITE_BEHAVIOR.CLIPPED,
                        new Vector2(628 - ((i + 1) * 40) - (i * 5), INDICATORS_HEIGHT));
                }
                else
                {
                    roundIndicators[i] = new SimpleSprite(round_ind, constants.SIMPLE_SPRITE_BEHAVIOR.CLIPPED,
                        new Vector2(630 + 16 + ((i % 2) * 40) + (i * 5), INDICATORS_HEIGHT));
                }
                roundIndicators[i].ClipTangle = new Rectangle(0, 0, 40, 40);
                roundIndicators[i].refreshClipping();
            }
        }

        /// <summary>
        /// This should only be called after everything has been constructed in the current level.
        /// </summary>
        public void Reset()
        {
            Player1.Reset();
            Player2.Reset();

            Player1.MySprite.GlobPos = new Vector2(0,
                (int)(constants.GROUND_HEIGHT - (constants.SPRITE_SIZE * constants.SPRITE_SCALE)));
            Player2.MySprite.GlobPos = new Vector2(constants.SCREEN_WIDTH - (int)(constants.SPRITE_WIDTH * constants.SPRITE_SCALE),
                (int)(constants.GROUND_HEIGHT - (constants.SPRITE_SIZE * constants.SPRITE_SCALE)));

            currentState = constants.LEVELSTATE.PLAYING;
        }

        /// <summary>
        /// Calls reset and sets each character's wins to 0.
        /// </summary>
        public void GameReset()
        {
            Player1.MyWins = 0;
            Player2.MyWins = 0;

            Rectangle dieHard = new Rectangle(0,0,40,40);
            foreach (SimpleSprite index in roundIndicators)
            {
                index.ClipTangle = dieHard;
                index.refreshClipping();
            }

            Reset();
        }

        public void Update(GameTime gametime)
        {
            // First make sure the characters are facing the right way
            if ((Player1 != null) && (Player2 != null))
            {
                int player1Pos = (int)Player1.MySprite.GlobPos.X + (constants.SPRITE_WIDTH / 2);
                int player2Pos = (int)Player2.MySprite.GlobPos.X + (constants.SPRITE_WIDTH / 2);

                if ((player1Pos > player2Pos) && Player1.IsOnLeft)
                {
                    Player1.flipLeft();
                    Player2.flipLeft();
                }
                else if ((player2Pos > player1Pos)
                    && Player2.IsOnLeft)
                {
                    Player1.flipLeft();
                    Player2.flipLeft();
                }
            }

            Player1.Update(gametime);
            Player2.Update(gametime);

            if (Player2.IsRobot)
            {
                switch (Player2.MyBehavior) 
                {
                    case constants.ROBOT_BEHAVIOR.BLOCKING:
                        if (Player2.MySprite.CurrentAnim != constants.AnimType.CROUCHING)
                            Player2.setAnim(constants.AnimType.CROUCHING);
                        if (Player1.isAttacking() && Player2.shouldBlock(Player1))
                        {
                            Player2.setAnim(constants.AnimType.BLOCKL);
                        }
                        break;
                    case constants.ROBOT_BEHAVIOR.ATTACKING:
                        if (Player2.MySprite.CurrentAnim != constants.AnimType.LPUNCH)
                            Player2.setAnim(constants.AnimType.LPUNCH);
                        break;
                }
            }

            if (currentState != constants.LEVELSTATE.ROUNDWIN &&
                currentState != constants.LEVELSTATE.GAMEWIN)
            {
                if (Player1.MySprite.IsOnLastFrame && Player1.IsDead)
                {
                    playerWin(Player2);
                }

                if (Player2.MySprite.IsOnLastFrame && Player2.IsDead)
                {
                    playerWin(Player1);
                }
            }
        }

        private void playerWin(Character player)
        {
            player.MyWins += 1;

            for (int i = 0; i < roundIndicators.Length; i++)
            {
                if (i < (roundIndicators.Length / 2))
                {
                    if (i < Player1.MyWins)
                        roundIndicators[i].ClipTangle = new Rectangle(40, 0, 40, 40);
                }
                else
                {
                    if ((i - (roundIndicators.Length / 2)) < Player2.MyWins)
                        roundIndicators[i].ClipTangle = new Rectangle(40, 0, 40, 40);
                }
                roundIndicators[i].refreshClipping();
            }

            if (player.MyWins == (constants.NUM_ROUNDS / 2))
            {
                wintext = player.MySprite.charName.ToUpper() + " IS CHAMPION. HOORAY!";
                currentState = constants.LEVELSTATE.GAMEWIN;
            }
            else
            {
                wintext = player.MySprite.charName.ToUpper() + " WINS";
                currentState = constants.LEVELSTATE.ROUNDWIN;
            }

            winTextPos.X = (constants.SCREEN_WIDTH / 2) - (mainFont.MeasureString(wintext).X / 2);
            winTextPos.Y = (constants.SCREEN_HEIGHT / 2) - (mainFont.MeasureString(wintext).Y / 2);
        }
    }
}
