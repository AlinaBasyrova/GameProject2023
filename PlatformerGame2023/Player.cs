using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Platformer2D;
using PlatformerGame2023.Content._2d;

namespace PlatformerGame2023;


public class Player
{
    private Animation idleAnimation;
    private Animation runAnimation;
    private Animation jumpAnimation;
    private AnimationPlayer sprite = new();
    private SpriteEffects flip = SpriteEffects.None;

    public Attack attack = null;
    public Level level { get; }

    Texture2D _texture;

    public Vector2 Position
    {
        get => position;
        set => position = value;
    }

    protected internal Vector2 position;

    public Vector2 Velocity
    {
        get => velocity;
        set => velocity = value;
    }

    protected Vector2 velocity;
    float speed = 5f;
    protected float movement;
    private Color color = Color.White;

    private bool isJumping;
    private bool wasJumping;
    private float jumpTime;
    private float previousBottom;
    private FaceDirection direction = FaceDirection.Left;

    public bool IsOnGround => isOnGround;
    bool isOnGround;
    public bool IsAlive => isAlive;
    bool isAlive;
    public bool IsHiding => isHiding;
    bool isHiding;
    private bool isHidingB;
    bool isAttacking;
    public bool ISCrouching => isCrouching;
    protected bool isCrouching;

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

    public Player(Level level, Vector2 position)
    {
        this.level = level;
        this.position = position;
        LoadContent();
        Reset(position);
    }

    public void LoadContent()
    {
        idleAnimation = new Animation(Level.content.Load<Texture2D>("2d/ChikBoy_idle"), 0.1f, true, 6);
        runAnimation = new Animation(Level.content.Load<Texture2D>("2d/ChikBoy_run"), 0.1f, true, 10);

        int width = idleAnimation.FrameWidth;
        int left = idleAnimation.FrameWidth - width;
        int height = idleAnimation.FrameHeight;
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
        GetInput(keyboardState);
        ApplyPhysics(gameTime);

        if (IsAlive && IsOnGround)
        {
            if (Math.Abs(Velocity.X) - 0.02f > 0)
                sprite.PlayAnimation(runAnimation);
            else
                sprite.PlayAnimation(idleAnimation);
        }

        if (isAttacking)
        {
            if(direction == FaceDirection.Left)
                attack = new Attack(level, new Vector2(position.X, position.Y));
            else
                attack = new Attack(level, new Vector2(position.X + (int)direction * 60, position.Y));
        }
        else if (attack != null && attack.sprite.IsAnimationEnd)
            attack = null;

        movement = 0.0f;
        isJumping = false;
    }
    


    private void GetInput(KeyboardState keyboardState)
    {
        if (Math.Abs(movement) < 0.5f)
            movement = 0.0f;

        if (keyboardState.IsKeyDown(Keys.Left) ||
            keyboardState.IsKeyDown(Keys.A))
        {
            movement = -1.0f;
            direction = FaceDirection.Left;
        }
        else if (keyboardState.IsKeyDown(Keys.Right) ||
                 keyboardState.IsKeyDown(Keys.D))
        {
            movement = 1.0f;
            direction = FaceDirection.Right;
        }

        isJumping =
            keyboardState.IsKeyDown(Keys.Space) ||
            keyboardState.IsKeyDown(Keys.Up) ||
            keyboardState.IsKeyDown(Keys.W);

        keyboardState.IsKeyDown(Keys.C);


        isAttacking = keyboardState.IsKeyDown(Keys.E);
        isCrouching = keyboardState.IsKeyDown(Keys.S);
        isHidingB = keyboardState.IsKeyDown(Keys.Q);
    }

    public void OnKilled()
    {
        isAlive = false;
        position = new Vector2(1000, 1000);
    }

    public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
    {
        if (Velocity.X < 0)
            flip = SpriteEffects.FlipHorizontally;
        else if (Velocity.X > 0)
            flip = SpriteEffects.None;
        
        if (attack != null)
            attack.Draw(gameTime, _spriteBatch);
        
        if (isHiding)
            color = Color.Black;
        else
            color = Color.White;
        
        sprite.Draw(gameTime, _spriteBatch, Position, flip, color);
    }
    
    
    // Constants for controlling horizontal movement
    const float MoveAcceleration = 13000.0f;
    const float MaxMoveSpeed = 1750.0f;
    const float GroundDragFactor = 0.48f;
    const float AirDragFactor = 0.58f;

    // Constants for controlling vertical movement
    const float MaxJumpTime = 0.1f;
    const float JumpLaunchVelocity = -3500.0f;
    const float GravityAcceleration = 3400.0f;
    const float MaxFallSpeed = 550.0f;
    const float JumpControlPower = 0.2f;
    
    
    public void ApplyPhysics(GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Vector2 previousPosition = position;

        velocity.X += movement * MoveAcceleration * elapsed;
        velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

        velocity.Y = DoJump(velocity.Y, gameTime);

        if (IsOnGround)
            velocity.X *= GroundDragFactor;
        else
            velocity.X *= AirDragFactor;
        if (isCrouching)
            velocity.X *= 0.7f;

        velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

        position += velocity * elapsed;
        position = new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y));

        HandleCollisions(gameTime);

        if (position.X == previousPosition.X)
            velocity.X = 0;

        if (position.Y == previousPosition.Y)
            velocity.Y = 0;
    }
    
    private float DoJump(float velocityY, GameTime gameTime)
    {
        if (isJumping)
        {
            if ((!wasJumping && IsOnGround) || jumpTime > 0.0f)
                jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
            else
                jumpTime = 0.0f;
        }
        else
            jumpTime = 0.0f;

        wasJumping = isJumping;

        return velocityY;
    }
    
    private void HandleCollisions(GameTime gameTime)
    {
        // Get the player's bounding rectangle and find neighboring tiles.
        Rectangle bounds = BoundingRectangle;
        int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
        int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
        int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
        int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

        isOnGround = false;
        isHiding = false;

        for (int y = topTile; y <= bottomTile; y++)
        {
            for (int x = leftTile; x <= rightTile; x++)
            {
                TileCollision collision = Level.GetCollision(x, y);

                if (collision == TileCollision.Hiding && isHidingB)
                {
                    isHiding = true;
                }

                if (collision != TileCollision.Passable && collision != TileCollision.Hiding)
                {
                    // Determine collision depth (with direction) and magnitude.
                    Rectangle tileBounds = Level.GetBounds(x, y);
                    Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);

                    if (depth != Vector2.Zero)
                    {
                        float absDepthX = Math.Abs(depth.X);
                        float absDepthY = Math.Abs(depth.Y);

                        if (absDepthY < absDepthX)
                        {
                            if (previousBottom <= tileBounds.Top)
                                isOnGround = true;

                            if (collision == TileCollision.Impassable || IsOnGround)
                            {
                                Position = new Vector2(Position.X, Position.Y + depth.Y);
                                bounds = BoundingRectangle;
                            }
                        }
                        else if (collision == TileCollision.Impassable)
                        {
                            Position = new Vector2(Position.X + depth.X, Position.Y);
                            bounds = BoundingRectangle;
                        }
                    }
                }
            }
        }

        previousBottom = bounds.Bottom;
    }
}

