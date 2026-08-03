using System;
using BulletDevil.Core;
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

    #region IDisposable Implementation

    private bool disposed = false;

    private void Dispose(bool disposing)
    {
        if (!disposed)
        {
            // Dispose of shaders
            shaders.Dispose();
            shaders = null;

            // Dispose of textures
            textures.Dispose();
            textures = null;

            // Dispose of screen
            //screen.Dispose();

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