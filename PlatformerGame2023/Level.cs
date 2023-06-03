using System;
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
    public Vector2 start = new(40, 60);
    private Point? finishPoint = null;


    public ContentManager Content => content;
    private static Tile[,] tiles;
    public List<Enemy> enemies = new();
    public List<Bullet> bullets = new();

    public Player player { get; set; }
    public static ContentManager content;
    public bool IsExiteReached => isExiteReached;
    bool isExiteReached;


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
            
            for (var i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];
                enemy.Update(gameTime);
                if (player.attack != null && enemy.BoundingRectangle.Intersects(player.attack.BoundingRectangle))
                {
                    enemies.RemoveAt(i--);
                    continue;
                }
                if (enemy.BoundingRectangle.Intersects(player.BoundingRectangle))
                {

                    if (player.IsHiding)
                        continue;
                    player.OnKilled();
                }
            }

            for (var i = 0; i < bullets.Count; i++)
            {
                var bullet = bullets[i];
                bullet.Update(gameTime);
                var bounds = bullet.BoundingRectangle;
                
                int x = (int)Math.Floor((float)bounds.Left / Tile.Width);
                int y = (int)Math.Floor((float)bounds.Top / Tile.Height);
                if (bounds.Intersects(player.BoundingRectangle))
                {
                    player.OnKilled();
                    bullets.RemoveAt(i--);
                }
                if (GetCollision(x, y) == TileCollision.Impassable)
                {
                    bullets.RemoveAt(i--);
                }
            }


            if (player.IsAlive &&
                player.IsOnGround &&
                player.BoundingRectangle.Contains((Point)finishPoint))
            {
                isExiteReached = true;
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
        '-' => LoadTile("2d/Tiles",
            TileCollision.Impassable),
        'H' => LoadTile("2d/Props-01",
            TileCollision.Hiding),
        'L' => LoadTile("2d/Tiles",
            TileCollision.Ladder),
        'S' => LoadStartTile(x, y), //Player
        'F' => LoadExitTile(x, y),
        '1' => LoadEnemyTile(x, y, 1),
        '2' => LoadEnemyTile(x, y, 2),

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

    private Tile LoadExitTile(int x, int y)
    {
        if (finishPoint != null)
            throw new NotSupportedException("A level may only have one exit.");

        finishPoint = GetBounds(x, y).Center;

        return LoadTile("2d/Props-01",
            TileCollision.Passable);
    }

    private Tile LoadEnemyTile(int x, int y, int type)
    {
        Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
        if (type == 1)
            enemies.Add(new Enemy(this, position, 1));
        else
        {
            enemies.Add(new Enemy(this, position, 2));
        }

        return new Tile(null, TileCollision.Passable);
    }

    public void CreateBullet(Vector2 position, FaceDirection direction)
    {
        Bullet bullet = new Bullet(this, position, direction);
        bullets.Add(bullet);
    }

    public static TileCollision GetCollision(int x, int y)
    {
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
        foreach (Bullet bullet in bullets)
            bullet.Draw(gameTime, _spriteBatch);
    }

    private void DrawTiles(SpriteBatch spriteBatch)
    {
        // For each tile position
        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                Texture2D texture = tiles[x, y].Texture;
                if (texture != null)
                {
                    Vector2 position = new Vector2(x, y) * Tile.Size;

                    if (tiles[x, y].Collision == TileCollision.Hiding)
                    {
                        spriteBatch.Draw(texture, position,
                            new Rectangle(50, 0, Tile.Width, Tile.Height),
                            Color.White);
                    }
                    else if (tiles[x, y].Collision == TileCollision.Ladder)
                    {
                        spriteBatch.Draw(texture, position,
                            new Rectangle(0, 0, Tile.Width, Tile.Height),
                            Color.WhiteSmoke);
                    }
                    else
                        spriteBatch.Draw(texture, position,
                            new Rectangle(0, 32, Tile.Width, Tile.Height),
                            Color.White);
                }
            }
        }
    }
}