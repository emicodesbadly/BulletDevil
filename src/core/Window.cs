using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using BulletDevil.Rendering;

namespace BulletDevil.Core;

public sealed class Window : GameWindow
{
    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {

    }

    // Runs immediately after Run() is called
    protected override void OnLoad()
    {
        base.OnLoad();

        // LOAD SHADERS
        Shader.Create("sprite-default");
        Shader.Create("screen");

        // LOAD TEXTURES
        Texture.Create("missing", ".png");
        Texture.Create("missing-red", ".png");
        Texture.Create("missing-green", ".png");
        Texture.Create("missing-blue", ".png");
    }

    // Called when the window is resized
    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
    }

    // Called when the frame is rendered, AFTER OnUpdateFrame()
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        SwapBuffers();
    }
}