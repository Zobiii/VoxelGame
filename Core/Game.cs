using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelGame.Rendering;
using VoxelGame.World;

namespace VoxelGame.Core;

public sealed class Game : GameWindow
{
    private readonly Camera _camera = new(new Vector3(8f, 10f, 28f));
    private double _time;

    private Renderer _renderer = null!;
    private Mesh _chunkMesh = null!;

    private bool _firstMouse = true;
    private Vector2 _lastMousePos;

    public Game()
        : base(GameWindowSettings.Default,
                new NativeWindowSettings
                {
                    ClientSize = new Vector2i(1280, 720),
                    Title = "VoxelGame - Minimal GPU Voxel",
                    APIVersion = new Version(3, 3),
                    Flags = ContextFlags.ForwardCompatible,
                })
    {
        CursorState = CursorState.Grabbed;
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.Enable(EnableCap.DepthTest);
        GL.ClearColor(0.15f, 0.18f, 0.22f, 1f);

        _renderer = new Renderer("Assets/Shaders/basic.vert", "Assets/Shaders/basic.frag");


        var chunk = new Chunk(new Vector3i(0, 0, 0));
        chunk.GenerateDemoContent();

        var mesher = new ChunkMesher();
        _chunkMesh = mesher.BuildMesh(chunk);

        _renderer.LightDirection = new Vector3(0.6f, 1.0f, 0.3f).Normalized();
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        _chunkMesh?.Dispose();
        _renderer?.Dispose();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, Size.X, Size.Y);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        _time += args.Time;

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
            return;
        }

        var mouse = MouseState;
        if (_firstMouse)
        {
            _lastMousePos = mouse.Position;
            _firstMouse = false;
        }

        var delta = mouse.Delta;
        const float sens = 0.12f;
        _camera.AddYawPitch(delta.X * sens, -delta.Y * sens);

        float speed = KeyboardState.IsKeyDown(Keys.LeftShift) ? 15f : 6f;
        _camera.Move(KeyboardState, (float)args.Time, speed);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        var proj = _camera.GetProjection(Size.X / (float)Size.Y);
        var view = _camera.GetViewMatrix();
        var model = Matrix4.Identity;

        _renderer.DrawMesh(_chunkMesh, model, view, proj, new Vector3(1f, 1f, 1f));

        SwapBuffers();
    }
}