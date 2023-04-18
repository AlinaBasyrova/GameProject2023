using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PlatformerGame2023;

public class Level : IDisposable
{
    Texture2D _texture;
    Vector2 position = new Vector2(60, 60);
    float speed = 5f;

    public int currentTime = 0; // сколько времени прошло
    public int period = 50; // частота обновления в миллисекундах

    int frameWidth = 32;
    int frameHeight = 32;
    Point currentFrame = new Point(0, 0);
    Point spriteSize = new Point(1, 10);

    // Level content.        
    public ContentManager Content
    {
        get { return content; }
    }

    ContentManager content;

    /// <summary>
    /// Constructs a new level.
    /// </summary>
    /// <param name="serviceProvider">
    /// The service provider that will be used to construct a ContentManager.
    /// </param>
    /// <param name="fileStream">
    /// A stream containing the tile data.
    /// </param>
    public Level(IServiceProvider serviceProvider)
    {
        // Create a new content manager to load content used just by this level.
        content = new ContentManager(serviceProvider, "Content");
        _texture = Content.Load<Texture2D>("2d/ChikBoy_run");
    }

    public void Update(GameTime gameTime, KeyboardState keyboardState, Rectangle window)
    {
        currentTime += gameTime.ElapsedGameTime.Milliseconds;
        if (currentTime > period)
        {
            currentTime -= period;

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
        
    }

    public void Dispose()
    {
        content.Unload();
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