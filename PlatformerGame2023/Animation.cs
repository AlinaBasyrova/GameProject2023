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
    
    public int FrameCount => frameCount;
    private int frameCount;
    
    public int FrameWidth => Texture.Width;

    public int FrameHeight => Texture.Height/FrameCount;

    public Animation(Texture2D texture, float frameTime, bool isLooping, int frameCount)
    {
        this.texture = texture;
        this.frameCount = frameCount;
        this.frameTime = frameTime;
        this.isLooping = isLooping;
    }
}