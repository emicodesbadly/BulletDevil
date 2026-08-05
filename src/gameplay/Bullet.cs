using System;
using OpenTK.Mathematics;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using BulletDevil.Rendering;
using BulletDevil.Utilities;
using BulletDevil.Core;

namespace BulletDevil.Gameplay;

public class Bullet : Sprite
{
    protected readonly int instVBO;

    private List<BulletInstance> instances = [];

    private Bullet(string shader, string texture, Vector2 size)
        : base(shader, texture, size)
    {
        // Also create & bind instance vertex buffer
        instVBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, instVBO);

        // Set up the transform vertex attribute
        // It is a mat4, so it takes 4 locations

        // Column 0
        GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, 16 * sizeof(float), 0);
        GL.EnableVertexAttribArray(2);
        GL.VertexAttribDivisor(2, 1);

        // Column 1
        GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, 16 * sizeof(float), 4 * sizeof(float));
        GL.EnableVertexAttribArray(3);
        GL.VertexAttribDivisor(3, 1);

        // Column 2
        GL.VertexAttribPointer(4, 4, VertexAttribPointerType.Float, false, 16 * sizeof(float), 8 * sizeof(float));
        GL.EnableVertexAttribArray(4);
        GL.VertexAttribDivisor(4, 1);

        // Column 3
        GL.VertexAttribPointer(5, 4, VertexAttribPointerType.Float, false, 16 * sizeof(float), 12 * sizeof(float));
        GL.EnableVertexAttribArray(5);
        GL.VertexAttribDivisor(5, 1);

        // Unbind buffers
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }

    public static Bullet Create(string name, string shader, string texture, Vector2 size)
    {
        Lazy<Bullet> lazyBullet = new Lazy<Bullet>(() => new Bullet(shader, texture, size));

        if (!RenderingServer.Instance.bullets.TryAdd(name, lazyBullet))
        {
            Utils.ThrowWarning("ProjectileTK.Gameplay.Bullet", $"Bullet \'{name}\' could not be created!");

            return null;
        }

        return lazyBullet.Value;
    }

    public void Fire(TransformData transformData, BulletBehavior behavior, float angle)
    {
        BulletInstance instance = new(behavior, angle);

        instance.transform.Position = transformData.position;
        instance.transform.Rotation = transformData.rotation;
        instance.transform.Scale    = transformData.scale;

        instances.Add(instance);
    }

    public void Update()
    {
        BulletInstance instance;
        for (int i = instances.Count - 1; i >= 0; i--)
        {
            instance = instances[i];

            if (instance.elapsed >= instance.behavior.lifetime)
            {
                instances.Remove(instance);

                continue;
            }

            switch (instance.behavior.type)
            {
                case BulletBehaviorType.UniformLinear:
                    instance.transform.Position += MathUtils.Rotate(Vector2.UnitY, instance.angle) * instance.behavior.data[0] * Time.DeltaTime;

                    break;
                case BulletBehaviorType.AcceleratedLinear:
                    instance.transform.Position +=
                        MathUtils.Rotate(Vector2.UnitY, instance.angle) *
                        ((instance.behavior.data[0] + instance.behavior.data[1] * instance.elapsed) * Time.DeltaTime +
                        0.5f * instance.behavior.data[1] * Time.DeltaTime * Time.DeltaTime);

                    break;
                case BulletBehaviorType.UniformRadial:
                    instance.transform.Position += MathUtils.Rotate(Vector2.UnitY, instance.angle) * instance.behavior.data[0] * Time.DeltaTime;

                    instance.angle += instance.behavior.data[1] * Time.DeltaTime;

                    break;
                case BulletBehaviorType.AcceleratedRadial:
                    instance.transform.Position +=
                        MathUtils.Rotate(Vector2.UnitY, instance.angle) *
                        ((instance.behavior.data[0] + instance.behavior.data[1] * instance.elapsed) * Time.DeltaTime +
                        0.5f * instance.behavior.data[1] * Time.DeltaTime * Time.DeltaTime);

                    instance.angle += instance.behavior.data[2] * Time.DeltaTime;

                    break;
            }

            instance.elapsed += Time.DeltaTime;
        }
    }

    public override void Render()
    {
        // If this sprite has not been instantiated, skip
        if (instances == null || instances.Count <= 0)
        {
            return;
        }

        // Gather instance data
        float[] instanceData = new float[16 * instances.Count];
        for (int i = 0; i < instances.Count; i++)
        {
            // Get instance transformation matrix
            Matrix4 transform = instances[i].transform.CalculateTransformationMatrix();

            // Column 0
            instanceData[16 * i + 0] = transform.Row0.X;
            instanceData[16 * i + 1] = transform.Row0.Y;
            instanceData[16 * i + 2] = transform.Row0.Z;
            instanceData[16 * i + 3] = transform.Row0.W;

            // Column 1
            instanceData[16 * i + 4] = transform.Row1.X;
            instanceData[16 * i + 5] = transform.Row1.Y;
            instanceData[16 * i + 6] = transform.Row1.Z;
            instanceData[16 * i + 7] = transform.Row1.W;

            // Column 2
            instanceData[16 * i +  8] = transform.Row2.X;
            instanceData[16 * i +  9] = transform.Row2.Y;
            instanceData[16 * i + 10] = transform.Row2.Z;
            instanceData[16 * i + 11] = transform.Row2.W;

            // Column 3
            instanceData[16 * i + 12] = transform.Row3.X;
            instanceData[16 * i + 13] = transform.Row3.Y;
            instanceData[16 * i + 14] = transform.Row3.Z;
            instanceData[16 * i + 15] = transform.Row3.W;
        }

        // Bind instance data buffer & upload instance data to it
        GL.BindBuffer(BufferTarget.ArrayBuffer, instVBO);
        GL.BufferData(BufferTarget.ArrayBuffer, instanceData.Length * sizeof(float), instanceData, BufferUsageHint.StreamDraw);

        // Bind VAO
        GL.BindVertexArray(VAO);

        // Activate shader & texture
        shader.Use();
        texture.Use(TextureUnit.Texture0);

        // Draw sprite instances
        GL.DrawElementsInstanced(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0, instances.Count);

        // Unbind buffers & arrays
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }

    #region IDisposable Implementation

    protected override void Dispose(bool disposing)
    {
        if (!disposed)
        {

        }
    }

    #endregion
}