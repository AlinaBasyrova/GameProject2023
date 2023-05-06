﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Platformer2D;

namespace PlatformerGame2023;

public class Level : IDisposable
{
    public int currentTime;
    public int period = 50;
    public Vector2 start = new Vector2(40, 60);

    public ContentManager Content => content;
    private static Tile[,] tiles;
    private List<Enemy> enemies = new();

    public Player player { get; set; }
    public static ContentManager content;


    public Level(IServiceProvider serviceProvider, Stream fileStream)
    {
        content = new ContentManager(serviceProvider, "Content");
        LoadTiles(fileStream);
    }


    public void Update(GameTime gameTime, KeyboardState keyboardState, Rectangle window)
    {
        currentTime += gameTime.ElapsedGameTime.Milliseconds;
        if (currentTime > period)
        {
            currentTime -= period;
            player.Update(gameTime, keyboardState, window);
            foreach (Enemy enemy in enemies)
            {
                enemy.Update(gameTime);
                if (!player.IsHiding && enemy.BoundingRectangle.Intersects(player.BoundingRectangle))
                {
                    player.OnKilled();
                    player.Reset(start);
                }
            }
        }
    }

    public static int Width => tiles.GetLength(0);
    public static int Height => tiles.GetLength(1);


    private void LoadTiles(Stream fileStream)
    {
        int width;
        List<string> lines = new List<string>();
        using (StreamReader reader = new StreamReader(fileStream))
        {
            string line = reader.ReadLine();
            width = line.Length;
            while (line != null)
            {
                lines.Add(line);
                if (line.Length != width)
                    throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.",
                        lines.Count));
                line = reader.ReadLine();
            }
        }

        tiles = new Tile[width, lines.Count];

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                char tileType = lines[y][x];
                tiles[x, y] = LoadTile(tileType, x, y);
            }
        }

        if (player == null)
            throw new NotSupportedException("A level must have a starting point.");
    }


    private Tile LoadTile(char tileType, int x, int y) => tileType switch
    {
        '.' => new Tile(null, TileCollision.Passable), // Blank space
        '-' => LoadTile("2d/Dungeon Ruins Tileset/Dungeon Ruins Tileset/Dungeon Ruins Tileset Night",
            TileCollision.Impassable),
        'H' => LoadTile("2d/Dungeon Ruins Tileset/Dungeon Ruins Tileset/Dungeon Ruins Tileset Day",
            TileCollision.Hiding),
        'S' => LoadStartTile(x, y), //Player
        'E' => LoadEnemyTile(x, y, "enemy"),
        _ => throw new ArgumentOutOfRangeException(nameof(tileType), tileType, null)
    };

    private Tile LoadTile(string name, TileCollision collision)
    {
        return new Tile(Content.Load<Texture2D>(name), collision);
    }

    private Tile LoadStartTile(int x, int y)
    {
        if (player != null)
            throw new NotSupportedException("A level may only have one starting point.");

        start = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
        player = new Player(this, start);

        return new Tile(null, TileCollision.Passable);
    }

    private Tile LoadEnemyTile(int x, int y, string spriteSet)
    {
        Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
        enemies.Add(new Enemy(this, position, spriteSet));

        return new Tile(null, TileCollision.Passable);
    }

    public static TileCollision GetCollision(int x, int y)
    {
        // Prevent escaping past the level ends.
        if (x < 0 || x >= Width)
            return TileCollision.Impassable;
        if (y < 0 || y >= Height)
            return TileCollision.Impassable;

        return tiles[x, y].Collision;
    }

    public static Rectangle GetBounds(int x, int y)
    {
        return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
    }

    public void Dispose() => content.Unload();

    public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
    {
        DrawTiles(_spriteBatch);
        player.Draw(gameTime, _spriteBatch);
        foreach (Enemy enemy in enemies)
            enemy.Draw(gameTime, _spriteBatch);
    }

    private void DrawTiles(SpriteBatch spriteBatch)
    {
        // For each tile position
        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                // If there is a visible tile in that position
                Texture2D texture = tiles[x, y].Texture;
                if (texture != null)
                {
                    // Draw it in screen space.
                    Vector2 position = new Vector2(x, y) * Tile.Size;
                    spriteBatch.Draw(texture, position,
                        new Rectangle(20, 20, 40, 40),
                        Color.White);
                }
            }
        }
    }
}