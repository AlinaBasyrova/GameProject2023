/*using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PlatformerGame2023;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    Texture2D _texture;
    Vector2 position = new Vector2(60, 60);
    float speed = 5f;
    
    int currentTime = 0; // сколько времени прошло
    int period = 50; // частота обновления в миллисекундах

    int frameWidth = 32;
    int frameHeight = 32;
    Point currentFrame = new Point(0, 0);
    Point spriteSize = new Point(1, 10);


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _texture = Content.Load<Texture2D>("2d/ChikBoy_run");

        // TODO: use this.Content to load your game content here
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

        currentTime += gameTime.ElapsedGameTime.Milliseconds;
        if (currentTime > period)
        {
            currentTime -= period;
 
            //position.X += speed;
            if (keyboardState.IsKeyDown(Keys.A) && position.X > 0)
                position.X -= speed;
            if (keyboardState.IsKeyDown(Keys.D) && position.X < Window.ClientBounds.Width - frameWidth)
                position.X += speed;
            if (keyboardState.IsKeyDown(Keys.W) && position.Y > 0)
                position.Y -= speed;
            if (keyboardState.IsKeyDown(Keys.S) && position.Y < Window.ClientBounds.Height - frameHeight)
                position.Y += speed;

            ++currentFrame.Y;
            if (currentFrame.Y >= spriteSize.Y)
            {
                currentFrame.Y = 0;
            }
        }

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // отрисовка спрайта
        _spriteBatch.Begin();
        
        _spriteBatch.Draw(_texture, position,
            new Rectangle(currentFrame.X * frameWidth,
                currentFrame.Y * frameHeight,
                frameWidth, frameHeight),
            Color.White, 0, Vector2.Zero,
            1, SpriteEffects.None, 0);
        
        _spriteBatch.End();
        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}*/