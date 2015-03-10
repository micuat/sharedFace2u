using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FaceManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetMesh(List<Vector3> list) {
		var mesh = new Mesh();
		mesh.name = "Mesh";

		int numVerts = list.Count;

		var vertices = new List<Vector3>();
		var normals = new List<Vector3>();
		var uvs = new List<Vector2>();
		var triangles = new List<int>();

		foreach (var vertex in list) {
			vertices.Add(vertex);
		}

		mesh.vertices = vertices.GetRange(0, numVerts).ToArray();
//		mesh.normals = normals.GetRange(0, numVerts).ToArray();
//		mesh.uv = uvs.GetRange(0, numVerts).ToArray();
//		mesh.triangles = triangles.GetRange(0, numIndices).ToArray();
		mesh.RecalculateBounds();
		
		// not separated at first
		GetComponent<MeshFilter>().sharedMesh = mesh;
	}
}
