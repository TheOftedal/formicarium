// Marching Cubes algorithm copied from Sebastian Lague's Coding Adventures : Terraformning
// https://www.youtube.com/watch?v=vTMEdHcKgM4&t=231s
// Support him on Patreon: https://www.patreon.com/SebastianLague

using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class Chunk
{
	public Vector3 Center { get; private set; }
	public float Size { get; private set; }
	public Mesh Mesh { get; private set; }

	private readonly ComputeBuffer _pointsBuffer;

	public MeshFilter filter;
	MeshRenderer renderer;
	MeshCollider collider;
	public bool terra;
	public Vector3 Id { get; private set; }

	// Mesh processing
	Dictionary<int2, int> vertexIndexMap;
	List<Vector3> processedVertices;
	List<Vector3> processedNormals;
	List<int> processedTriangles;


	public Chunk(Vector3 coord, Vector3 centre, float size, int numPointsPerAxis, GameObject meshHolder)
	{
		Id = coord;
		Center = centre;
		Size = size;
		Mesh = new Mesh();
		Mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

		int numPointsTotal = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;
		ComputeHelper.CreateStructuredBuffer<PointData>(ref _pointsBuffer, numPointsTotal);

		// Mesh rendering and collision components
		filter = meshHolder.AddComponent<MeshFilter>();
		renderer = meshHolder.AddComponent<MeshRenderer>();


		filter.mesh = Mesh;
		collider = renderer.gameObject.AddComponent<MeshCollider>();

		vertexIndexMap = new Dictionary<int2, int>();
		processedVertices = new List<Vector3>();
		processedNormals = new List<Vector3>();
		processedTriangles = new List<int>();
	}

	public void CreateMesh(VertexData[] vertexData, int numVertices, bool useFlatShading)
	{
		vertexIndexMap.Clear();
		processedVertices.Clear();
		processedNormals.Clear();
		processedTriangles.Clear();

		int triangleIndex = 0;

		for (int i = 0; i < numVertices; i++)
		{
			VertexData data = vertexData[i];

			int sharedVertexIndex;
			if (!useFlatShading && vertexIndexMap.TryGetValue(data.id, out sharedVertexIndex))
			{
				processedTriangles.Add(sharedVertexIndex);
			}
			else
			{
				if (!useFlatShading)
				{
					vertexIndexMap.Add(data.id, triangleIndex);
				}
				processedVertices.Add(data.position);
				processedNormals.Add(data.normal);
				processedTriangles.Add(triangleIndex);
				triangleIndex++;
			}
		}

		collider.sharedMesh = null;

		Mesh.Clear();
		Mesh.SetVertices(processedVertices);
		Mesh.SetTriangles(processedTriangles, 0, true);

		if (useFlatShading)
		{
			Mesh.RecalculateNormals();
		}
		else
		{
			Mesh.SetNormals(processedNormals);
		}

		collider.sharedMesh = Mesh;
	}

	public struct PointData
	{
		public Vector3 position;
		public Vector3 normal;
		public float density;
	}

	public void AddCollider()
	{
		collider.sharedMesh = Mesh;
	}

	public void SetMaterial(Material material)
	{
		renderer.material = material;
	}

	public void Release()
	{
		ComputeHelper.Release(_pointsBuffer);
	}

	public void DrawBoundsGizmo(Color col)
	{
		Gizmos.color = col;
		Gizmos.DrawWireCube(Center, Vector3.one * Size);
	}
}