using BulletDevil.Core;

namespace BulletDevil.Gameplay;

public class BulletInstance
{
    public readonly Transform transform = new();
    public readonly BulletBehavior behavior;
}