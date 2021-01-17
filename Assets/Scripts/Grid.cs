using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using EzySlice;
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
	public bool generateRandom;
	public float beachSize;
	public float grassLessSize;
	public Material[] grassLevels;
	public Material[] sparseGrassLevels;
	public Material mainMaterial;
	public Material sandMaterial;
	private GameObject sparseGrassArea;
	private GameObject denseGrassArea;

	[Serializable]
	public struct SpawnOption {
		public GameObject obj;
		public int minAmount;
		public int maxAmount;
	}
	public SpawnOption[] spawns;
	private List<int> chosenLocations; // Saves the vertices indices on which we already spawned an object
	private List<GameObject> thingsSpawned; // List of all the things we have spawned so far

	private Mesh mesh;
	private Vector3[] vertices;

	private void Start () {
		Generate();
		spawnObjects();
		if (generateRandom) spawnPlayer(); // This island is random, meaning the player and the fire need to be spawned as well
		bakeNavMesh();
		cutMesh();
	}

	private void Generate () {
		if (mesh != null) mesh.Clear();
		if (generateRandom) seed = Random.Range(0, 100000);
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

	// Spawns a random amount of objects on the island at the correct heights
	private void spawnObjects() {
		chosenLocations = new List<int>(); // Make sure the current list of chosen locations is empty
		thingsSpawned = new List<GameObject>();
		foreach (var objectToSpawn in spawns) {
			var amount = Random.Range(objectToSpawn.minAmount, objectToSpawn.maxAmount);
			// If it is an item, we should spawn it slightly higher to ensure it doesn't clip through the ground
			var spawnHigher = objectToSpawn.obj.GetComponent<Rigidbody>() != null;
			for (var i = 0; i < amount; i++) {
				var spawnPos = getRandomSpawnLocation() + (spawnHigher ? Vector3.up : Vector3.zero);
				thingsSpawned.Add(Instantiate(objectToSpawn.obj, spawnPos, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0))));
			}
		}
	}

	// Returns a vector location on which no object has been spawned before
	private Vector3 getRandomSpawnLocation() {
		var waterHeight = -gameObject.transform.position.y + beachSize;

		var pos = new Vector2(Random.Range(0, xSize), Random.Range(0, ySize));
		var vertexIndex = (int) pos.y * xSize + (int) pos.x;
		// Keep re-rolling a new position until it is above the water and not selected before
		var rolls = 0; // We keep track to prevent theoretical infinite loop, although normally impossible
		while ((chosenLocations.Contains(vertexIndex) || vertices[vertexIndex].y < waterHeight) && rolls++ < 100) {
			pos = new Vector2(Random.Range(0, xSize), Random.Range(0, ySize));
			vertexIndex = (int) pos.y * xSize + (int) pos.x;
		}
		chosenLocations.Add(vertexIndex); // We save this index to make sure we don't spawn anything else here
		var localScale = transform.localScale;
		var vertexPos = vertices[vertexIndex];
		var posOffset = new Vector3(vertexPos.x * localScale.x, vertexPos.y * localScale.y, vertexPos.z * localScale.z);
		return posOffset + transform.position;
	}

	// Spawns the player and the fire near a beach
	private void spawnPlayer() {
		var player = GameObject.Find("Player");
		// We find the locations by getting random points (this ruins the chosenLocations list, but not more relevant
		// at this point) and then getting the one with the lowest y position. We do the same for the fire, but this
		// time we choose the location which is closest to the player. This doesn't guarantee that the fire or player
		// will have great positions, but it is statistically unlikely they are far away from the beach/player
		var spawnPositions = new List<Vector3>();
		for(var i = 0; i < 30; i++) spawnPositions.Add(getRandomSpawnLocation());
		var spawnPos = spawnPositions.OrderBy(pos => pos.y).First();
		player.transform.position = spawnPos + Vector3.up;

		var fire = GameObject.Find("Fire");
		spawnPositions = new List<Vector3>();
		for(var i = 0; i < 100; i++) spawnPositions.Add(getRandomSpawnLocation());
		spawnPos = spawnPositions.OrderBy(pos => Vector3.Distance(spawnPos, pos)).First();
		fire.transform.position = spawnPos;

		foreach (var spawnedThing in thingsSpawned) {
			if (Vector3.Distance(spawnedThing.transform.position, fire.transform.position) < 4) {
				Debug.Log("Destroyed " + spawnedThing.name);
				Destroy(spawnedThing);
			}
		}
	}

	private void bakeNavMesh() {
		var navMeshSurface = GetComponent<NavMeshSurface>();
		if (navMeshSurface) navMeshSurface.BuildNavMesh();
	}

	private void cutMesh() {
		// Cut into beach and non-beach area
		var sandGrassPieces = gameObject.SliceInstantiate(new Vector3(0, beachSize, 0), new Vector3(0, 1, 0));
		sandGrassPieces[1].GetComponent<Renderer>().sharedMaterials = new [] {sandMaterial};
		
		// Cut main area into dense and sparse grass areas
		var greenGrassPieces = sandGrassPieces[0].SliceInstantiate(new Vector3(0, grassLessSize, 0), new Vector3(0, 1, 0));
		Destroy(sandGrassPieces[0]);
		greenGrassPieces[0].GetComponent<Renderer>().sharedMaterials = new [] {mainMaterial};
		greenGrassPieces[1].GetComponent<Renderer>().sharedMaterials = new [] {mainMaterial};
		
		// Create a clones of the grass area so we can add a new material to them, the grass shader
		denseGrassArea = Instantiate(greenGrassPieces[0], greenGrassPieces[0].transform.position, Quaternion.identity);
		sparseGrassArea = Instantiate(greenGrassPieces[1], greenGrassPieces[1].transform.position, Quaternion.identity);
		setGrassQuality(QualitySettings.GetQualityLevel()); // Update the grass materials with current quality settings

		// Disable the rendering of the original island, but keep it, for colliders, children, navmesh, etc
		gameObject.GetComponent<MeshRenderer>().enabled = false;
	}

	public void setGrassQuality(int index) {
		denseGrassArea.GetComponent<Renderer>().sharedMaterials = new [] {grassLevels[index]};
		sparseGrassArea.GetComponent<Renderer>().sharedMaterials = new [] {sparseGrassLevels[index]};
	}

	// Very useful function, enable this to automatically see the terrain update in unity as you're changing variables!
	// The reason this is commented out is because unity has a warning glitch which can be annoying
	// void OnValidate() {
	// 	Generate();
	// }
}