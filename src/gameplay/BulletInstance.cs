using BulletDevil.Core;

namespace BulletDevil.Gameplay;

public class BulletInstance
{
    public readonly Transform transform = new();
    public readonly BulletBehavior behavior;

    public float angle;

    public float elapsed;

    public BulletInstance(BulletBehavior behavior, float angle)
    {
        this.behavior = behavior;

        this.angle = angle;
    }
}