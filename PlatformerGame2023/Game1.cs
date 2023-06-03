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
    private int levelIndex = -1;
    private const int numberOfLevels = 3;
    private Matrix globalTransformation;
    int backbufferWidth, backbufferHeight;
    Vector2 baseScreenSize = new Vector2(800, 480);
    Texture2D backgroundTexture;
    private Texture2D winOverlay;
    private Texture2D diedOverlay;




    Texture2D _texture;


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 800;
        _graphics.PreferredBackBufferHeight = 480;
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Window.AllowUserResizing = true;
        base.Initialize();
    }

    protected override void LoadContent()
    {
        Content.RootDirectory = "Content";
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        backgroundTexture = Content.Load<Texture2D>("2d/Background1");
        winOverlay = Content.Load<Texture2D>("2d/you_win");
        diedOverlay = Content.Load<Texture2D>("2d/you_died");



        ScalePresentationArea();
        LoadNextLevel();
    }

    public void ScalePresentationArea()
    {
        //Work out how much we need to scale our graphics to fill the screen
        backbufferWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
        backbufferHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
        float horScaling = backbufferWidth / baseScreenSize.X;
        float verScaling = backbufferHeight / baseScreenSize.Y;
        Vector3 screenScalingFactor = new Vector3(horScaling, verScaling, 1);
        globalTransformation = Matrix.CreateScale(screenScalingFactor);
        System.Diagnostics.Debug.WriteLine("Screen Size - Width[" +
                                           GraphicsDevice.PresentationParameters.BackBufferWidth + "] Height [" +
                                           GraphicsDevice.PresentationParameters.BackBufferHeight + "]");
    }

    protected override void UnloadContent()
    {
    }

    protected override void Update(GameTime gameTime)
    {
        if (backbufferHeight != GraphicsDevice.PresentationParameters.BackBufferHeight ||
            backbufferWidth != GraphicsDevice.PresentationParameters.BackBufferWidth)
        {
            ScalePresentationArea();
        }

        KeyboardState keyboardState = Keyboard.GetState() ;

        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        bool continuePressed =
            keyboardState.IsKeyDown(Keys.Space)
            || keyboardState.IsKeyDown(Keys.W);
        bool reloadPressed = keyboardState.IsKeyDown(Keys.R);


        // Perform the appropriate action to advance the game and
        // to get the player back to playing.
        if (continuePressed)
        {
            if (!_level.player.IsAlive)
            {
                --levelIndex;
                LoadNextLevel();
            }
            else if (_level.IsExiteReached)
                LoadNextLevel();
        }

        if (reloadPressed)
        {
            --levelIndex;
            LoadNextLevel();
        }
        
        var window = Window.ClientBounds;
        _level.Update(gameTime, keyboardState, window);

        base.Update(gameTime);
    }

    private void LoadNextLevel()
    {
        // move to the next level
        levelIndex = (levelIndex + 1) % numberOfLevels;

        // Unloads the content for the current level before loading the next one.
        if (_level != null)
            _level.Dispose();

        // Load the level.
        string levelPath = string.Format("Content/Levels/{0}.txt", levelIndex);
        using (Stream fileStream = TitleContainer.OpenStream(levelPath))
            _level = new Level(Services, fileStream);
    }


    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, globalTransformation);
        _spriteBatch.Draw(backgroundTexture, new Vector2(0, 0), Color.White);

        _level.Draw(gameTime, _spriteBatch);

        Texture2D status = null;
        if (!_level.player.IsAlive)
        {
            status = diedOverlay;
        }
        else if (_level.IsExiteReached)
        {
            status = winOverlay;
        }

        if (status != null)
        {
            // Draw status message.
            Vector2 statusSize = new Vector2(status.Width, status.Height);
            _spriteBatch.Draw(status, new Vector2(0, 0), Color.White);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}