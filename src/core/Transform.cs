using OpenTK.Mathematics;
using BulletDevil.Utilities;
using BulletDevil.Rendering;

namespace BulletDevil.Core;

public readonly struct TransformData
{
    public readonly Vector2 position;
    public readonly float rotation;
    public readonly Vector2 scale;

    public TransformData(Vector2 position, float rotation, Vector2 scale)
    {
        this.position = position;
        this.rotation = rotation;
        this.scale    = scale;
    }
}

public sealed class Transform
{
    private Transform parent;
    public Transform Parent => parent;

    private Vector2 position;
    private float rotation;
    private Vector2 scale;

    #region Properties

    public Vector2 Position
    {
        get => position;
        set => position = value;
    }

    public Vector2 LocalPosition
    {
        get
        {
            if (parent == null) return position;

            return position - parent.Position;
        }
        set
        {
            if (parent == null) position = value;
            else position = parent.Position + MathUtils.Rotate(value * parent.Scale, parent.rotation);
        }
    }
    
    public float Rotation
    {
        get => rotation;
        set => rotation = value;
    }

    public float LocalRotation
    {
        get
        {
            if (parent == null) return rotation;

            return rotation - parent.Rotation;
        }
        set
        {
            if (parent == null) rotation = value;
            else rotation = value + parent.Rotation;
        }
    }

    public Vector2 Scale
    {
        get => scale;
        set => scale = value;
    }

    public Vector2 LocalScale
    {
        get
        {
            if (parent == null) return scale;

            return (scale.X / parent.Scale.X, scale.Y / parent.Scale.Y);
        }
        set
        {
            if (parent == null) scale = value;
            else scale = (parent.Scale.X * value.X, parent.Scale.Y * value.Y);
        }
    }

    public Vector2 Up => MathUtils.Rotate(Vector2.UnitY, rotation);

    public TransformData Data => new(Position, Rotation, Scale);

    #endregion

    public Transform()
    {
        position = Vector2.Zero;
        rotation = 0f;
        scale    = Vector2.One;
    }

    public Transform(Vector2 position, float rotation, Vector2 scale)
    {
        this.position = position;
        this.rotation = rotation;
        this.scale    = scale;
    }

    public void SetParent(Transform newParent, bool keepTransform = true)
    {
        if (keepTransform)
        {
            parent = newParent;
        }
        else
        {
            Vector2 localPos   = LocalPosition;
            Vector2 localScale = LocalScale;
            float localRot     = LocalRotation;

            parent = newParent;

            LocalScale    = localScale;
            LocalRotation = localRot;
            LocalPosition = localPos;
        }
    }

    public Matrix4 CalculateTransformationMatrix()
    {
        // First we apply scale
        Matrix4 transformation = Matrix4.CreateScale(Scale.X, Scale.Y, 1.0f);
        
        // Then we apply rotation
        transformation *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation));

        // Then we apply translation
        transformation *= Matrix4.CreateTranslation(Position.X, Position.Y, 0.0f);

        // Lastly we apply the world-to-screen matrix
        transformation *= RenderingServer.Instance.Screen.WorldToScreenMatrix;

        return transformation;
    }
}