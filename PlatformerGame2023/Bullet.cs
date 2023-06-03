using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platformer2D;

namespace PlatformerGame2023;

public class Bullet
{
    public Level Level => level;
    Level level;

    public Vector2 Position => position;
    Vector2 position;
    float speed = 10f;
    private Animation idleAnimation;
    private AnimationPlayer sprite = new();
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
    
    public Bullet(Level level, Vector2 position, FaceDirection direction)
    {
        this.level = level;
        this.position = position;
        this.direction = direction;

        LoadContent();
    }
    
    public void LoadContent()
    {
        idleAnimation = new Animation(Level.content.Load<Texture2D>("2d/Bullet"), 100f, true, 1);
        sprite.PlayAnimation(idleAnimation);

        int width = (int)(idleAnimation.FrameWidth * 0.35);
        int left = (idleAnimation.FrameWidth - width) / 2;
        int height = (int)(idleAnimation.FrameHeight * 0.7);
        int top = idleAnimation.FrameHeight - height;
        localBounds = new Rectangle(left, top, width, height);
    }

    public void Update(GameTime gameTime)
    {
        position.X = direction > 0 ? position.X + speed : position.X - speed;
        if (BoundingRectangle.Intersects(level.player.BoundingRectangle)) ;

    }
    
    public void Delete()
    {
        position = Vector2.Zero;
    }
    
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        sprite.Draw(gameTime, spriteBatch, Position, SpriteEffects.None, Color.White);
    }
}