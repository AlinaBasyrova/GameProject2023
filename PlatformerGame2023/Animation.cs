using Microsoft.Xna.Framework.Graphics;

namespace PlatformerGame2023;

public class Animation
{
    public Texture2D Texture => texture;
    Texture2D texture;
    
    public float FrameTime => frameTime;
    float frameTime;
    
    public bool IsLooping => isLooping;
    bool isLooping;
    
    public int FrameCount => Texture.Height / FrameHeight; // Assume square frames.
    
    public int FrameWidth => Texture.Width;

    public int FrameHeight => Texture.Width;

    public Animation(Texture2D texture, float frameTime, bool isLooping)
    {
        this.texture = texture;
        this.frameTime = frameTime;
        this.isLooping = isLooping;
    }
}