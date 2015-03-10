using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityOSC;

public class FaceGenerator : MonoBehaviour {

	public GameObject faceMesh;

	// Use this for initialization
	void Start () {
		OSCHandler.Instance.Init();
	}
	
	// Update is called once per frame
	void Update () {
		OSCHandler.Instance.UpdateLogs();
		var servers = OSCHandler.Instance.Servers;
		
		foreach(var packet in servers["osceleton"].packets)
		{
			//Debug.Log(packet.Address);	
			if(packet.Address.Equals("/osceleton/face_mesh")) {
				var list = new List<Vector3>();
				for( int i = 0; i < FaceConst.FACE_NUM_POINTS; i++) {
					float scale = 100;
					float x = (float)packet.Data[i * 3 + 2] * scale;
					float y = (float)packet.Data[i * 3 + 3] * scale;
					float z = (float)packet.Data[i * 3 + 4] * scale;
					list.Add(new Vector3(x, y, z));
				}
				faceMesh.GetComponent<FaceManager>().SetMesh(list);
			}
		}
	}
}
