using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour {
	
	// Creates a procedural mesh of a given size
	// Great tutorial: https://catlikecoding.com/unity/tutorials/procedural-grid/

	public int xSize, ySize;
	public List<Vector2> heightScales; // Vector indicating the heightscale and the density
	public float dropOff;

	private Mesh mesh;
	private Vector3[] vertices;

	private void Start () {
		Generate();
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
		
		for (int i = 0, y = 0; y <= ySize; y++) { // For each point, we assign a height based on its xy position
			for (int x = 0; x <= xSize; x++, i++) {
				float height = 0;
				for (var h = 0; h < heightScales.Count; h++) {
					height += Mathf.PerlinNoise(x * heightScales[h].y, y * heightScales[h].y) * heightScales[h].x;
				}
				//height *= (float) Math.Abs(xSize/2 - x) / xSize + (float) Math.Abs(ySize/2 - y) / ySize * 2;
				var dist = Vector2.Distance(new Vector2(x, y), new Vector2(xSize / 2, ySize / 2));
				height -= dist * dropOff;
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

	// Very useful function, enable this to automatically see the terrain update in unity as you're changing variables!
	// The reason this is commented out is because unity has a warning glitch which can be annoying
	void OnValidate() {
		Generate();
	}
}