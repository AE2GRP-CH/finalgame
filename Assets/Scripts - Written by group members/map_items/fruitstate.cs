using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class fruitstate : MapItem {
	
	[SyncVar(hook = "KillObject")]
	private bool present;

	private void KillObject(bool status){
		if(!status){
			Destroy(gameObject);
		}
	}

	protected override void OnTriggerEnter(Collider x){

		if(x.gameObject.tag == "pacman"){
			AudioSource audio = GetComponent<AudioSource>();
			if(isServer){
				Invoke("kill",audio.clip.length-0.3f);
			}
        	audio.Play();  
		}
	}

	void kill(){
		present = false;
	}
}
