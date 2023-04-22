using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PlatformerGame2023;

public class Level : IDisposable
{
    public int currentTime = 0; // сколько времени прошло
    public int period = 50; // частота обновления в миллисекундах
    public ContentManager Content => content;
    private Tile[,] tiles;
    public Player player { get; set; }
    private Platform platform { get; }
    Vector2 startPosition = new Vector2(60, 60);

    public static ContentManager content;


    public Level(IServiceProvider serviceProvider, Stream fileStream)
    {
        // Create a new content manager to load content used just by this level.
        content = new ContentManager(serviceProvider, "Content");
        LoadTiles(fileStream);
        //player = new Player(this, startPosition);
        //platform = new Platform(this, startPosition);
    }


    public void Update(GameTime gameTime, KeyboardState keyboardState, Rectangle window)
    {
        currentTime += gameTime.ElapsedGameTime.Milliseconds;
        if (currentTime > period)
        {
            currentTime -= period;
            player.Update(gameTime, keyboardState, window);
        }
    }

    public int Width => tiles.GetLength(0);
    public int Height => tiles.GetLength(1);


    private void LoadTiles(Stream fileStream)
    {
        // Load the level and ensure all of the lines are the same length.
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
                    throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                line = reader.ReadLine();
            }
        }

        // Allocate the tile grid.
        tiles = new Tile[width, lines.Count];

        // Loop over every tile position,
        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                // to load each tile.
                char tileType = lines[y][x];
                tiles[x, y] = LoadTile(tileType, x, y);
            }
        }

        // Verify that the level has a beginning.
        if (player == null)
            throw new NotSupportedException("A level must have a starting point.");
    }

    
    private Tile LoadTile(char tileType, int x, int y) => tileType switch
    {
        '.' => new Tile(null, TileCollision.Passable), // Blank space
        '-' => LoadTile("2d/Dungeon Ruins Tileset/Dungeon Ruins Tileset/Dungeon Ruins Tileset Night", TileCollision.Platform), //Platform
        'S' => LoadStartTile(x, y), //Player
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

        //start = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
        var start = new Vector2(x*40, y*40);
        player = new Player(this, start);

        return new Tile(null, TileCollision.Passable);
    }

    public void Dispose() => content.Unload();

    public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
    {
        DrawTiles(_spriteBatch);
        player.Draw(gameTime, _spriteBatch);
        //platform.Draw(gameTime, _spriteBatch);
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
                        new Rectangle(20,20, 40, 40),
                        Color.White);
                }
            }
        }
    }
}