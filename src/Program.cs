using BulletDevil.Core;
using OpenTK.Windowing.Desktop;

namespace BulletDevil;

sealed class Program
{
    static void Main()
    {
        GameWindowSettings gameWindowSettings = new()
        {
            UpdateFrequency = 60d
        };

        NativeWindowSettings nativeWindowSettings = new()
        {
            Title = "title",
            //ClientSize = (640, 360),
            ClientSize = (960, 540),
            APIVersion = new(4, 6)
        };

        using Window window = new(gameWindowSettings, nativeWindowSettings);
        window.Run();
    }
}