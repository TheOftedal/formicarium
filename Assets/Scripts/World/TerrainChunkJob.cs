using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

public class TerrainChunkJob : IJobParallelFor
{
    [ReadOnly]
    public NativeMultiHashMap<int, float3> HeightMap;
    [WriteOnly]
    public NativeMultiHashMap<int, float3> Vertices;
    [WriteOnly]
    public NativeMultiHashMap<int, float3> Normals;
    [WriteOnly]
    public NativeMultiHashMap<int, int> Triangles;
    [WriteOnly]
    public NativeMultiHashMap<int, float2> Uvs;

    public void Execute(int index)
    {
        var heightMap = GetHeightMap(ref index);

        try
        {
            sbyte[] density = new sbyte[8];

        }
        finally
        {
            heightMap.Dispose();
        }
    }

    private NativeArray<float3> GetHeightMap(ref int index)
    {
        var heightMap = new NativeArray<float3>(HeightMap.CountValuesForKey(index), Allocator.Temp);

        if (heightMap.Length < 1)
        {
            return heightMap;
        }

        var values = HeightMap.GetValuesForKey(index);

        for (int i = 0; values.MoveNext(); i++)
        {
            heightMap[i] = values.Current;
        }

        return heightMap;
    }
}