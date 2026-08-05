using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using BulletDevil.Utilities;

namespace BulletDevil.Rendering;

public sealed class Shader : IDisposable
{
    private readonly int handle;
    public readonly string name;

    public readonly bool valid = true;

    private Shader(string name)
    {
        this.name = name;

        int vert, frag;

        // Read shader source code
        string vertSource = File.ReadAllText("resources/shaders/" + name + ".vert");
        string fragSource = File.ReadAllText("resources/shaders/" + name + ".frag");

        // Create vertex shader
        vert = GL.CreateShader(ShaderType.VertexShader);
		GL.ShaderSource(vert, vertSource);

        // Create fragment shader
        frag = GL.CreateShader(ShaderType.FragmentShader);
		GL.ShaderSource(frag, fragSource);

        // Compile vertex shader & check for errors
        GL.CompileShader(vert);

        GL.GetShader(vert, ShaderParameter.CompileStatus, out int success);
		if (success == 0)
		{
			valid = false;

			string infoLog = GL.GetShaderInfoLog(vert);
            Utils.ThrowWarning(this, "Shader creation aborted! (vertex compilation stage)");
			Console.WriteLine(infoLog);
		}

		// Compile fragment shader & check for errors
		GL.CompileShader(frag);

		GL.GetShader(frag, ShaderParameter.CompileStatus, out success);
		if (success == 0)
		{
			valid = false;

			string infoLog = GL.GetShaderInfoLog(frag);
            Utils.ThrowWarning(this, "Shader creation aborted! (fragment compilation stage)");
			Console.WriteLine(infoLog);
		}

        // Create GPU program & attach our shaders
		handle = GL.CreateProgram();

		GL.AttachShader(handle, vert);
		GL.AttachShader(handle, frag);

		// Link program & check for errors
		GL.LinkProgram(handle);

		GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out success);
		if (success == 0)
		{
			string infoLog = GL.GetProgramInfoLog(handle);
            Utils.ThrowWarning(this, "Shader creation aborted! (program linking stage)");
			Console.WriteLine(infoLog);
		}

		// Detach & delete our shaders, since we no longer need them
		GL.DetachShader(handle, vert);
		GL.DetachShader(handle, frag);
		GL.DeleteShader(vert);
		GL.DeleteShader(frag);
    }

    public static Shader Create(string name)
    {
        Lazy<Shader> lazyShader = new(() => new Shader(name));

        if (!RenderingServer.Instance.shaders.TryAdd(name, lazyShader))
        {
            Utils.ThrowWarning("BulletDevil.Rendering.Shader", $"Shader \'{name}\' could not be created!");

            return null;
        }

        return lazyShader.Value;
    }

    public void Use()
    {
        if (valid)
		{
			GL.UseProgram(handle);
		}
		else
		{
			Utils.ThrowWarning(this, $"Shader is invalid! ({name})");
		}
    }

    #region IDisposable Implementation

    private bool disposed = false;

    private void Dispose(bool disposing)
    {
        if (!disposed)
        {
            GL.DeleteShader(handle);

            disposed = true;
        }
    }

    ~Shader()
    {
        if (!disposed)
        {
            Utils.ThrowError(this, "GPU resource leak! Did you forget to call Dispose()?");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}