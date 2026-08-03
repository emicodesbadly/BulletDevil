using System;
using OpenTK.Graphics.OpenGL4;
using BulletDevil.Core;
using BulletDevil.Utilities;

namespace BulletDevil.Rendering;

public abstract class Sprite : IDisposable
{
    public readonly Transform transform = new();

    protected readonly float[] vertices = [
    //	 X       Y      UV(X) UV(Y)
            0.25f,  0.25f, 1.0f, 1.0f,
            0.25f, -0.25f, 1.0f, 0.0f,
        -0.25f, -0.25f, 0.0f, 0.0f,
        -0.25f,  0.25f, 0.0f, 1.0f
    ];

    protected readonly uint[] indices = [
        0, 1, 2, // bottom triangle
        2, 3, 0  // top triangle
    ];

    protected readonly int VBO, VAO, EBO;

    protected Shader shader;
    protected Texture texture;

    protected Sprite()
    {
        // Create & bind vertex buffer, & upload data to it
        VBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

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
    }

    public abstract void Render();

    #region IDisposable Implementation

    protected bool disposed = false;

    protected abstract void Dispose(bool disposing);

    ~Sprite()
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