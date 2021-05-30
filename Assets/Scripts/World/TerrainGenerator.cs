using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using UnityEngine;
using Unity.Rendering;

public class TerrainGenerator : MonoBehaviour
{
    [Range(2, 16)]
    public int Width = 2;
    [Range(2, 16)]
    public int Height = 4;
    [Range(2, 16)]
    public int Depth = 4;
    [Range(2, 256)]
    public int ChunkSize = 16;
    [Range(1, 255)]
    public int VisibleChunkCount = 9; // Must be an odd number
    public uint Seed = 1234;
    [Range(2, 255)]
    public float Scale = 25f;
    [Range(0f, 255f)]
    public float TerrainHeight = 1f;
    [Range(2, 32)]
    public int Octaves = 4;
    [Range(0f, 1f)]
    public float Persistance = 0.5f;
    [Range(1f, 10f)]
    public float Lacunarity = 2f;
    [Range(0f, 1f)]
    public float GroundLevel = 0.3f;
    [Range(0f, 1f)]
    public float WaterLevel = 0.25f;
    [Range(0.00f, 1.00f)]
    public float GroundSmoothingLevel = 0.1f;

    private EntityManager _entityManager;
    private EntityArchetype _archetype;
    private Unity.Mathematics.Random _prng;

    void Awake()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _archetype = _entityManager.CreateArchetype(typeof(Translation), typeof(RenderMesh), typeof(RenderBounds), typeof(LocalToWorld));
        _prng = new Unity.Mathematics.Random(Seed);
    }

    private void Update()
    {
        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        var chunkCount = Width * Height;

        var terrainHeightMap = new NativeMultiHashMap<int, float>(chunkCount * ChunkSize * ChunkSize, Allocator.TempJob);

        new TerrainHeightMapJob
        {
            ChunkSize = ChunkSize,
            Seed = Seed,
            Offset = new float2(0, 0),
            Scale = Scale,
            Octaves = Octaves,
            Persistance = Persistance,
            Lacunarity = Lacunarity,
            GroundLevel = GroundLevel,
            GroundSmoothingLevel = GroundSmoothingLevel,
            Prng = _prng,
            TerrainHeightMap = terrainHeightMap.AsParallelWriter()
        }.Schedule(chunkCount, 16).Complete();

        terrainHeightMap.Dispose();
    }
}