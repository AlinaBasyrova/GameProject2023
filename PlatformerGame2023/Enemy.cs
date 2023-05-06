using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlatformerGame2023;

namespace Platformer2D
{
    enum FaceDirection
    {
        Left = -1,
        Right = 1,
    }
    
    class Enemy
    {
        public Level Level => level;
        Level level;

        public Vector2 Position => position;
        Vector2 position;

        private Rectangle localBounds;

        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        private Animation runAnimation;
        private Animation idleAnimation;
        private AnimationPlayer sprite = new();
        
        private FaceDirection direction = FaceDirection.Left;
        
        private float waitTime;
        private const float MaxWaitTime = 0.5f;
        private const float MoveSpeed = 115.0f;
        
        public Enemy(Level level, Vector2 position, string spriteSet)
        {
            this.level = level;
            this.position = position;

            LoadContent(spriteSet);
        }
        
        public void LoadContent(string spriteSet)
        {
            idleAnimation = new Animation(Level.content.Load<Texture2D>("2d/ChikBoy_idle"), 0.1f, true);
            runAnimation = new Animation(Level.content.Load<Texture2D>("2d/ChikBoy_run"), 0.1f, true);
            sprite.PlayAnimation(idleAnimation);

            int width = (int)(idleAnimation.FrameWidth * 0.35);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameHeight * 0.7);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);
        }



        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float posX = Position.X + localBounds.Width / 2 * (int)direction;
            int tileX = (int)Math.Floor(posX / Tile.Width) - (int)direction;
            int tileY = (int)Math.Floor(Position.Y / Tile.Height);

            if (waitTime > 0)
            {
                waitTime = Math.Max(0.0f, waitTime - (float)gameTime.ElapsedGameTime.TotalSeconds);
                if (waitTime <= 0.0f)
                    direction = (FaceDirection)(-(int)direction);
            }
            else
            {
                // If we are about to run into a wall or off a cliff, start waiting.
                if (Level.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.Impassable ||
                    Level.GetCollision(tileX + (int)direction, tileY) == TileCollision.Passable)
                {
                    waitTime = MaxWaitTime;
                }
                else
                {
                    Vector2 velocity = new Vector2((int)direction * MoveSpeed * elapsed, 0.0f);
                    position = position + velocity;
                }
            }
        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!Level.player.IsAlive || waitTime > 0)
                sprite.PlayAnimation(idleAnimation);
            else
                sprite.PlayAnimation(runAnimation);

            SpriteEffects flip = direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            sprite.Draw(gameTime, spriteBatch, Position, flip, Color.Red);
        }
    }
}