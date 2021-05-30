using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct TerrainHeightMapJob : IJobParallelFor
{
    [ReadOnly]
    public int ChunkSize;
    [ReadOnly]
    public uint Seed;
    [ReadOnly]
    public float Scale;
    [ReadOnly]
    public int Octaves;
    [ReadOnly]
    public float Persistance;
    [ReadOnly]
    public float Lacunarity;
    [ReadOnly]
    public float GroundLevel;
    [ReadOnly]
    public float GroundSmoothingLevel;
    [ReadOnly]
    public float2 Offset;
    [ReadOnly]
    public Unity.Mathematics.Random Prng;
    [WriteOnly]
    public NativeMultiHashMap<int, float>.ParallelWriter TerrainHeightMap;

    public void Execute(int index)
    {
        NativeArray<float2> octaveOffsets = new NativeArray<float2>(Octaves, Allocator.Temp);

        try
        {
            float halfChunk = ChunkSize / 2f;
            float maxPosHeight = 0;
            float amplitude = 1f;

            for (int i = 0; i < octaveOffsets.Length; i++)
            {
                float offsetX = Prng.NextFloat(-100000, 100000) + Offset.x;
                float offsetZ = Prng.NextFloat(-100000, 100000) - Offset.y;
                octaveOffsets[i] = new float2(offsetX, offsetZ);

                maxPosHeight += amplitude;
                amplitude *= Persistance;
            }

            float heightAdjust = 2f * maxPosHeight / 1.75f;
            float groundAdjust = 2f * maxPosHeight;


            for (int x = 0; x < ChunkSize; x++)
            {
                for (int z = 0; z < ChunkSize; z++)
                {
                    amplitude = 1f;
                    float frequency = 1f;
                    float noiseHeight = 0f;
                    float groundLevel = 0f;

                    for (int i = 0; i < Octaves; i++)
                    {
                        float sampleX = (x - halfChunk + octaveOffsets[i].x) / Scale * frequency;
                        float sampleZ = (z - halfChunk + octaveOffsets[i].y) / Scale * frequency;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        groundLevel += (Mathf.PerlinNoise(sampleX / 10, sampleZ / 10) * 2 - 1) * amplitude;

                        amplitude *= Persistance;
                        frequency *= Lacunarity;
                    }

                    float normalizedHeight = (noiseHeight + 1) / heightAdjust;
                    float normalizedGroundLevel = (groundLevel + 1) / groundAdjust;

                    normalizedHeight = Mathf.MoveTowards(normalizedHeight, normalizedGroundLevel, GroundSmoothingLevel);

                    TerrainHeightMap.Add(index, normalizedHeight);
                }
            }
        }
        finally
        {
            octaveOffsets.Dispose();
        }
    }
}