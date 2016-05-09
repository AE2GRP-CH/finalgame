using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MapItem : NetworkBehaviour {

	// Use this for initialization
	protected virtual void Start(){
		if(!isServer){
			gameObject.transform.eulerAngles = new Vector3(270.0f,0,0);
		}
	}

	protected virtual void OnTriggerEnter(Collider collide){}
	void Update () {}
}
