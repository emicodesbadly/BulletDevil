using System;
using BulletDevil.Core;
using BulletDevil.Gameplay;
using BulletDevil.Utilities;

namespace BulletDevil.Rendering;

public sealed class RenderingServer : IDisposable
{
    // Lazy singleton implementation (NOT THREAD-SAFE!!!)
	private static readonly Lazy<RenderingServer> instance = new(() => new RenderingServer());
	public static RenderingServer Instance => instance.Value;

    private RenderingServer()
    {
        shaders  = new();
        textures = new();
    }

    // Graphics resources
    public GraphicsResourceContainer<Shader> shaders;
    public GraphicsResourceContainer<Texture> textures;
    public GraphicsResourceContainer<Bullet> bullets;

    private Screen screen;
    public Screen Screen => screen;

    private Background background;
    public Background Background => background;

    public void CreateScreen(Window window, (int width, int height) targetResolution, float size = 1f)
    {
        screen = new Screen(window, targetResolution, size);
    }

    public void CreateBackground(Window window, string shader, string texture)
    {
        background = new(window, shader, texture);
    }

    #region IDisposable Implementation

    private bool disposed = false;

    private void Dispose(bool disposing)
    {
        if (!disposed)
        {
            // Dispose of screen
            screen.Dispose();
            screen = null;

            // Dispose of background
            background.Dispose();
            background = null;

            // Dispose of bullets
            bullets.Dispose();
            bullets = null;

            // Dispose of shaders
            shaders.Dispose();
            shaders = null;

            // Dispose of textures
            textures.Dispose();
            textures = null;

            disposed = true;
        }
    }

    ~RenderingServer()
    {
        if (!disposed)
        {
            Utils.ThrowError(this, "GPU resource leak! Did you forget to call Dispose()?");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}