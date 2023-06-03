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
    public bool IsAnimationEnd => isAnimationEnd;
    private bool isAnimationEnd;

    public Vector2 Origin => new Vector2(Animation.FrameWidth, Animation.FrameHeight);
    
    public void PlayAnimation(Animation animation)
    {
        if (Animation == animation)
            return;

        this.animation = animation;
        this.frameIndex = 0;
        this.time = 0.0f;
        isAnimationEnd = false;
    }
    
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects, Color color)
    {
        if (Animation == null)
            throw new NotSupportedException("No animation is currently playing.");

        time += (float)gameTime.ElapsedGameTime.TotalSeconds;
        while (time > Animation.FrameTime)
        {
            time -= Animation.FrameTime;

            if (Animation.IsLooping)
            {
                frameIndex = (frameIndex + 1) % Animation.FrameCount;
            }
            else
            {
                frameIndex = Math.Min(frameIndex + 1, Animation.FrameCount - 1);
            }
            
            if (frameIndex == Animation.FrameCount - 1)
                isAnimationEnd = true;
            else
            {
                isAnimationEnd = false;
            }
        }

        Rectangle source = new Rectangle(0, FrameIndex * Animation.FrameHeight, Animation.FrameWidth, Animation.FrameHeight);
        spriteBatch.Draw(Animation.Texture, position, source, color, 0.0f, Origin, 1.0f, spriteEffects, 0.0f);
    }
}