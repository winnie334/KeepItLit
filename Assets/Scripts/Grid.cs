using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour {
	
	// Creates a procedural mesh of a given size
	// Great tutorial: https://catlikecoding.com/unity/tutorials/procedural-grid/

	public int xSize, ySize;
	public List<Vector2> heightScales; // Vector indicating the heightscale and the density
	public float dropOff;

	public int seed;

	[Tooltip("Min and max amount of trees on the island")]
	public Vector2 treeRange;
	public GameObject tree;

	private Mesh mesh;
	private Vector3[] vertices;

	private void Start () {
		Generate();
		SpawnTrees();
		bakeNavMesh();
	}

	private void Generate () {
		if (mesh != null) mesh.Clear();
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Procedural Grid";

		// We create a list of points in 3D space which will be the points of the mesh
		vertices = new Vector3[(xSize + 1) * (ySize + 1)];
		Vector2[] uv = new Vector2[vertices.Length];
		Vector4[] tangents = new Vector4[vertices.Length];
		Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
		var center = new Vector2(xSize / 2, ySize/2);
		for (int i = 0, y = 0; y <= ySize; y++) { // For each point, we assign a height based on its xy position
			for (int x = 0; x <= xSize; x++, i++) {
				float height = 0;
				for (var h = 0; h < heightScales.Count; h++) {
					height += Mathf.PerlinNoise(x * heightScales[h].y +seed, y * heightScales[h].y + seed) * heightScales[h].x;
				}
				var dist = Vector2.Distance(new Vector2(x, y), center);
				height -= dist * dropOff * xSize / 100;
				vertices[i] = new Vector3(x, height, y);
				uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
				tangents[i] = tangent;
			}
		}
		
		mesh.vertices = vertices;
		 mesh.uv = uv;
		mesh.tangents = tangents;

		// Now we have our vertices, we create triangles between them to have a visible mesh
		int[] triangles = new int[xSize * ySize * 6];
		for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
			for (int x = 0; x < xSize; x++, ti += 6, vi++) {
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
				triangles[ti + 5] = vi + xSize + 2;
			}
		}
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		GetComponent<MeshCollider>().sharedMesh = mesh;
	}

	// Spawns a random amount of trees on the island at the correct heights
	private void SpawnTrees() {
		var treeAmount = Random.Range(treeRange.x, treeRange.y);
		var chosenLocations = new List<int>(); // Saves the vertices indices on which we already spawned a tree
		var waterHeight = -gameObject.transform.position.y;
		
		for (var i = 0; i < treeAmount; i++) {
			var pos = new Vector2(Random.Range(0, xSize), Random.Range(0, ySize));
			var vertexIndex = (int) pos.y * xSize + (int) pos.x;
			// Keep re-rolling a new position until it is above the water and not selected before
			var rolls = 0; // We keep track to prevent theoretical infinite loop, although normally impossible
			while ((chosenLocations.Contains(vertexIndex) || vertices[vertexIndex].y < waterHeight) && rolls++ < 100) {
				pos = new Vector2(Random.Range(0, xSize), Random.Range(0, ySize));
				vertexIndex = (int) pos.y * xSize + (int) pos.x;
			}
			chosenLocations.Add(vertexIndex);
			// Eventually we can later use Normals instead of Quaternion.identity to spawn trees angled to their ground
			var localScale = transform.localScale;
			var vertexPos = vertices[vertexIndex];
			var posOffset = new Vector3(vertexPos.x * localScale.x, vertexPos.y * localScale.y, vertexPos.z * localScale.z);
			var newTree = Instantiate(tree, posOffset + transform.position, Quaternion.identity);
			newTree.transform.parent = gameObject.transform;
		}
	}

	private void bakeNavMesh() {
		var navMeshSurface = GetComponent<NavMeshSurface>();
		if (navMeshSurface) navMeshSurface.BuildNavMesh();
	}

	// Very useful function, enable this to automatically see the terrain update in unity as you're changing variables!
	// The reason this is commented out is because unity has a warning glitch which can be annoying
	// void OnValidate() {
	// 	Generate();
	// }
}