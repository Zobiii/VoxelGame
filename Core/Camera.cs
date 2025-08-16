using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace VoxelGame.Core;

/// <summary>
/// Very small FPS-style camera with yaw/pitch and WASD movement
/// </summary>

public sealed class Camera
{
    public Vector3 Position;
    public float Yaw;
    public float Pitch;

    public Camera(Vector3 startPos)
    {
        Position = startPos;
        Yaw = -90f;
        Pitch = 0f;
    }

    public void AddYawPitch(float yawDelta, float pitchDelta)
    {
        Yaw += yawDelta;
        Pitch += pitchDelta;
        Pitch = MathHelper.Clamp(Pitch, -89f, 89f);
    }

    public void Move(KeyboardState kb, float dt, float speed)
    {
        var forward = GetForward();
        var right = Vector3.Normalize(Vector3.Cross(forward, Vector3.UnitY));
        var up = Vector3.UnitY;

        if (kb.IsKeyDown(Keys.W)) Position += forward * speed * dt;
        if (kb.IsKeyDown(Keys.S)) Position -= forward * speed * dt;
        if (kb.IsKeyDown(Keys.A)) Position -= right * speed * dt;
        if (kb.IsKeyDown(Keys.D)) Position += right * speed * dt;
        if (kb.IsKeyDown(Keys.Space)) Position += up * speed * dt;
        if (kb.IsKeyDown(Keys.LeftControl)) Position -= up * speed * dt;
    }

    public Matrix4 GetViewMatrix()
    {
        var front = GetForward();
        return Matrix4.LookAt(Position, Position + front, Vector3.UnitY);
    }

    public Matrix4 GetProjection(float aspect)
    {
        return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70f), aspect, 0.1f, 500f);
    }

    public Vector3 GetForward()
    {
        var yaw = MathHelper.DegreesToRadians(Yaw);
        var pitch = MathHelper.DegreesToRadians(Pitch);
        var x = MathF.Cos(yaw) * MathF.Cos(pitch);
        var y = MathF.Sin(pitch);
        var z = MathF.Sin(yaw) * MathF.Cos(pitch);
        return Vector3.Normalize(new Vector3(x, y, z));
    }
}