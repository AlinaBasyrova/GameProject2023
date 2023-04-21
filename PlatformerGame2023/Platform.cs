using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlatformerGame2023;

public class Platform
{
    private readonly Level level;
    Texture2D _texture;
    Vector2 position;
    
    int frameWidth = 32;
    int frameHeight = 32;
    Point currentFrame = new Point(0, 0);
    Point spriteSize = new Point(1, 10);


    public Platform(Level level, Vector2 position)
    {
        this.level = level;
        this.position = position;
        _texture = Level.content.Load<Texture2D>("2d/Dungeon Ruins Tileset/Dungeon Ruins Tileset/Dungeon Ruins Tileset Night");
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