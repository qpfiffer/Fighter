using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fighter
{
    static class constants
    {
        #region ENUMERATORS
        public enum GAMESTATE
        {
            START_SCREEN,
            MENU,
            CHAR_SELECT,
            GAME,
            QUIT
        }

        public enum LEVELSTATE
        {
            PLAYING,
            ROUNDWIN,
            GAMEWIN
        }

        public enum HUDSTATE
        {
            PRESS_START,
            HEALTH
        }

        public enum AnimType
        {
            STANDING, WALKING, DASHING,
            LPUNCH, HPUNCH, JUMPING,
            LKICK, HKICK, DYING,
            BLOCKL, BLOCKH, HITSTUN,
            CROUCHING
        }

        public enum jumpType
        {
            BACK,
            NEUTRAL,
            FORWARD,
            FALLING
        }

        public enum SIMPLE_SPRITE_BEHAVIOR
        {
            BOUNCING,
            MANUAL,
            RSCROLL,
            BLINKING,
            CLIPPED
        }

        public enum ROBOT_BEHAVIOR
        {
            BLOCKING, ATTACKING
        }
        #endregion

        #region CONSTANT VALUES
        public const int SPRITE_SIZE = 63;
        public const float SPRITE_SCALE = 4;
        public const int SPRITE_WIDTH = 32;

        public const int GROUND_HEIGHT = 698;

        public const int SCREEN_HEIGHT = 720;
        public const int SCREEN_WIDTH  = 1280;

        public const int MENU_SIZE  = 4;

        public const int SIMPLE_SPRITE_SPEED = 3;
        public const int BLINK_FRAME_COUNT = 10;

        public const int NUM_CHARACTERS = 4;

        public const string DEFAULT_P2 = "john";

        public const int ANIM_FRAME_UPDATE = 20;
        public const int ANIM_FRAME_UPDATE_FAST = 5;

        public const string SETTINGS_FILENAME = "Content/Level/CHARACTER_DATA.xml";
        public const bool drawHitboxes = true;

        public const int blockDistance = 250;

        public const int NUM_ROUNDS = 4;

        public const float JUMP_SPEED = 8;
        public const int JUMP_HEIGHT = 275;

        #endregion
    }
}
