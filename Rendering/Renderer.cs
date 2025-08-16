using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace VoxelGame.Rendering;

/// <summary>
/// High-level draw helper that binds shader, sets matrices and issues the draw call.
/// </summary>

public sealed class Renderer : IDisposable
{
    private readonly Shader _shader;
    public Vector3 LightDirection = new(0.6f, 1.0f, 0.3f);

    public Renderer(string vertPath, string fragPath)
    {
        _shader = new Shader(vertPath, fragPath);
    }

    public void DrawMesh(Mesh mesh, Matrix4 model, Matrix4 view, Matrix4 proj, Vector3 color)
    {
        _shader.Use();
        _shader.Set("uModel", model);
        _shader.Set("uView", view);
        _shader.Set("uProj", proj);
        _shader.Set("uColor", color);
        _shader.Set("uLightDir", LightDirection);

        GL.CullFace(TriangleFace.Back);
        GL.Enable(EnableCap.CullFace);

        mesh.Draw();

        GL.Disable(EnableCap.CullFace);
    }

    public void Dispose()
    {
        _shader.Dispose();
    }
}