using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace fighter
{
    class AnimatedSprite : FrameData
    {
        #region PRIVATE_DATA
        // Name of the sprite sheet
        private String _SpriteName;
        // The actual sprite sheet
        private Texture2D _mySheet;
        public Texture2D MySheet
        {
            get { return _mySheet; }
            set { _mySheet = value; }
        }

        // The current frame of our animation is stored in this rect:
        private Rectangle _ClipSheet;
        // Whether to mirror this sprite or not
        private bool _isLeft = true;
        public bool IsLeft {
            get { return _isLeft; }
            set { _isLeft = value; }
        }
        // Our current position on the screen:
        private Vector2 _globPos;
        public Vector2 GlobPos {
            get { return _globPos; }
            set { _globPos = value; }
        }

        // Current frame of the animation
        int _AnimFrame = 0;
        public int AnimFrame
        {
            get { return _AnimFrame; }
        }

        // The current animation we are in:
        constants.AnimType _CurrentAnim = constants.AnimType.STANDING;
        public constants.AnimType CurrentAnim
        {
            get { return _CurrentAnim; }
            set { _CurrentAnim = value; }
        }
        // This is incremented each time update is called (1/60th of a second.)
        // If we hit the animTimeStepNum with this, we change animFrames;
        int _CurrentTStepNum = 0;

        // Only loop animations like standing or walking
        bool loopCurrentAnimation = true;
        public bool animLoop
        {
            get { return loopCurrentAnimation; }
            set { loopCurrentAnimation = value; }
        }

        bool stayOnLastFrame = false;
        public bool StayOnLastFrame
        {
            get { return stayOnLastFrame; }
            set { stayOnLastFrame = value; }
        }

        bool isOnLastFrame = false;
        public bool IsOnLastFrame
        {
            get { return isOnLastFrame; }
        }
        #endregion

        public void Reset()
        {
            setAnim(constants.AnimType.STANDING);
            stayOnLastFrame = false;
            loopCurrentAnimation = true;
            isOnLastFrame = false;
        }

        public AnimatedSprite(String newName): base(newName)
        {
            _SpriteName = "Level/" + newName;
            _ClipSheet = new Rectangle(0, 0, 32, 64);
            _globPos = Vector2.Zero;
        }
        public AnimatedSprite(String newName, bool isLeft): this(newName)
        {
            _isLeft = isLeft;
        }

        public void incPos(int X, int Y)
        {
            if (_globPos.X + X < 0)
                _globPos.X = 0;
            else
                _globPos.X += X;

            if (_globPos.X + (X + constants.SPRITE_WIDTH * constants.SPRITE_SCALE) > constants.SCREEN_WIDTH)
                _globPos.X = constants.SCREEN_WIDTH - (constants.SPRITE_WIDTH * constants.SPRITE_SCALE);
            else
               _globPos.X += X;

            _globPos.Y += Y;
        }
        
        public void flipLeft() { _isLeft = !_isLeft; }

        public void Load(ContentManager globManager)
        {
            if (_SpriteName != null)
                _mySheet = globManager.Load<Texture2D>(_SpriteName);
            else
                throw new Exception("Null spritename.");
        }

        public void setAnim(constants.AnimType newType)
        {
            // Set the new animation type
            _CurrentAnim = newType;
            _CurrentTStepNum = 0;
            // Start the frame over
            _AnimFrame = 0;
            // Make sure the clipping rectangle knows whats going on
            _ClipSheet.X = (int)animationOffsets[(int)_CurrentAnim].X;
            _ClipSheet.Y = (int)animationOffsets[(int)_CurrentAnim].Y;
        }

        public void Update(GameTime gameTime)
        {
            _CurrentTStepNum++;
            if (loopCurrentAnimation == true)
            {
                if (_CurrentTStepNum == constants.ANIM_FRAME_UPDATE)
                {
                    // Go to the next frame
                    _AnimFrame++;
                    // (Actually do it)
                    _ClipSheet.X += 32;
                    // Reset our timer
                    _CurrentTStepNum = 0;
                    if (_AnimFrame >= animationFrames[(int)_CurrentAnim])
                    {
                        _AnimFrame = 0;
                        _ClipSheet.X = (int)animationOffsets[(int)_CurrentAnim].X;
                        _ClipSheet.Y = (int)animationOffsets[(int)_CurrentAnim].Y;
                    }
                }
            }
            else if (loopCurrentAnimation == false)
            {
                if (_CurrentTStepNum == constants.ANIM_FRAME_UPDATE_FAST)
                {
                    if (!stayOnLastFrame)
                    {
                        // Go to the next frame
                        _AnimFrame++;
                        // (Actually do it)
                        _ClipSheet.X += 32;
                        // Reset our timer
                        _CurrentTStepNum = 0;
                        if (_AnimFrame >= animationFrames[(int)_CurrentAnim])
                        {
                            _AnimFrame = 0;
                            _CurrentAnim = constants.AnimType.STANDING;
                            _ClipSheet.X = (int)animationOffsets[(int)_CurrentAnim].X;
                            _ClipSheet.Y = (int)animationOffsets[(int)_CurrentAnim].Y;
                            loopCurrentAnimation = true;
                        }
                    }
                    else
                    {
                        if (_AnimFrame < (animationFrames[(int)_CurrentAnim] - 1))
                        {
                            // Go to the next frame
                            _AnimFrame++;
                            // (Actually do it)
                            _ClipSheet.X += 32;
                            // Reset our timer
                            _CurrentTStepNum = 0;
                        }
                        else
                        {
                            isOnLastFrame = true;
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //if (_CurrentAnim == AnimType.JUMPING)
            //{
            //    int magicNum = (int)(Game1.GROUND_HEIGHT - (Game1.SPRITE_SIZE * Game1.SPRITE_SCALE));
            //    if ((_GlobPos.Y - magicNum) > 425)
            //    {
            //        _GlobPos.Y += JUMP_SPEED;
            //        isJumping = false;
            //    }
            //    else if (isJumping)
            //    {
            //        _GlobPos.Y -= JUMP_SPEED;
            //    }
            //    if (_GlobPos.Y > magicNum)
            //    {
            //        _GlobPos.Y = magicNum;
            //        _CurrentAnim = AnimType.STANDING;
            //    }
            //}
            if (_isLeft)
            {
                spriteBatch.Draw(_mySheet, _globPos, _ClipSheet, Color.White, 0f, Vector2.Zero, constants.SPRITE_SCALE,
                    SpriteEffects.None, 0f);
            }
            else
            {

                spriteBatch.Draw(_mySheet, _globPos, _ClipSheet, Color.White, 0f, Vector2.Zero, constants.SPRITE_SCALE,
                    SpriteEffects.FlipHorizontally, 0f);
            }
        }
    }
}
