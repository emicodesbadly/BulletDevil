using OpenTK.Mathematics;

namespace BulletDevil.Core;

public enum ColliderType
{
    Circle = 0,
    Box    = 1,
}

public sealed class Collider
{
    public readonly ColliderType type;
    public readonly float[] data;

    private Collider(ColliderType type, float[] data)
    {
        this.type = type;
        this.data = data;
    }

    public static Collider CreateCircleCollider(float radius)
    {
        return new(ColliderType.Circle, [radius]);
    }

    public static Collider CreateBoxCollider(Vector2 size)
    {
        return new(ColliderType.Box, [size.X, size.Y]);
    }
}