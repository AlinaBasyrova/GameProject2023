using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlatformerGame2023;

public enum TileCollision
{
    Passable = 0,
    Impassable = 1,
    Hiding = 2,
    Ladder = 3,
}

struct Tile
{
    public Texture2D Texture;
    public TileCollision Collision;

    public const int Width = 32;
    public const int Height = 32;

    public static readonly Vector2 Size = new(Width, Height);
    
    public Tile(Texture2D texture, TileCollision collision)
    {
        Texture = texture;
        Collision = collision;
    }
}