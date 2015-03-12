using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityOSC;

public class FaceManager : MonoBehaviour {

	Mesh mesh;

	Vector3[] vertices = new Vector3[FaceConst.FACE_NUM_POINTS];
	Vector3[] verticesBack = new Vector3[FaceConst.FACE_NUM_POINTS];
	Vector3[] normals = new Vector3[FaceConst.FACE_NUM_POINTS];
	Vector2[] uvs = new Vector2[FaceConst.FACE_NUM_POINTS];
	int[] triangles = new int[FaceConst.FACE_NUM_TRIANGLES * 3];

	// Use this for initialization
	void Start () {
		mesh = new Mesh();
		mesh.name = "Mesh";

		for(int i = 0; i < FaceConst.FACE_NUM_TRIANGLES; i++) {
			triangles[i * 3 + 0] = FaceConst.FaceTriangles[i, 0];
			triangles[i * 3 + 1] = FaceConst.FaceTriangles[i, 1];
			triangles[i * 3 + 2] = FaceConst.FaceTriangles[i, 2];
		}

		for(int i = 0; i < FaceConst.FACE_NUM_POINTS; i++) {
			uvs[i] = new Vector2(FaceConst.FaceUvs[i, 0] / FaceConst.FaceUvWidth, (768-FaceConst.FaceUvs[i, 1]) / FaceConst.FaceUvHeight);
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
		for( int i = 0; i < FaceConst.FACE_NUM_POINTS; i++) {
			vertices[i].x = verticesBack[i].x;
			vertices[i].y = verticesBack[i].y;
			vertices[i].z = verticesBack[i].z;
		}
		mesh.vertices = vertices;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		GetComponent<MeshFilter>().sharedMesh = mesh;
	}
	
	public void PacketReceivedEvent(OSCServer sender, OSCPacket packet) {
		Debug.Log (packet.Address);
		if(packet.Address.Equals("/osceleton/face_mesh")) {
			for( int i = 0; i < FaceConst.FACE_NUM_POINTS; i++) {
				float scale = 100;
				float x = (float)packet.Data[i * 3 + 2] * scale;
				float y = (float)packet.Data[i * 3 + 3] * scale;
				float z = (float)packet.Data[i * 3 + 4] * scale;
				verticesBack[i] = new Vector3(x, y, z);
			}
		}
	}
	
	void OnDrawGizmos()
	{
		if (mesh) {
			for(int i = 0; i < FaceConst.FACE_NUM_TRIANGLES; i++) {
				Gizmos.DrawLine (vertices[mesh.triangles[i * 3 + 0]],
				                 vertices[mesh.triangles[i * 3 + 1]]);
				Gizmos.DrawLine (vertices[mesh.triangles[i * 3 + 1]],
				                 vertices[mesh.triangles[i * 3 + 2]]);
				Gizmos.DrawLine (vertices[mesh.triangles[i * 3 + 2]],
				                 vertices[mesh.triangles[i * 3 + 0]]);
			}
		}
	}
}
