using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FaceManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var mesh = new Mesh();
		mesh.name = "Mesh";
		
		int numVerts = FaceConst.FACE_NUM_POINTS;
		
		var vertices = new Vector3[FaceConst.FACE_NUM_POINTS];
		var normals = new Vector3[FaceConst.FACE_NUM_POINTS];
		var uvs = new Vector2[FaceConst.FACE_NUM_POINTS];
		var triangles = new int[FaceConst.FACE_NUM_TRIANGLES * 3];

		for(int i = 0; i < FaceConst.FACE_NUM_TRIANGLES; i++) {
			triangles[i * 3 + 0] = FaceConst.FaceTriangles[i, 0];
			triangles[i * 3 + 1] = FaceConst.FaceTriangles[i, 1];
			triangles[i * 3 + 2] = FaceConst.FaceTriangles[i, 2];
		}

		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = uvs;
		mesh.triangles = triangles;

		// not separated at first
		GetComponent<MeshFilter>().sharedMesh = mesh;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetMesh(List<Vector3> list) {
		var mesh = GetComponent<MeshFilter>().sharedMesh;

		int numVerts = list.Count;

		int i = 0;
		foreach (var vertex in list) {
			mesh.vertices[i++] = vertex;
		}
		mesh.RecalculateBounds();
	}
}
