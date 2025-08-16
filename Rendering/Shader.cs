using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace VoxelGame.Rendering;

/// <summary>
/// Small OpenGL shader helper for compiling programs and setting uniforms.
/// </summary>

public sealed class Shader : IDisposable
{
    public int Handle { get; }

    public Shader(string vertexPath, string fragmentPath)
    {
        string vsSource = File.ReadAllText(vertexPath);
        string fsSource = File.ReadAllText(fragmentPath);

        int vs = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vs, vsSource);
        GL.CompileShader(vs);
        GL.GetShader(vs, ShaderParameter.CompileStatus, out int vsStatus);
        if (vsStatus == 0) throw new Exception($"Vertex shader error: {GL.GetShaderInfoLog(vs)}");

        int fs = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fs, fsSource);
        GL.CompileShader(fs);
        GL.GetShader(fs, ShaderParameter.CompileStatus, out int fsStatus);
        if (fsStatus == 0) throw new Exception($"Fragment shader error: {GL.GetShaderInfoLog(fs)}");

        Handle = GL.CreateProgram();
        GL.AttachShader(Handle, vs);
        GL.AttachShader(Handle, fs);
        GL.LinkProgram(Handle);
        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int linkStatus);
        if (linkStatus == 0) throw new Exception($"Program link error: {GL.GetProgramInfoLog(Handle)}");

        // Shaders are now linked into the program; we can delete the individual objects
        GL.DetachShader(Handle, vs);
        GL.DetachShader(Handle, fs);
        GL.DeleteShader(vs);
        GL.DeleteShader(fs);
    }

    public void Use() => GL.UseProgram(Handle);

    public void Set(string name, Matrix4 value)
    {
        int loc = GL.GetUniformLocation(Handle, name);
        GL.UniformMatrix4(loc, false, ref value);
    }

    public void Set(string name, Vector3 value)
    {
        int loc = GL.GetUniformLocation(Handle, name);
        GL.Uniform3(loc, value);
    }

    public void Dispose()
    {
        GL.DeleteProgram(Handle);
    }
}