using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PlatformerGame2023;

public class Player
{
    public Level level { get; }

    Texture2D _texture;
    Vector2 position;
    float speed = 5f;
    private float movement;
    
    // Jumping state
    private bool isJumping;
    private bool wasJumping;
    private float jumpTime;

    int frameWidth = 32;
    int frameHeight = 32;
    Point currentFrame = new Point(0, 0);
    Point spriteSize = new Point(1, 10);

    public Player(Level level, Vector2 position)
    {
        this.level = level;
        this.position = position;
        _texture = Level.content.Load<Texture2D>("2d/ChikBoy_run");
        //LoadContent();
        //Reset(position);
    }

    public void Update(GameTime gameTime, KeyboardState keyboardState, Rectangle window)
    {
        if (keyboardState.IsKeyDown(Keys.A) && position.X > 0)
            position.X -= speed;
        if (keyboardState.IsKeyDown(Keys.D) && position.X < window.Width - frameWidth)
            position.X += speed;
        if (keyboardState.IsKeyDown(Keys.W) && position.Y > 0)
            position.Y -= speed;
        if (keyboardState.IsKeyDown(Keys.S) && position.Y < window.Height - frameHeight)
            position.Y += speed;

        ++currentFrame.Y;
        if (currentFrame.Y >= spriteSize.Y)
        {
            currentFrame.Y = 0;
        }
    }
    
    /*public void ApplyPhysics(GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Vector2 previousPosition = position;

        // Base velocity is a combination of horizontal movement control and
        // acceleration downward due to gravity.
        velocity.X += movement * MoveAcceleration * elapsed;
        velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

        velocity.Y = DoJump(velocity.Y, gameTime);

        // Apply pseudo-drag horizontally.
        if (IsOnGround)
            velocity.X *= GroundDragFactor;
        else
            velocity.X *= AirDragFactor;

        // Prevent the player from running faster than his top speed.            
        velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

        // Apply velocity.
        position += velocity * elapsed;
        position = new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y));

        // If the player is now colliding with the level, separate them.
        HandleCollisions();

        // If the collision stopped us from moving, reset the velocity to zero.
        if (position.X == previousPosition.X)
            velocity.X = 0;

        if (position.Y == previousPosition.Y)
            velocity.Y = 0;
    }*/

    
    private void GetInput(KeyboardState keyboardState)
    {

        // Ignore small movements to prevent running in place.
        if (Math.Abs(movement) < 0.5f)
            movement = 0.0f;

        // If any digital horizontal movement input is found, override the analog movement.
        if (keyboardState.IsKeyDown(Keys.Left) ||
            keyboardState.IsKeyDown(Keys.A))
            movement = -1.0f;
        else if (keyboardState.IsKeyDown(Keys.Right) ||
                 keyboardState.IsKeyDown(Keys.D))
            movement = 1.0f;

        // Check if the player wants to jump.
        isJumping =
            keyboardState.IsKeyDown(Keys.Space) ||
            keyboardState.IsKeyDown(Keys.Up) ||
            keyboardState.IsKeyDown(Keys.W);
    }


    public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(_texture, position,
                new Rectangle(currentFrame.X * frameWidth,
                    currentFrame.Y * frameHeight,
                    frameWidth, frameHeight),
                Color.White, 0, Vector2.Zero,
                1, SpriteEffects.None, 0);
        }
    }