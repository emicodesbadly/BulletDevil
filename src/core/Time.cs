using System;
using BulletDevil.Utilities;

namespace BulletDevil.Core;

public static class Time
{
    private static object timeSource = null;

    private static float deltaTime;
    public static float DeltaTime => deltaTime;

    private static float time;
    public static float TotalTime => time;

    public static bool SetTimeSource(object source)
    {
        if (timeSource != null)
        {
            Utils.ThrowWarning("BulletDevil.Core.Time", "Time Source is already set!");

            return false;
        }

        timeSource = source;

        return true;
    }

    public static bool UpdateTime(object sender, float delta)
    {
        if (sender != timeSource)
        {
            Utils.ThrowWarning("BulletDevil.Core.Time", "Attempt to update time by object other than the Time Source!");

            return false;
        }

        deltaTime = delta;

        time += delta;

        return true;
    }
}