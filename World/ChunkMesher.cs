using System.Runtime.CompilerServices;
using OpenTK.Mathematics;
using VoxelGame.Rendering;

namespace VoxelGame.World;

/// <summary>
/// Builds a GPU mesh by emitting only faces that are visible (i.e., where neighbor is air or out of bounds)
/// </summary>

public sealed class ChunkMesher
{
    private static readonly (Vector3 normal, Vector3[] corners)[] Faces =
    {
        // +X
        (new Vector3(1,0,0), new[]{ new Vector3(1,0,0), new Vector3(1,1,0), new Vector3(1,1,1), new Vector3(1,0,1) }),
        // -X
        (new Vector3(-1,0,0), new[]{ new Vector3(0,0,1), new Vector3(0,1,1), new Vector3(0,1,0), new Vector3(0,0,0) }),
        // +Y
        (new Vector3(0,1,0), new[]{ new Vector3(0,1,1), new Vector3(1,1,1), new Vector3(1,1,0), new Vector3(0,1,0) }),
        // -Y
        (new Vector3(0,-1,0), new[]{ new Vector3(0,0,0), new Vector3(1,0,0), new Vector3(1,0,1), new Vector3(0,0,1) }),
        // +Z
        (new Vector3(0,0,1), new[]{ new Vector3(0,0,1), new Vector3(1,0,1), new Vector3(1,1,1), new Vector3(0,1,1) }),
        // -Z
        (new Vector3(0,0,-1), new[]{ new Vector3(1,0,0), new Vector3(0,0,0), new Vector3(0,1,0), new Vector3(1,1,0) }),
    };

    public Mesh BuildMesh(Chunk chunk)
    {
        var verts = new List<float>(Chunk.Size * Chunk.Height * Chunk.Size * 24);
        var inds = new List<int>(Chunk.Size * Chunk.Height * Chunk.Size * 36);
        int vbase = 0;

        for (int z = 0; z < Chunk.Size; z++)
            for (int y = 0; y < Chunk.Height; y++)
                for (int x = 0; x < Chunk.Size; x++)
                {
                    if (chunk.Get(x, y, z) == Block.Air) continue;

                    for (int f = 0; f < 6; f++)
                    {
                        var (n, corners) = Faces[f];
                        var nx = x + (int)n.X;
                        var ny = y + (int)n.Y;
                        var nz = z + (int)n.Z;
                        bool neighborSolid = chunk.InBounds(nx, ny, nz) && chunk.Get(x, y, z) == Block.Solid;
                        if (neighborSolid) continue;

                        for (int i = 0; i < 4; i++)
                        {
                            var c = corners[i] + new Vector3(x, y, z);
                            verts.Add(c.X); verts.Add(c.Y); verts.Add(c.Z);
                            verts.Add(n.X); verts.Add(n.Y); verts.Add(n.Z);
                        }

                        inds.Add(vbase + 0); inds.Add(vbase + 1); inds.Add(vbase + 2);
                        inds.Add(vbase + 2); inds.Add(vbase + 3); inds.Add(vbase + 0);
                        vbase += 4;
                    }
                }
        return new Mesh(verts.ToArray(), inds.ToArray());
    }
}