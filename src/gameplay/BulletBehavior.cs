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

    public readonly float lifetime;
    public readonly float[] data;
    
    // Bullet Behavior constructor:
    //  Type: UniformLinear
    //      -> data[0] = velocity
    //  Type: AcceleratedLinear
    //      -> data[0] = velocity
    //      -> data[1] = acceleration
    //  Type: UniformRadial
    //      -> data[0] = velocity
    //      -> data[1] = angular velocity
    //  Type: AcceleratedRadial
    //      -> data[0] = velocity
    //      -> data[1] = acceleration
    //      -> data[2] = angular velocity
    public BulletBehavior(BulletBehaviorType type, float lifetime, float[] data)
    {
        this.type = type;

        this.lifetime = lifetime;
        this.data     = data;
    }
}