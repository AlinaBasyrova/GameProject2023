using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlatformerGame2023;

namespace Platformer2D
{
    public enum FaceDirection
    {
        Left = -1,
        Right = 1,
    }

    public enum EnemyType
    {
        Fighter = 1,
        Shooter = 2,
    }

    public class Enemy
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
        private Animation attackAnimation;
        public EnemyType enemyType;

        private AnimationPlayer sprite = new();

        private FaceDirection direction = FaceDirection.Left;

        private float waitTime;
        private float bulletWaitTime;
        private int attackWaitTime;

        public bool IsAttacking => isAttacking;
        bool isAttacking;

        private const float MaxWaitTime = 0.5f;
        private const float MoveSpeed = 115.0f;

        public Enemy(Level level, Vector2 position, int enemyType)
        {
            this.level = level;
            this.position = position;
            this.enemyType = (EnemyType)enemyType;

            LoadContent();
        }

        public void LoadContent()
        {
            if (enemyType == EnemyType.Fighter)
            {
                idleAnimation = new Animation(Level.content.Load<Texture2D>("2d/Ball and Chain Bot/idle"), 0.12f, true,
                    5);
                runAnimation = new Animation(Level.content.Load<Texture2D>("2d/Ball and Chain Bot/run"), 0.12f, true, 8);
                attackAnimation =
                    new Animation(Level.content.Load<Texture2D>("2d/Ball and Chain Bot/attack"), 0.1f, false, 8);
            }
            else
            {
                idleAnimation = new Animation(Level.content.Load<Texture2D>("2d/Bot Wheel/move with FX"), 0.2f, true, 8);
                runAnimation = new Animation(Level.content.Load<Texture2D>("2d/Bot Wheel/move with FX"), 0.2f, true, 8);
                attackAnimation =
                    new Animation(Level.content.Load<Texture2D>("2d/Bot Wheel/shoot with FX"), 0.1f, false, 4);
            }

            sprite.PlayAnimation(idleAnimation);

            int width = (int)(idleAnimation.FrameWidth * 0.5);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameHeight * 0.7);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);
        }


        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float posX = Position.X + localBounds.Width / 2 * (int)direction;
            int tileX = (int)Math.Floor(posX / Tile.Width) + (int)direction;
            int tileY = (int)Math.Floor(Position.Y / Tile.Height);

            FindPlayer();

            if (waitTime > 0)
            {
                waitTime = Math.Max(0.0f, waitTime - (float)gameTime.ElapsedGameTime.TotalSeconds);
                if (waitTime <= 0.0f)
                    direction = (FaceDirection)(-(int)direction);
            }
            else if (enemyType == EnemyType.Fighter)
            {
                if (Level.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.Impassable ||
                    Level.GetCollision(tileX + (int)direction, tileY) == TileCollision.Passable)
                {
                    waitTime = MaxWaitTime;
                }
                else
                {
                    Vector2 velocity = new Vector2((int)direction * MoveSpeed * elapsed, 0.0f);
                    position += velocity;
                }
            }
        }

        private void FindPlayer()
        {
            var distanceToPlayerX = level.player.Position.X - position.X;
            var distanceToPlayerY = level.player.Position.Y - position.Y;
            if ((distanceToPlayerX * (int)direction < 0 && !level.player.ISCrouching) ||
                distanceToPlayerX * (int)direction > 0)
            {
                if (distanceToPlayerY == 0 && Math.Abs(distanceToPlayerX) < 200 && !level.player.IsHiding)
                {
                    isAttacking = true;
                    direction = distanceToPlayerX > 0 ? FaceDirection.Right : FaceDirection.Left;
                    if (bulletWaitTime > 5)
                    {
                        if (attackWaitTime < 5)
                        {
                            isAttacking = true;
                            attackWaitTime++;
                        }

                        bulletWaitTime = 0;
                    }
                    else
                        bulletWaitTime++;
                }
            }
        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsAttacking && sprite.IsAnimationEnd)
            {
                sprite.PlayAnimation(attackAnimation);
                if (enemyType == EnemyType.Shooter && isAttacking)
                    level.CreateBullet(new Vector2(position.X, position.Y - 15), direction);
                isAttacking = false;

            }
            else if ((!Level.player.IsAlive || waitTime > 0) && sprite.IsAnimationEnd)
                sprite.PlayAnimation(idleAnimation);
            else if (sprite.IsAnimationEnd)
                sprite.PlayAnimation(runAnimation);

            SpriteEffects flip = direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            sprite.Draw(gameTime, spriteBatch, Position, flip, Color.White);
        }
    }
}