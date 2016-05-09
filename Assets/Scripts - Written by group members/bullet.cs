using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class bullet : NetworkBehaviour {

//	[SyncVar(hook="updatePos")]
//	private Vector3 bulletPos;
	int s = 0;
	[SyncVar(hook="updateRot")]
	private Vector3 rot;

	[SyncVar(hook="setParent")]
	private NetworkInstanceId parentNetId;

	void setParent(NetworkInstanceId parent){
		if (!isServer) {
			GameObject gun = ClientScene.FindLocalObject (parent);
			GameObject tip = Utility.getChildWithTag (gun.transform, "gunTip");
			//transform.position = tip.transform.position;
			//transform.SetParent (gun.transform);
			transform.position = tip.transform.position;
		}
	}

	void Update(){
		if(s < 10){
//			Debug.Log("qwe123");
//			Debug.Log(gameObject.transform.localPosition);
//			s++;
		}
	}

	void setParentId(NetworkInstanceId parent){
		parentNetId = parent;
	}
		
	void updateRot(Vector3 rot){
		gameObject.transform.rotation = Quaternion.Euler (rot);
	}
		
	void setRot(Vector3 rot1){
		rot = rot1;
	}

	void OnCollisionEnter(Collision c){
		
		Debug.Log ("bullet collided");
		Debug.Log (c.collider.tag);
		if (c.collider.tag == "enemy") {
			Debug.LogError ("hit enemy1");
			Destroy (gameObject);
		}
//
		if (c.collider.tag == "Untagged") {
			Debug.LogError ("hit Untagged");
			Destroy (gameObject);
		}

	}

	void OnTriggerEnter(Collider other) {

		if (other.gameObject.tag == "enemy") {
			Debug.LogError ("hit enemy");
			Destroy (gameObject);
		}

		if (other.gameObject.tag == "bean") {
			Debug.LogError ("hit bean");
			Destroy (gameObject);
		}
		if (other.gameObject.tag == "bomb") {

			Debug.LogError ("hit bomb");
			Destroy (gameObject);
		}
	}
}
