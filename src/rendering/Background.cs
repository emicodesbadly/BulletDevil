using System;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using BulletDevil.Core;
using BulletDevil.Utilities;

namespace BulletDevil.Rendering;

public sealed class Background : IDisposable
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

    private readonly int VBO, VAO, EBO;

    private Shader shader;
    private Texture texture;

    private readonly Window window;

    private (int width, int height) resolution;
    public (int width, int height) Resolution => resolution;

    public float AspectRatio => (float)resolution.width / (float)resolution.height;

    public Vector2 TopRight   => (vertices[0], vertices[1]);
    public Vector2 BottomLeft => (vertices[8], vertices[9]);

    public Background(Window window, string shader, string texture)
    {
        this.window = window;

        this.shader  = RenderingServer.Instance.shaders.GetResource(shader);
        this.texture = RenderingServer.Instance.textures.GetResource(texture);

        resolution = (this.texture.size.X, this.texture.size.Y);

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
        shader.Use();
        texture.Use(TextureUnit.Texture0);

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
            // Unbind & delete VBO & EBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffers(2, [VBO, EBO]);

            // Unbind & delete vertex array
            GL.BindVertexArray(0);
            GL.DeleteVertexArray(VAO);

            disposed = true;
        }
    }

    ~Background()
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