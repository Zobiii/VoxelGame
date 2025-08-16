using OpenTK.Mathematics;

namespace VoxelGame.World;

/// <summary>
/// A single 16x16x16 chunk. Coordinates are local to the chunk
/// </summary>

public partial class Chunk
{
    public const int Height = 200;
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

    public void GenerateFromNoiseSmoothed(
            PerlinNoise noise,
            int minHeight,
            int maxHeight,
            float baseScale = 0.045f,
            float detailScale = 0.12f,
            int baseOctaves = 5,
            int detailOctaves = 3,
            float persistence = 0.5f,
            float lacunarity = 2.0f,
            int blurPasses = 2,
            int optionalSlopeClamp = 0
        )
    {

        minHeight = Math.Clamp(minHeight, 0, Height - 1);
        maxHeight = Math.Clamp(maxHeight, 0, Height - 1);
        if (minHeight > maxHeight) minHeight = maxHeight;

        int W = Width;
        int D = Depth;


        var h = new float[W, D];

        for (int z = 0; z < D; z++)
            for (int x = 0; x < W; x++)
            {

                float wx = Origin.X + x;
                float wz = Origin.Z + z;


                float baseN = noise.OctavePerlin(wx * baseScale, wz * baseScale, baseOctaves, persistence, lacunarity);

                float detailN = noise.OctavePerlin(wx * detailScale, wz * detailScale, detailOctaves, persistence, lacunarity);


                float n = baseN * 0.80f + detailN * 0.20f;


                n = PerlinNoise.CosineSmooth01(n);

                float hFloat = minHeight + n * (maxHeight - minHeight);
                h[x, z] = hFloat;
            }


        if (blurPasses > 0)
        {
            var tmp = new float[W, D];
            for (int pass = 0; pass < blurPasses; pass++)
            {

                for (int z = 0; z < D; z++)
                {
                    for (int x = 0; x < W; x++)
                    {
                        float a = h[Math.Max(x - 1, 0), z];
                        float b = h[x, z];
                        float c = h[Math.Min(x + 1, W - 1), z];
                        tmp[x, z] = (a + b + c) / 3f;
                    }
                }

                for (int z = 0; z < D; z++)
                {
                    for (int x = 0; x < W; x++)
                    {
                        float a = tmp[x, Math.Max(z - 1, 0)];
                        float b = tmp[x, z];
                        float c = tmp[x, Math.Min(z + 1, D - 1)];
                        h[x, z] = (a + b + c) / 3f;
                    }
                }
            }
        }


        if (optionalSlopeClamp > 0)
        {
            int iter = 1;
            for (int it = 0; it < iter; it++)
            {
                for (int z = 0; z < D; z++)
                    for (int x = 0; x < W; x++)
                    {
                        float cur = h[x, z];

                        void ClampNeighbor(int nx, int nz)
                        {
                            if (nx < 0 || nx >= W || nz < 0 || nz >= D) return;
                            float nb = h[nx, nz];
                            float delta = cur - nb;
                            if (delta > optionalSlopeClamp)
                                h[x, z] = nb + optionalSlopeClamp;
                            else if (-delta > optionalSlopeClamp)
                                h[x, z] = nb - optionalSlopeClamp;
                        }
                        ClampNeighbor(x - 1, z);
                        ClampNeighbor(x + 1, z);
                        ClampNeighbor(x, z - 1);
                        ClampNeighbor(x, z + 1);
                    }
            }
        }


        for (int z = 0; z < D; z++)
            for (int x = 0; x < W; x++)
            {
                int hInt = (int)MathF.Floor(h[x, z]);
                hInt = Math.Clamp(hInt, 0, Height - 1);

                for (int y = 0; y < Height; y++)
                    _blocks[x, y, z] = (y <= hInt) ? Block.Solid : Block.Air;
            }
    }
}
