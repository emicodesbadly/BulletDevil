using OpenTK.Mathematics;
using System;
using BulletDevil.Core;
using BulletDevil.Utilities;
using OpenTK.Graphics.OpenGL4;

namespace BulletDevil.Rendering;

public sealed class Screen : IDisposable
{
    float[] vertices = [
         1.0f,  1.0f, 1.0f, 1.0f,
         1.0f, -1.0f, 1.0f, 0.0f,
        -1.0f, -1.0f, 0.0f, 0.0f,
        -1.0f,  1.0f, 0.0f, 1.0f,
    ];

    uint[] indices = [
        0, 1, 2,
        2, 3, 0
    ];

    private readonly int FBO;
    private readonly int renderTexture;

    private readonly int VBO, VAO, EBO;

    private readonly Window window;

    private (int width, int height) resolution;
    public (int width, int height) Resolution => resolution;

    public float AspectRatio => (float)resolution.width / (float)resolution.height;

    // Distance from the center to the top of the screen, in world units
    private float size;
    public float Size => size;

    private Matrix4 wts;
    public Matrix4 WorldToScreenMatrix => wts;

    public Screen(Window window, (int width, int height) resolution, float size = 1f)
    {
        // Set window & screen aspect ratio
        this.window = window;
        this.resolution = resolution;

        // Set screen size
        this.size = size;

        // Set world-to-screen matrix
        wts = CalculateWorldToScreen();

        // Create & bind frame buffer
        FBO = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);

        // Create the texture we'll render to & set its parameters
        renderTexture = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, renderTexture);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, resolution.width, resolution.height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        // Wrapping determines how the texture should behave when given UV coordinates outside the [0,1] range
        // S is the X axis, T is the Y axis
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        // Attach the render texture
        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, renderTexture, 0);

        // Check for frame buffer errors
        if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
        {
            Utils.ThrowError(this, "Frame buffer is incomplete!");
        }

        // Create & bind vertex buffer, & upload data to it
        VBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

        // Create & bind vertex array, & set vertex attributes
        VAO = GL.GenVertexArray();
        GL.BindVertexArray(VAO);

        // Vertex positions
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        // Vertex UVs
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        // Create & bind the element buffer, & upload data to it
        EBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

        OnWindowResized((window.ClientSize.X, window.ClientSize.Y));
    }
    
    public void OnWindowResized((int width, int height) windowResolution)
    {
        float windowAspect = (float)windowResolution.width / (float)windowResolution.height;

        if (windowAspect > AspectRatio)
        {
            vertices[0]  =  (1 / windowAspect) * AspectRatio;
            vertices[1]  =  1.0f;

            vertices[4]  =  (1 / windowAspect) * AspectRatio;
            vertices[5]  = -1.0f;

            vertices[8]  = -(1 / windowAspect) * AspectRatio;
            vertices[9]  = -1.0f;

            vertices[12] = -(1 / windowAspect) * AspectRatio;
            vertices[13] =  1.0f;

        }
        else if (windowAspect < AspectRatio)
        {
            vertices[0]  =  1.0f; 
            vertices[1]  =  1 / AspectRatio * windowAspect;

            vertices[4]  =  1.0f;
            vertices[5]  = -1 / AspectRatio * windowAspect;

            vertices[8]  = -1.0f;
            vertices[9]  = -1 / AspectRatio * windowAspect;

            vertices[12] = -1.0f;
            vertices[13] =  1 / AspectRatio * windowAspect;
        }
        else
        {
            vertices[0]  =  1.0f; 
            vertices[1]  =  1.0f;

            vertices[4]  =  1.0f;
            vertices[5]  = -1.0f;

            vertices[8]  = -1.0f;
            vertices[9]  = -1.0f;

            vertices[12] = -1.0f;
            vertices[13] =  1.0f;
        }

        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    private Matrix4 CalculateWorldToScreen()
    {
        return new Matrix4
        (
            (1f / (size * AspectRatio), 0.0f, 0.0f, 0.0f),
            (0.0f, 1f / size, 0.0f, 0.0f),
            (0.0f, 0.0f, 1.0f, 0.0f),
            (0.0f, 0.0f, 0.0f, 1.0f)
        );
    }

    public void SetSize(float size)
    {
        if (size <= 0f)
        {
            Utils.ThrowWarning(this, "Screen size must be greater than 0!");

            return;
        }

        this.size = size;

        wts = CalculateWorldToScreen();
    }

    public void BindFBO()
    {
        // Bind FBO
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);

        // Change viewport to match
        GL.Viewport(0, 0, resolution.width, resolution.height);

        GL.ClearColor(Color4.HotPink);
        GL.Clear(ClearBufferMask.ColorBufferBit);
    }

    public void Render()
    {
        // Bind default frame buffer
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        // Change viewport to match
        GL.Viewport(0, 0, window.ClientSize.X, window.ClientSize.Y);

        GL.ClearColor(Color4.Black);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        // Bind VAO
        GL.BindVertexArray(VAO);

        // Activate shader & texture
        RenderingServer.Instance.shaders.GetResource("screen").Use();
        
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, renderTexture);

        // Draw screen
        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

        // Unbind VAO
        GL.BindVertexArray(0);
    }

    #region  Dispose

    // IDisposable implementation
    private bool disposed = false;

    private void Dispose(bool disposing)
    {
        if (!disposed)
        {
            // Unbind & delete VBO, instance VBO & EBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffers(2, [VBO, EBO]);

            // Unbind & delete vertex array
            GL.BindVertexArray(0);
            GL.DeleteVertexArray(VAO);

            // Dispose of render texture
            GL.DeleteTexture(renderTexture);

            // Dispose of frame buffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.DeleteFramebuffer(FBO);

            disposed = true;
        }
    }

    ~Screen()
    {
        if (!disposed)
        {
            Utils.ThrowError(this, "GPU Resource leak! Did you forget to call Dispose()?");
        }
    }

    // MUST be called when the screen is no longer needed!
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}