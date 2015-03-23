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

	Vector3 facePosition = new Vector3();
	Quaternion faceRotation = new Quaternion();

	Vector3 fingerPosition = new Vector3();
	public GameObject fingerObject;

	public List<Material> materials;
	IEnumerator<Material> material;

	public Material fractureMaterial;

	public GameObject addonObject;

	public GameObject lineObject;

	List<GameObject> lines = new List<GameObject>();

	bool doFracture = false;
	bool doNormal = false;

	// Use this for initialization
	void Start () {
		material = materials.GetEnumerator();
		material.MoveNext();

		mesh = new Mesh();
		mesh.name = "Mesh";

		for(int i = 0; i < FaceConst.FACE_NUM_TRIANGLES; i++) {
			triangles[i * 3 + 0] = FaceConst.FaceTriangles[i, 0];
			triangles[i * 3 + 1] = FaceConst.FaceTriangles[i, 2];
			triangles[i * 3 + 2] = FaceConst.FaceTriangles[i, 1];
		}

		for(int i = 0; i < FaceConst.FACE_NUM_POINTS; i++) {
			uvs[i] = new Vector2(FaceConst.FaceUvs[i, 0] / FaceConst.FaceUvWidth, (768-FaceConst.FaceUvs[i, 1]) / FaceConst.FaceUvHeight);
			GameObject line = (GameObject)(GameObject.Instantiate(Resources.Load<GameObject>("LinePrefab"), Vector3.zero, Quaternion.identity));
			lines.Add(line);
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
		fingerObject.transform.position = fingerPosition;

		for( int i = 0; i < FaceConst.FACE_NUM_POINTS; i++) {
			vertices[i].x = verticesBack[i].x;
			vertices[i].y = verticesBack[i].y;
			vertices[i].z = verticesBack[i].z;
		}
		mesh.vertices = vertices;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		GetComponent<MeshFilter>().sharedMesh = mesh;
		GetComponent<MeshCollider> ().sharedMesh = mesh;

		addonObject.transform.position = facePosition;
		addonObject.transform.rotation = faceRotation;

		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			if(!material.MoveNext()) {
				material = materials.GetEnumerator();
				material.MoveNext();
			}
		}
		if (Input.GetKey("escape"))
			Application.Quit();

		GetComponent<Renderer>().material = material.Current;

		if (Input.GetKeyDown (KeyCode.F)) {
			doFracture = !doFracture;
		}
		if (Input.GetKeyDown (KeyCode.P)) {
			fingerObject.GetComponent<Renderer>().enabled = !fingerObject.GetComponent<Renderer>().enabled;
		}

		int index = Random.Range (0, FaceConst.FACE_NUM_TRIANGLES);
		if (doFracture && vertices [triangles [index * 3 + 0]].magnitude > 0) {
			GameObject fracture = new GameObject ();
			Mesh fractureMesh = new Mesh ();
			var fractureVertices = new Vector3[6];
			var fractureTriangles = new int[24];
			fractureVertices [0] = vertices [triangles [index * 3 + 0]];
			fractureVertices [1] = vertices [triangles [index * 3 + 2]];
			fractureVertices [2] = vertices [triangles [index * 3 + 1]];
			var upVector = Vector3.Cross (fractureVertices [1] - fractureVertices [0], fractureVertices [2] - fractureVertices [0]);
			upVector = upVector.normalized * 0.001f;
			fractureVertices [3] = fractureVertices [0] - upVector;
			fractureVertices [4] = fractureVertices [1] - upVector;
			fractureVertices [5] = fractureVertices [2] - upVector;
			fractureTriangles [0] = 0;
			fractureTriangles [1] = 1;
			fractureTriangles [2] = 2;
			fractureTriangles [3] = 3;
			fractureTriangles [4] = 5;
			fractureTriangles [5] = 4;

			fractureTriangles [6] = 0;
			fractureTriangles [7] = 3;
			fractureTriangles [8] = 1;
			fractureTriangles [9] = 4;
			fractureTriangles [10] = 1;
			fractureTriangles [11] = 3;
			fractureTriangles [12] = 1;
			fractureTriangles [13] = 4;
			fractureTriangles [14] = 2;
			fractureTriangles [15] = 5;
			fractureTriangles [16] = 2;
			fractureTriangles [17] = 4;
			fractureTriangles [18] = 2;
			fractureTriangles [19] = 5;
			fractureTriangles [20] = 0;
			fractureTriangles [21] = 3;
			fractureTriangles [22] = 0;
			fractureTriangles [23] = 5;
			fractureMesh.vertices = fractureVertices;
			fractureMesh.triangles = fractureTriangles;
			mesh.RecalculateNormals ();
			fracture.AddComponent<MeshFilter> ();
			fracture.AddComponent<MeshRenderer> ();
			fracture.GetComponent<MeshRenderer> ().material = fractureMaterial;
			fracture.AddComponent<FractureManager> ();
			fracture.GetComponent<MeshFilter> ().sharedMesh = fractureMesh;
			fracture.AddComponent<Rigidbody> ();
			//fracture.GetComponent<Rigidbody> ().isKinematic = true;
			fracture.GetComponent<Rigidbody> ().useGravity = true;
			//fracture.AddComponent<MeshCollider> ();
			//fracture.GetComponent<MeshCollider> ().sharedMesh = fractureMesh;
			//fracture.GetComponent<MeshCollider> ().convex = true;
			//fracture.GetComponent<MeshCollider> ().material = GetComponent<MeshCollider> ().material;
			fracture.transform.position = transform.position;
		}

		if (Input.GetKeyDown (KeyCode.N)) {
			doNormal = !doNormal;
		}

		if (doNormal) {
			normals = mesh.normals;
			for (int i = 0; i < normals.GetLength(0); i++) {
				if (uvs [i].x > 0.4 && uvs [i].x < 0.6) {
					var lineRenderer = lines[i].GetComponent<LineRenderer>();
					lineRenderer.SetPosition(0, vertices [i]);
					lineRenderer.SetPosition(1, vertices [i] + normals [5] * 0.05f);
					//lines[i].GetComponent<LineRenderer>() = lineRenderer;
				}
			}
		}
	}
	
	public void PacketReceivedEvent(OSCServer sender, OSCPacket packet) {
		if (packet.Address.Equals ("/osceleton/face_mesh")) {
			facePosition = Vector3.zero;
			for (int i = 0; i < FaceConst.FACE_NUM_POINTS; i++) {
				float x = -(float)packet.Data [i * 3 + 2] * FaceConst.FACE_SCALE;
				float y = (float)packet.Data [i * 3 + 3] * FaceConst.FACE_SCALE;
				float z = (float)packet.Data [i * 3 + 4] * FaceConst.FACE_SCALE;
				verticesBack [i] = new Vector3 (x, y, z);

				facePosition += verticesBack[i];
			}
			facePosition /= FaceConst.FACE_NUM_POINTS;
		} else if (packet.Address.Equals ("/osceleton/face")) {
			//facePosition.x = -(float)packet.Data[2] * FaceConst.FACE_SCALE * 0.001f;
			//facePosition.y = -(float)packet.Data[3] * FaceConst.FACE_SCALE * 0.001f;
			//facePosition.z = (float)packet.Data[4] * FaceConst.FACE_SCALE * 0.001f;

			Vector3 euler = new Vector3((float)packet.Data[5], (float)packet.Data[6], -(float)packet.Data[7]);
			faceRotation.eulerAngles = euler;
		} else if (packet.Address.Equals ("/osceleton/fingertip")) {
			fingerPosition.x = -(float)packet.Data[1];
			fingerPosition.y = (float)packet.Data[2];
			fingerPosition.z = (float)packet.Data[3];
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
