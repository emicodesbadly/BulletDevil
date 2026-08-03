using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using BulletDevil.Rendering;
using BulletDevil.Gameplay;

namespace BulletDevil.Core;

public sealed class Window : GameWindow
{
    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {

    }

    Bullet test;

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

        // LOAD BULLETS
        test = Bullet.Create("sprite-default", "missing", Vector2.One);

        // CREATE SCREEN
        //RenderingServer.Instance.CreateScreen(this, (1920, 1080), 5f);

        // DEBUG BULLETS
        test.Instantiate(Vector2.Zero, 0f, Vector2.One);
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        RenderingServer.Instance.Dispose();

        test.Dispose();
    }

    // Called when the window is resized
    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);

        //RenderingServer.Instance.Screen.OnWindowResized((e.Width, e.Height));
    }

    // Called when the frame is rendered, AFTER OnUpdateFrame()
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        // Bind screen FBO
        //RenderingServer.Instance.Screen.BindFBO();

        test.Render();

        // Render the screen
		//RenderingServer.Instance.Screen.Render();

        SwapBuffers();
    }
}