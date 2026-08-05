namespace BulletDevil.Gameplay;

public enum BulletBehaviorType
{
    UniformLinear     = 0,
    AcceleratedLinear = 1,
    UniformRadial     = 2,
    AcceleratedRadial = 3,
}

public readonly struct BulletBehavior
{
    public readonly BulletBehaviorType type;
    public readonly float[] data;
}