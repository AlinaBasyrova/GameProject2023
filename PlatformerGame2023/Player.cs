using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PlatformerGame2023;

public class Player
{
    private Animation idleAnimation;
    private Animation runAnimation;
    private Animation jumpAnimation;
    private AnimationPlayer sprite = new AnimationPlayer();
    private SpriteEffects flip = SpriteEffects.None;

    public Level level { get; }

    Texture2D _texture;

    public Vector2 Position
    {
        get { return position; }
        set { position = value; }
    }

    Vector2 position;

    public Vector2 Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }

    Vector2 velocity;
    float speed = 5f;
    private float movement;

    // Jumping state
    private bool isJumping;
    private bool wasJumping;
    private float jumpTime;
    private float previousBottom;

    public bool IsOnGround => isOnGround;
    bool isOnGround;
    public bool IsAlive => isAlive;
    bool isAlive;

    // Constants for controlling horizontal movement
    private const float MoveAcceleration = 13000.0f;
    private const float MaxMoveSpeed = 1750.0f;
    private const float GroundDragFactor = 0.48f;
    private const float AirDragFactor = 0.58f;

    // Constants for controlling vertical movement
    private const float MaxJumpTime = 0.35f;
    private const float JumpLaunchVelocity = -3500.0f;
    private const float GravityAcceleration = 3400.0f;
    private const float MaxFallSpeed = 550.0f;
    private const float JumpControlPower = 0.14f;
    
    private Rectangle localBounds;
    public Rectangle BoundingRectangle
    {
        get
        {
            int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X; //fix later
            int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

            return new Rectangle(left, top, localBounds.Width, localBounds.Height);
        }
    }

    int frameWidth = 32;
    int frameHeight = 32;
    Point currentFrame = new Point(0, 0);
    Point spriteSize = new Point(1, 10);

    public Player(Level level, Vector2 position)
    {
        this.level = level;
        this.position = position;
        //_texture = Level.content.Load<Texture2D>("2d/ChikBoy_run");
        LoadContent();
        Reset(position);
    }

    public void LoadContent()
    {
        idleAnimation = new Animation(Level.content.Load<Texture2D>("2d/ChikBoy_idle"), 0.1f, true);
        runAnimation = new Animation(Level.content.Load<Texture2D>("2d/ChikBoy_run"), 0.1f, true);
        
        int width = (int)(idleAnimation.FrameWidth);
        int left = (idleAnimation.FrameWidth - width);
        int height = (int)(idleAnimation.FrameHeight);
        int top = idleAnimation.FrameHeight - height;
        localBounds = new Rectangle(left, top, width, height);
    }
    
    public void Reset(Vector2 position)
    {
        Position = position;
        Velocity = Vector2.Zero;
        isAlive = true;
        sprite.PlayAnimation(idleAnimation);
    }

    public void Update(GameTime gameTime, KeyboardState keyboardState, Rectangle window)
    {
        /*if (keyboardState.IsKeyDown(Keys.A) && position.X > 0)
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
        }*/
        GetInput(keyboardState);
        ApplyPhysics(gameTime);
        
        if (IsAlive && IsOnGround)
        {
            if (Math.Abs(Velocity.X) - 0.02f > 0)
            {
                sprite.PlayAnimation(runAnimation);
            }
            else
            {
                sprite.PlayAnimation(idleAnimation);
            }
        }
        
        movement = 0.0f;
        isJumping = false;
    }

    public void ApplyPhysics(GameTime gameTime)
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
    }

    private float DoJump(float velocityY, GameTime gameTime)
    {
        // If the player wants to jump
        if (isJumping)
        {
            // Begin or continue a jump
            if ((!wasJumping && IsOnGround) || jumpTime > 0.0f)
            {
                //if (jumpTime == 0.0f)
                    //jumpSound.Play();

                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                //sprite.PlayAnimation(jumpAnimation);
            }

            // If we are in the ascent of the jump
            if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
            {
                // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
            }
            else
            {
                // Reached the apex of the jump
                jumpTime = 0.0f;
            }
        }
        else
        {
            // Continues not jumping or cancels a jump in progress
            jumpTime = 0.0f;
        }

        wasJumping = isJumping;

        return velocityY;
    }


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
    
    private void HandleCollisions()
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

            // Reset flag to search for ground collision.
            isOnGround = false;

            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; y++)
            {
                for (int x = leftTile; x <= rightTile; x++)
                {
                    // If this tile is collidable,
                    TileCollision collision = Level.GetCollision(x, y);
                    if (collision != TileCollision.Passable)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = Level.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            // Resolve the collision along the shallow axis.
                            if (absDepthY < absDepthX || collision == TileCollision.Platform)
                            {
                                // If we crossed the top of a tile, we are on the ground.
                                if (previousBottom <= tileBounds.Top)
                                    isOnGround = true;

                                // Ignore platforms, unless we are on the ground.
                                if (collision == TileCollision.Impassable || IsOnGround)
                                {
                                    // Resolve the collision along the Y axis.
                                    Position = new Vector2(Position.X, Position.Y + depth.Y);

                                    // Perform further collisions with the new bounds.
                                    bounds = BoundingRectangle;
                                }
                            }
                            else if (collision == TileCollision.Impassable) // Ignore platforms.
                            {
                                // Resolve the collision along the X axis.
                                Position = new Vector2(Position.X + depth.X, Position.Y);

                                // Perform further collisions with the new bounds.
                                bounds = BoundingRectangle;
                            }
                        }
                    }
                }
            }

            // Save the new bounds bottom.
            previousBottom = bounds.Bottom;
        }
    
    public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
    {
        /*_spriteBatch.Draw(_texture, position,
            new Rectangle(currentFrame.X * frameWidth,
                currentFrame.Y * frameHeight,
                frameWidth, frameHeight),
            Color.White, 0, Vector2.Zero,
            1, SpriteEffects.None, 0);*/
        
        // Flip the sprite to face the way we are moving.
        if (Velocity.X > 0)
            flip = SpriteEffects.FlipHorizontally;
        else if (Velocity.X < 0)
            flip = SpriteEffects.None;

        // Draw that sprite.
        sprite.Draw(gameTime, _spriteBatch, Position, flip);
    }
}