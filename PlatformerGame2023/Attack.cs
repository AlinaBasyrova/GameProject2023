using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platformer2D;


namespace PlatformerGame2023.Content._2d;

public class Attack
{
    public Level Level => level;
    Level level;

    public Vector2 Position => position;
    Vector2 position;
    float speed = 10f;
    private Animation idleAnimation;
    public AnimationPlayer sprite = new();
    private FaceDirection direction = FaceDirection.Left;
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

    public Attack(Level level, Vector2 position)
    {
        this.level = level;
        this.position = position;

        LoadContent();
    }

    public void LoadContent()
    {
        idleAnimation = new Animation(Level.content.Load<Texture2D>("2d/Thrusts 1 SpriteSheet"), 0.1f, true, 5);
        sprite.PlayAnimation(idleAnimation);

        int width = (int)(idleAnimation.FrameWidth * 0.35);
        int left = (idleAnimation.FrameWidth - width) / 2;
        int height = (int)(idleAnimation.FrameHeight * 0.7);
        int top = idleAnimation.FrameHeight - height;
        localBounds = new Rectangle(left, top, width, height);
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch) =>
        sprite.Draw(gameTime, spriteBatch, Position, SpriteEffects.None, Color.White);
}