using System;
using System.IO;
using BulletDevil.Utilities;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace BulletDevil.Rendering;

public sealed class Texture : IDisposable
{
    private readonly int handle;

    public readonly string name;

    private Texture(string name, string fileExtension)
    {
        this.name = name;

        // Assemble path to image
        string path = "resources/textures/" + name + fileExtension;

        // Generate handle
        handle = GL.GenTexture();

        // Bind the handle
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, handle);

        // OpenGL has its texture origin on the bottom left instead of the top left
        // We must tell StbImageSharp to vertically flip images on load to corect for this
        StbImage.stbi_set_flip_vertically_on_load(1);

        // Open a stream to the image and pass it to StbImageSharp
        using (Stream stream = File.OpenRead(path))
        {
            ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            // Now that our pixels are prepared, it's time to generate a texture. We do this with GL.TexImage2D.
            // Arguments:
            //   The type of texture we're generating. There are various different types of textures, but the only one we need right now is Texture2D.
            //   Level of detail. We can use this to start from a smaller mipmap (if we want), but we don't need to do that, so leave it at 0.
            //   Target format of the pixels. This is the format OpenGL will store our image with.
            //   Width of the image
            //   Height of the image.
            //   Border of the image. This must always be 0; it's a legacy parameter that Khronos never got rid of.
            //   The format of the pixels, explained above. Since we loaded the pixels as RGBA earlier, we need to use PixelFormat.Rgba.
            //   Data type of the pixels.
            //   And finally, the actual pixels.
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        }

        // MinFilter determines how to filter the image when downscaling it
        // MagFilter determines how to filter the image when upscaling it
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        
        // Wrapping determines how the texture should behave when given UV coordinates outside the [0,1] range
        // S is the X axis, T is the Y axis
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        // Optionally, generate mipmaps
        // GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
    }

    // Takes 2 arguments: name, file extension
    public static Texture Create(string name, string fileExtension)
    {
        Lazy<Texture> lazyTexture = new(() => new Texture(name, fileExtension));

        if (RenderingServer.Instance.textures.TryAdd(name, lazyTexture))
        {
            return lazyTexture.Value;
        }
        else
        {
            Utils.ThrowWarning("ProjectileTK.Rendering.Texture", $"Texture \'{name}\' could not be created!");

            return null;
        }
    }

    public void Use(TextureUnit unit)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, handle);
    }

    #region IDisposable Implementation

    private bool disposed = false;

    private void Dispose(bool disposing)
    {
        if (!disposed)
        {
            GL.DeleteTexture(handle);

            disposed = true;
        }
    }

    ~Texture()
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