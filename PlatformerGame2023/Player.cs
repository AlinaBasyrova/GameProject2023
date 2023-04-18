using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PlatformerGame2023;

public class Player
{
    public Level level { get; }

    //int currentTime = 0; // сколько времени прошло
    //int period = 50; // частота обновления в миллисекундах
    
    public Player(Level level, Vector2 position)
    {
        this.level = level;

        //LoadContent();

        //Reset(position);
    }
    
    public void Update(GameTime gameTime, KeyboardState keyboardState, Rectangle window)
    {
        level.currentTime += gameTime.ElapsedGameTime.Milliseconds;
        if (level.currentTime > level.period)
        {
            level.currentTime -= level.period;
            
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
}