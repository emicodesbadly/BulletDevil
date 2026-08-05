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

    // Runs immediately after Run() is called
    protected override void OnLoad()
    {
        base.OnLoad();

        // SET AS TIME SOURCE
        Time.SetTimeSource(this);

        // LOAD SHADERS
        Shader.Create("sprite-default");
        Shader.Create("screen");

        // LOAD TEXTURES
        Texture.Create("missing", ".png");
        Texture.Create("missing-red", ".png");
        Texture.Create("missing-green", ".png");
        Texture.Create("missing-blue", ".png");
        Texture.Create("missing-background-16x9", ".png");

        // LOAD BULLETS

        // CREATE BACKGROUND
        RenderingServer.Instance.CreateBackground(this, "screen", "missing-background-16x9");

        // CREATE SCREEN
        RenderingServer.Instance.CreateScreen(this, (810, 1080), 5f);
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        RenderingServer.Instance.Dispose();
    }

    // Called when the window is resized
    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);

        RenderingServer.Instance.Background.OnWindowResized((e.Width, e.Height));

        RenderingServer.Instance.Screen.OnWindowResized(RenderingServer.Instance.Background.Resolution);
    }

    // Called when the frame starts, BEFORE OnRenderFrame()
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        // Set delta time
        Time.SetDeltaTime(this, (float)e.Time);

        // Update bullets
        GameServer.Instance.UpdateBullets();
    }

    // Called when the frame is rendered, AFTER OnUpdateFrame()
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        // Bind screen FBO
        RenderingServer.Instance.Screen.BindFBO();

        /* Render game here */

        // Render the background
        RenderingServer.Instance.Background.Render();

        // Render the screen
		RenderingServer.Instance.Screen.Render();


        SwapBuffers();
    }
}