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
	int[] triangles = new int[FaceConst.FACE_NUM_TRIANGLES * 3 * 2];

	Vector3 facePosition = new Vector3();
	Quaternion faceRotation = new Quaternion();
	
	public GameObject addonObject;

	// Use this for initialization
	void Start () {
		mesh = new Mesh();
		mesh.name = "Mesh";

		for(int i = 0; i < FaceConst.FACE_NUM_TRIANGLES; i++) {
			triangles[i * 3 + 0] = FaceConst.FaceTriangles[i, 0];
			triangles[i * 3 + 1] = FaceConst.FaceTriangles[i, 1];
			triangles[i * 3 + 2] = FaceConst.FaceTriangles[i, 2];
		}
		for(int i = 0; i < FaceConst.FACE_NUM_TRIANGLES; i++) {
			triangles[i * 3 + 0 + FaceConst.FACE_NUM_TRIANGLES * 3] = FaceConst.FaceTriangles[i, 0];
			triangles[i * 3 + 1 + FaceConst.FACE_NUM_TRIANGLES * 3] = FaceConst.FaceTriangles[i, 2];
			triangles[i * 3 + 2 + FaceConst.FACE_NUM_TRIANGLES * 3] = FaceConst.FaceTriangles[i, 1];
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

		Matrix4x4 extrinsics = new Matrix4x4();

		extrinsics.SetRow (0, new Vector4 (9.6951375724054678e-001f, 9.4447770100172856e-002f,
		                                   2.2610328003906302e-001f, -3.3052667086500043e+002f));
		extrinsics.SetRow (1, new Vector4 (-2.3094250319200885e-002f, 9.5384408201206639e-001f,
		                                   -2.9941296366849657e-001f, -8.9743887308778181e+000f));
		extrinsics.SetRow (2, new Vector4 (-2.4394616234635083e-001f, 2.8506330162555693e-001f,
		                                   9.2694616021796250e-001f, 3.8313595147359138e+001f));
		extrinsics.SetRow (3, new Vector4 (0, 0, 0, 1));
		
/*		transform.position = extrinsics.GetColumn(3);
		
		transform.rotation = Quaternion.LookRotation(
			extrinsics.GetColumn(2),
			extrinsics.GetColumn(1)
			);
*/
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

		addonObject.transform.position = facePosition;
		addonObject.transform.rotation = faceRotation;
	}
	
	public void PacketReceivedEvent(OSCServer sender, OSCPacket packet) {
		if (packet.Address.Equals ("/osceleton/face_mesh")) {
			for (int i = 0; i < FaceConst.FACE_NUM_POINTS; i++) {
				float x = -(float)packet.Data [i * 3 + 2] * FaceConst.FACE_SCALE;
				float y = (float)packet.Data [i * 3 + 3] * FaceConst.FACE_SCALE;
				float z = (float)packet.Data [i * 3 + 4] * FaceConst.FACE_SCALE;
				verticesBack [i] = new Vector3 (x, y, z);
			}
		} else if (packet.Address.Equals ("/osceleton/face")) {
			facePosition.x = (float)packet.Data[2] * FaceConst.FACE_SCALE * 0.001f;
			facePosition.y = (float)packet.Data[3] * FaceConst.FACE_SCALE * 0.001f;
			facePosition.z = (float)packet.Data[4] * FaceConst.FACE_SCALE * 0.001f;

			Vector3 euler = new Vector3((float)packet.Data[5], (float)packet.Data[6], (float)packet.Data[7]);
			faceRotation.eulerAngles = euler;
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
