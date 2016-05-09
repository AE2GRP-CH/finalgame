using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class flashstate : MapItem {

	[SyncVar(hook = "KillObject")]
	private bool alive;
	
	protected override void OnTriggerEnter(Collider c){
		if(c.gameObject.tag == "pacman"){
			GetComponent<AudioSource>().Play();
			Invoke("DestroyObject",GetComponent<AudioSource>().clip.length-0.9f);
		}
	}

	private void KillObject(bool status){
		if(!status){
			Destroy(gameObject);
		}
	}

	private void DestroyObject(){
		alive = false;
	}
}
