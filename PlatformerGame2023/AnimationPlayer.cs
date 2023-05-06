using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlatformerGame2023;

public class AnimationPlayer
{
    public Animation Animation => animation;
    Animation animation;
    
    public int FrameIndex => frameIndex;
    int frameIndex;
    
    private float time;

    public Vector2 Origin => new Vector2(Animation.FrameWidth, Animation.FrameHeight);
    
    public void PlayAnimation(Animation animation)
    {
        // If this animation is already running, do not restart it.
        if (Animation == animation)
            return;

        // Start the new animation.
        this.animation = animation;
        this.frameIndex = 0;
        this.time = 0.0f;
    }
    
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects, Color color)
    {
        if (Animation == null)
            throw new NotSupportedException("No animation is currently playing.");

        // Process passing time.
        time += (float)gameTime.ElapsedGameTime.TotalSeconds;
        while (time > Animation.FrameTime)
        {
            time -= Animation.FrameTime;

            // Advance the frame index; looping or clamping as appropriate.
            if (Animation.IsLooping)
            {
                frameIndex = (frameIndex + 1) % Animation.FrameCount;
            }
            else
            {
                frameIndex = Math.Min(frameIndex + 1, Animation.FrameCount - 1);
            }
        }

        // Calculate the source rectangle of the current frame.
        Rectangle source = new Rectangle(0, FrameIndex * Animation.Texture.Width, Animation.Texture.Width, Animation.Texture.Width);

        // Draw the current frame.
        spriteBatch.Draw(Animation.Texture, position, source, color, 0.0f, Origin, 1.0f, spriteEffects, 0.0f);
    }
}