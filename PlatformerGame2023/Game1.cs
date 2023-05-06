using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PlatformerGame2023;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Level _level;
    private Matrix globalTransformation;

    Texture2D _texture;


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        Content.RootDirectory = "Content";
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        string levelPath = "Content/Levels/0.txt";
        using (Stream fileStream = TitleContainer.OpenStream(levelPath)) 
            _level = new Level(Services, fileStream);
    }

    protected override void UnloadContent()
    {
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        var window = Window.ClientBounds;
        _level.Update(gameTime, keyboardState, window);
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        _level.Draw(gameTime, _spriteBatch);
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}