using OpenTK.Mathematics;

namespace BulletDevil.Utilities;

public static class MathUtils
{
    public static Vector2 Rotate(Vector2 v, float theta)
    {
        float radians = MathHelper.DegreesToRadians(theta);

        float sin = (float)MathHelper.Sin(radians);
        float cos = (float)MathHelper.Cos(radians);

        Vector2 a = (cos, sin);
        Vector2 b = (-sin, cos);

        return v.X * a + v.Y * b;
    }
}