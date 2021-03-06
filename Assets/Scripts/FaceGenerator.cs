﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityOSC;

public class FaceGenerator : MonoBehaviour {

	public GameObject faceMesh;

	// Use this for initialization
	void Start () {
		Physics.gravity = new Vector3 (0, -0.1f, 0);

		OSCHandler.Instance.Init();
		OSCHandler.Instance.CreateServer ("osceleton", 57121, new PacketReceivedEventHandler(faceMesh.GetComponent<FaceManager>().PacketReceivedEvent));
	}
	
	// Update is called once per frame
	void Update () {
	}
}
