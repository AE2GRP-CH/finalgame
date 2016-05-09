using UnityEngine;
using System.Collections;

public class testRigidBody : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log ("asdasd");
		GetComponent<Rigidbody> ().AddForce (transform.up * 30.0f);
	}
}
