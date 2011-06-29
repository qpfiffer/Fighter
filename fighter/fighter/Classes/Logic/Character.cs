using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace fighter
{
    class Character
    {
        #region PRIVATE_DATA
        AnimatedSprite mySprite;
        int myWins = 0;
        bool isOnLeft;
        bool isRobot = false;
        constants.ROBOT_BEHAVIOR myBehavior = constants.ROBOT_BEHAVIOR.BLOCKING;
        Rectangle myHitbox;
        private bool isDead = false;
        HUD myHealth;
        Vector2 startingPos;
        constants.jumpType myJumpType;
        #endregion

        #region properties
        /// <summary>
        /// Sprite of this character.
        /// </summary>
        public AnimatedSprite MySprite
        {
            get { return mySprite; }
            set { mySprite = value; }
        }

        /// <summary>
        /// Number of wins in a game this character has.
        /// </summary>
        public int MyWins
        {
            get { return myWins; }
            set { myWins = value; }
        }

        /// <summary>
        /// Whether this character is dead or not.
        /// </summary>
        public bool IsDead
        {
            get { return isDead; }
        }

        /// <summary>
        /// The hitbox of this character.
        /// </summary>
        public Rectangle MyHitbox
        {
            get { return myHitbox; }
        }

        /// <summary>
        /// If this character is a robot, their current behavior.
        /// </summary>
        public constants.ROBOT_BEHAVIOR MyBehavior
        {
            get { return myBehavior; }
        }

        /// <summary>
        /// Whether or not this is an AI.
        /// </summary>
        public bool IsRobot
        {
            get { return isRobot; }
        }

        /// <summary>
        /// Whether this character is on the left or not.
        /// </summary>
        public bool IsOnLeft
        {
            get { return isOnLeft; }
            set
            {
                isOnLeft = value;
                mySprite.IsLeft = value;
            }
        }
        #endregion

        public Character(string characterName, bool onLeft) 
        {
            mySprite = new AnimatedSprite(characterName, onLeft);
            myHealth = new HUD(constants.HUDSTATE.HEALTH, onLeft);
            isOnLeft = onLeft;
            Vector2 jumpSpeedDOWN = new Vector2(0.0f, -(constants.JUMP_SPEED));
            Vector2 jumpSpeedUP = new Vector2(0.0f, constants.JUMP_SPEED);
        }

        public Character(string characterName, bool onLeft, constants.ROBOT_BEHAVIOR robot)
            : this(characterName, onLeft)
        {
            this.myBehavior = robot;
            isRobot = true;
        }

        public void doDamage(int damage)
        {
            myHealth.doDamage(damage);
            if (myHealth.DisPercent <= 0.0f)
            {
                isDead = true;
            }
        }

        public bool isBlocking()
        {
            if (mySprite.CurrentAnim == constants.AnimType.BLOCKH)
                return true;
            if (mySprite.CurrentAnim == constants.AnimType.BLOCKL)
                return true;
            return false;
        }

        public void DrawCharacter(SpriteBatch spriteBatch)
        {
            mySprite.Draw(spriteBatch);
        }

        public void DrawHud(SpriteBatch spriteBatch)
        {
            myHealth.Draw(spriteBatch);
        }

        public void updateJump()
        {
            if (mySprite.CurrentAnim == constants.AnimType.JUMPING)
            {
                if (startingPos.Y - mySprite.GlobPos.Y > constants.JUMP_HEIGHT)
                {
                    myJumpType = constants.jumpType.FALLING;
                }

                if ((mySprite.GlobPos.Y > startingPos.Y) && myJumpType == constants.jumpType.FALLING)
                {
                    mySprite.GlobPos = new Vector2(mySprite.GlobPos.X, startingPos.Y);
                    setAnim(constants.AnimType.STANDING);
                }

                int xPos = 0;
                if (isOnLeft)
                {
                    if (myJumpType == constants.jumpType.FORWARD)
                    {
                        xPos = (int)constants.JUMP_SPEED/2;
                    }
                    else if (myJumpType == constants.jumpType.BACK)
                    {
                        xPos -= (int)constants.JUMP_SPEED/2;
                    }
                }
                else
                {
                    {
                        if (myJumpType == constants.jumpType.FORWARD)
                        {
                            xPos -= (int)constants.JUMP_SPEED/2;
                        }
                        else if (myJumpType == constants.jumpType.BACK)
                        {
                            xPos = (int)constants.JUMP_SPEED/2;
                        }
                    }
                }
                // We are falling. Decrement pos.
                if (myJumpType != constants.jumpType.FALLING)
                {
                    mySprite.incPos(xPos, -((int)constants.JUMP_SPEED));
                }
                else
                {
                    // We are still jumping.
                    mySprite.incPos(xPos, (int)constants.JUMP_SPEED);
                }
            }
        }

        public void startJump(constants.jumpType jumpType)
        {
            if (mySprite.CurrentAnim != constants.AnimType.JUMPING)
            {
                this.setAnim(constants.AnimType.JUMPING);
                myJumpType = jumpType;
                updateJump();
            }
        }

        public bool canMove()
        {
            if (mySprite.CurrentAnim == constants.AnimType.STANDING)
                return true;
            if (mySprite.CurrentAnim == constants.AnimType.CROUCHING)
                return true;
            if (mySprite.CurrentAnim == constants.AnimType.WALKING)
                return true;
            if (mySprite.CurrentAnim == constants.AnimType.DASHING)
                return true;
            if (mySprite.CurrentAnim == constants.AnimType.BLOCKH)
                return true;
            if (mySprite.CurrentAnim == constants.AnimType.BLOCKL)
                return true;
            return false;
        }

        public bool isWalking()
        {
            if (mySprite.CurrentAnim == constants.AnimType.WALKING)
                return true;
            return false;
        }

        public bool isActiveFrame()
        {
            if (isAttacking())
            {
                foreach (int activeFrame in mySprite.activeAnimationFrames[(int)mySprite.CurrentAnim])
                {
                    if (activeFrame == mySprite.AnimFrame)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool isAttacking()
        {
            if (mySprite.activeAnimationFrames[(int)mySprite.CurrentAnim] != null)
                return true;
            return false;
        }

        public bool shouldBlock(Character otherPlayer)
        {
            int player2XPos = (int)otherPlayer.MySprite.GlobPos.X;

            if (otherPlayer.isAttacking()) 
            {
                if (isOnLeft && (player2XPos - mySprite.GlobPos.X <= constants.blockDistance))
                    return true;
                else if (!isOnLeft && (mySprite.GlobPos.X - player2XPos <= constants.blockDistance))
                    return true;
            }

            return false;
        }

        public void Load(ContentManager globManager)
        {
            mySprite.Load(globManager);
            myHealth.Load(globManager);
            startingPos = mySprite.GlobPos;
        }

        public void Update(GameTime gametime)
        {
            mySprite.Update(gametime);
            myHitbox.X = (int)mySprite.GlobPos.X;
            myHitbox.Y = (int)mySprite.GlobPos.Y;
            myHitbox.Width = (int)(constants.SPRITE_WIDTH * constants.SPRITE_SCALE);
            myHitbox.Height = (int)(constants.SPRITE_SIZE * constants.SPRITE_SCALE);
            updateJump();
        }

        public void incPos(int X, int Y)
        {
            mySprite.incPos(X, Y);
        }

        public void flipLeft() 
        { 
            isOnLeft = !isOnLeft;
            mySprite.flipLeft();
        }

        public void setAnim(constants.AnimType newType)
        {
            mySprite.setAnim(newType);
        }

        public void Reset()
        {
            isDead = false;
            mySprite.Reset();
            myHealth.Reset();
        }
    }
}
