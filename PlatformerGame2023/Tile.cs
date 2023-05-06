﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlatformerGame2023;

public enum TileCollision
{
    Passable = 0,
    Impassable = 1,
    Hiding = 2,
}

struct Tile
{
    public Texture2D Texture;
    public TileCollision Collision;

    public const int Width = 40;
    public const int Height = 40;

    public static readonly Vector2 Size = new Vector2(Width, Height);
    
    public Tile(Texture2D texture, TileCollision collision)
    {
        Texture = texture;
        Collision = collision;
    }
}