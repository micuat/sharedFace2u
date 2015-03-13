using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

	Matrix4x4 intrinsics = new Matrix4x4();
	Matrix4x4 extrinsics = new Matrix4x4();

	static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far) {
		float x = 2.0F * near / (right - left);
		float y = 2.0F * near / (top - bottom);
		float a = (right + left) / (right - left);
		float b = (top + bottom) / (top - bottom);
		float c = -(far + near) / (far - near);
		float d = -(2.0F * far * near) / (far - near);
		float e = -1.0F;
		Matrix4x4 m = new Matrix4x4();
		m[0, 0] = x;
		m[0, 1] = 0;
		m[0, 2] = a;
		m[0, 3] = 0;
		m[1, 0] = 0;
		m[1, 1] = y;
		m[1, 2] = b;
		m[1, 3] = 0;
		m[2, 0] = 0;
		m[2, 1] = 0;
		m[2, 2] = c;
		m[2, 3] = d;
		m[3, 0] = 0;
		m[3, 1] = 0;
		m[3, 2] = e;
		m[3, 3] = 0;
		return m;
	}

	// Use this for initialization
	void Start () {
		// https://github.com/kylemcdonald/ofxCv/blob/master/libs/ofxCv/src/Calibration.cpp
		Debug.Log (GetComponent<Camera> ().projectionMatrix);
		float w = Screen.width;
		float h = Screen.height;
		float fx = 1.6994161051180934e+003f;
		float fy = 1.7015519527472304e+003f;
		float cx = 5.3168731922681593e+002f / 1024 * w;
		float cy = (9.2416198645433894e+002f) / 768 * h;
		float zNear = GetComponent<Camera> ().nearClipPlane;
		float zFar = GetComponent<Camera> ().farClipPlane;
		float left = zNear * (-cx) / fx;
		float right = zNear * (w - cx) / fx;
		float bottom = zNear * (cy) / fy;
		float top = zNear * (cy - h) / fy;

		intrinsics = PerspectiveOffCenter (left, right, -bottom, -top, zNear, zFar);

		GetComponent<Camera> ().projectionMatrix = intrinsics;

		extrinsics.SetRow (0, new Vector4 (9.6951375724054678e-001f, 9.4447770100172856e-002f,
		                                   2.2610328003906302e-001f, -3.3052667086500043e+002f));
		extrinsics.SetRow (1, new Vector4 (-2.3094250319200885e-002f, 9.5384408201206639e-001f,
		                                   -2.9941296366849657e-001f, -8.9743887308778181e+000f));
		extrinsics.SetRow (2, new Vector4 (-2.4394616234635083e-001f, 2.8506330162555693e-001f,
		                                   9.2694616021796250e-001f, 3.8313595147359138e+001f));
		extrinsics.SetRow (3, new Vector4 (0, 0, 0, 1));
		// ofxKinect is 1000x scaled
		extrinsics [0, 3] /= 1000;
		extrinsics [1, 3] /= 1000;
		extrinsics [2, 3] /= 1000;
		// https://github.com/benkuper/Mappity/blob/master/Assets/Mappity/Mappity.cs
		extrinsics[2,0] = -extrinsics[2,0];
		extrinsics[2,1] = -extrinsics[2,1];
		extrinsics[2,2] = -extrinsics[2,2];
		extrinsics[2,3] = -extrinsics[2,3];
//		Debug.Log (GetComponent<Camera> ().worldToCameraMatrix);
		GetComponent<Camera> ().worldToCameraMatrix = extrinsics;
		//extrinsics[1,0] = -extrinsics[1,0];
		//extrinsics[1,1] = -extrinsics[1,1];
		//extrinsics[1,2] = -extrinsics[1,2];
		//extrinsics[2,0] = -extrinsics[2,0];
		//extrinsics[2,1] = -extrinsics[2,1];
		//extrinsics[2,2] = -extrinsics[2,2];
		//extrinsics[0,2] = -extrinsics[0,2];
		//extrinsics[1,2] = -extrinsics[1,2];
		//extrinsics[2,2] = -extrinsics[2,2];
		//extrinsics[2,0] = -extrinsics[2,0];

		//GetComponent<Camera> ().worldToCameraMatrix = extrinsics;
		return;
//		extrinsics = extrinsics.inverse;
/*		extrinsics[2,0] = -extrinsics[2,0];
		extrinsics[2,1] = -extrinsics[2,1];
		extrinsics[2,2] = -extrinsics[2,2];
		extrinsics[2,3] = -extrinsics[2,3];*/
		Debug.Log (GetComponent<Camera> ().worldToCameraMatrix);
		transform.position = extrinsics.GetColumn(3);
		transform.rotation = Quaternion.LookRotation(
			extrinsics.GetColumn(2),
			extrinsics.GetColumn(1)
			);
		}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDrawGizmos () {
		DrawFrustum (GetComponent<Camera> ());
	}

	// http://forum.unity3d.com/threads/drawfrustum-is-drawing-incorrectly.208081/
	void DrawFrustum ( Camera cam ) {
		Vector3[] nearCorners = new Vector3[4]; //Approx'd nearplane corners
		Vector3[] farCorners = new Vector3[4]; //Approx'd farplane corners
		Vector3 center = new Vector3();
		Plane[] camPlanes = GeometryUtility.CalculateFrustumPlanes( cam ); //get planes from matrix
		Plane temp = camPlanes[1]; camPlanes[1] = camPlanes[2]; camPlanes[2] = temp; //swap [1] and [2] so the order is better for the loop
		
		for ( int i = 0; i < 4; i++ ) {
			nearCorners[i] = Plane3Intersect( camPlanes[4], camPlanes[i], camPlanes[( i + 1 ) % 4] ); //near corners on the created projection matrix
			farCorners[i] = Plane3Intersect( camPlanes[5], camPlanes[i], camPlanes[( i + 1 ) % 4] ); //far corners on the created projection matrix
		}
		center = Plane3Intersect (camPlanes [0], camPlanes [1], camPlanes [2]);
		
		for ( int i = 0; i < 4; i++ ) {
			Debug.DrawLine( nearCorners[i], nearCorners[( i + 1 ) % 4], Color.red, Time.deltaTime, true ); //near corners on the created projection matrix
			Debug.DrawLine( farCorners[i], farCorners[( i + 1 ) % 4], Color.blue, Time.deltaTime, true ); //far corners on the created projection matrix
			Debug.DrawLine( center, farCorners[i], Color.green, Time.deltaTime, true ); //sides of the created projection matrix
//			Debug.DrawLine( nearCorners[i], farCorners[i], Color.green, Time.deltaTime, true ); //sides of the created projection matrix
		}
	}
	
	Vector3 Plane3Intersect ( Plane p1, Plane p2, Plane p3 ) { //get the intersection point of 3 planes
		return ( ( -p1.distance * Vector3.Cross( p2.normal, p3.normal ) ) +
		        ( -p2.distance * Vector3.Cross( p3.normal, p1.normal ) ) +
		        ( -p3.distance * Vector3.Cross( p1.normal, p2.normal ) ) ) /
			( Vector3.Dot( p1.normal, Vector3.Cross( p2.normal, p3.normal ) ) );
	}
}
