using OpenTK.Mathematics;

namespace VoxelGame.World;

/// <summary>
/// A single 16x16x16 chunk. Coordinates are local to the chunk
/// </summary>

public partial class Chunk
{
    public const int Height = 16;
    public const int Width = 500;
    public const int Depth = 500;

    public readonly Vector3i Origin;

    private readonly Block[,,] _blocks = new Block[Width, Height, Depth];

    public Chunk(Vector3i origin)
    {
        Origin = origin;
    }

    public Block Get(int x, int y, int z) => _blocks[x, y, z];
    public void Set(int x, int y, int z, Block b) => _blocks[x, y, z] = b;

    public bool InBounds(int x, int y, int z)
        => x >= 0 && x < Width && y >= 0 && y < Height && z >= 0 && z < Depth;

    public void GenerateFromNoise(PerlinNoise noise, float scale = 0.01f, int minHeight = 3, int maxHeight = Height - 1, int octaves = 4)
    {
        if (minHeight < 0) minHeight = 0;
        if (maxHeight >= Height) maxHeight = Height - 1;
        if (minHeight > maxHeight) minHeight = maxHeight;

        for (int z = 0; z < Depth; z++)
            for (int x = 0; x < Width; x++)
            {
                float n = noise.OctavePerlin(x * scale, z * scale, octaves, 0.5f, 2.0f) * 0.25f;
                int h = minHeight + (int)MathF.Round(n * (maxHeight - minHeight));

                for (int y = 0; y < Height; y++)
                {
                    Set(x, y, z, y <= h ? Block.Solid : Block.Air);
                }
            }
    }
}