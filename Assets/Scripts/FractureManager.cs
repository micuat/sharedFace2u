using UnityEngine;
using System.Collections;

public class FractureManager : MonoBehaviour {

	public float threshold = -2f;
	// Use this for initialization
	void Start () {
//		GetComponent<Rigidbody> ().AddForce (new Vector3(0, 0, -2));
//		GetComponent<Rigidbody> ().AddTorque (new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0));
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y < threshold) {
			Destroy (gameObject);
		}
	}
}
