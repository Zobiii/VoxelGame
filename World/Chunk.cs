using OpenTK.Mathematics;

namespace VoxelGame.World;

/// <summary>
/// A single 16x16x16 chunk. Coordinates are local to the chunk
/// </summary>

public sealed class Chunk
{
    public const int Size = 16;
    public const int Height = 16;

    public readonly Vector3i Origin;

    private readonly Block[,,] _blocks = new Block[Size, Height, Size];

    public Chunk(Vector3i origin)
    {
        Origin = origin;
    }

    public Block Get(int x, int y, int z) => _blocks[x, y, z];
    public void Set(int x, int y, int z, Block b) => _blocks[x, y, z] = b;

    public bool InBounds(int x, int y, int z)
        => x >= 0 && x < Size && y >= 0 && y < Height && z >= 0 && z < Size;

    /// <summary>
    /// Fill a simple demo shape: a rolling hill-ish surface for visual variety
    /// </summary>

    public void GenerateDemoContent()
    {
        for (int z = 0; z < Size; z++)
            for (int x = 0; x < Size; x++)
            {
                //Small deterministic height variation without noise libs
                float fx = x / (float)Size;
                float fz = z / (float)Size;
                int h = 6 + (int)(4 * MathF.Sin(fx * MathF.PI * 2) * MathF.Cos(fz * MathF.PI * 2));
                h = Math.Clamp(h, 1, Height - 1);

                for (int y = 0; y <= h; y++)
                    _blocks[x, y, z] = Block.Solid;
            }
    }
}