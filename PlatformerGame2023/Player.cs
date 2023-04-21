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
        //position.X += speed;
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