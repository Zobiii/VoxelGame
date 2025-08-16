// File: WorldGen/Noise.cs  (oder dein Ordner)
// Summary (EN): Seeded 2D Perlin with correct fBm (OctavePerlin). Includes handy shaping functions.
using System;

namespace VoxelGame.World
{
    /// <summary>
    /// Lightweight 2D Perlin noise generator with seed control.
    /// Usage: float n = noise.OctavePerlin(x, z, 5, 0.5f, 2.0f); // n in [0,1]
    /// </summary>
    public sealed class PerlinNoise
    {
        private readonly int[] _perm = new int[512];

        public PerlinNoise(int seed)
        {
            var p = new int[256];
            for (int i = 0; i < 256; i++) p[i] = i;

            var rng = new Random(seed);
            for (int i = 255; i > 0; i--)
            {
                int swap = rng.Next(i + 1);
                (p[i], p[swap]) = (p[swap], p[i]);
            }
            for (int i = 0; i < 512; i++) _perm[i] = p[i & 255];
        }

        /// <summary>
        /// Fractal Brownian Motion (fBm): multiple octaves of Perlin noise.
        /// Returns value in [0,1].
        /// </summary>
        public float OctavePerlin(float x, float y, int octaves, float persistence, float lacunarity)
        {
            float amp = 1f;
            float freq = 1f;
            float sum = 0f;
            float ampSum = 0f;

            for (int i = 0; i < octaves; i++)
            {
                sum += Noise2D(x * freq, y * freq) * amp;
                ampSum += amp;
                amp *= persistence;
                freq *= lacunarity;
            }
            return ampSum > 0f ? sum / ampSum : 0f;
        }

        /// <summary>Standard 2D Perlin mapped to [0,1].</summary>
        public float Noise2D(float x, float y) => Noise3D(x, y, 0f);

        // ----- shaping helpers for smoother terrains -----
        /// <summary>Hermite smoothstep in [0,1] → [0,1] (softens extremes).</summary>
        public static float Smooth01(float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            return t * t * (3f - 2f * t);
        }

        /// <summary>Cosine smooth in [0,1] → [0,1] (very gentle).</summary>
        public static float CosineSmooth01(float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            return 0.5f - 0.5f * MathF.Cos(t * MathF.PI);
        }

        // ----- internal classic Perlin 3D (z kept for compactness) -----
        private static float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);
        private static float Lerp(float a, float b, float t) => a + t * (b - a);
        private static float Grad(int hash, float x, float y, float z)
        {
            int h = hash & 15;
            float u = h < 8 ? x : y;
            float v = h < 4 ? y : (h == 12 || h == 14 ? x : z);
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        private float Noise3D(float x, float y, float z)
        {
            int X = (int)MathF.Floor(x) & 255;
            int Y = (int)MathF.Floor(y) & 255;
            int Z = (int)MathF.Floor(z) & 255;

            x -= MathF.Floor(x);
            y -= MathF.Floor(y);
            z -= MathF.Floor(z);

            float u = Fade(x);
            float v = Fade(y);
            float w = Fade(z);

            int A = _perm[X] + Y;
            int AA = _perm[A] + Z;
            int AB = _perm[A + 1] + Z;
            int B = _perm[X + 1] + Y;
            int BA = _perm[B] + Z;
            int BB = _perm[B + 1] + Z;

            float res = Lerp(
                Lerp(
                    Lerp(Grad(_perm[AA], x, y, z), Grad(_perm[BA], x - 1, y, z), u),
                    Lerp(Grad(_perm[AB], x, y - 1, z), Grad(_perm[BB], x - 1, y - 1, z), u),
                    v
                ),
                Lerp(
                    Lerp(Grad(_perm[AA + 1], x, y, z - 1), Grad(_perm[BA + 1], x - 1, y, z - 1), u),
                    Lerp(Grad(_perm[AB + 1], x, y - 1, z - 1), Grad(_perm[BB + 1], x - 1, y - 1, z - 1), u),
                    v
                ),
                w
            );
            return (res + 1f) * 0.5f;
        }
    }
}
