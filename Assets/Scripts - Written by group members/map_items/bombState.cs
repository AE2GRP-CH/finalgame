using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class bombState : MapItem {

	private float time = 6;

	[SyncVar(hook="updatePosition")]
	private Vector3 pos;

	[SyncVar(hook="updateRotation")]
	private Quaternion rotation;

	[SyncVar(hook="PlayExplode")]
	private bool explode;

	[SyncVar]
	private bool activated;

	[SyncVar(hook="KillObject")]
	private bool alive;

	void updatePosition(Vector3 pos){ 
		gameObject.transform.position = pos;
	}
	void updateRotation(Quaternion rot){ 
		gameObject.transform.rotation = rot; 
	}

	protected override void Start(){
		base.Start();
	}

	protected override void OnTriggerEnter(Collider c){
		if(c.gameObject.tag == "pacman" && !activated){
			GetComponent<AudioSource>().Play();
			Invoke("changeState",GetComponent<AudioSource>().clip.length-0.9f);
		}
	}

	void changeState(){
		alive = false; 
	}

	void KillObject(bool a){
		if(!a){ Destroy(gameObject); }
	}

	void PlayExplode(bool status){
		if(status){
			GameObject explosionEffect = Utility.getChildWithTag(gameObject.transform,"explosion");
			explosionEffect.SetActive(true);
			GetComponent<AudioSource>().clip = (AudioClip)Resources.Load("sounds/bomb_edited");
			GetComponent<AudioSource>().Play();
			Invoke("Kill",gameObject.GetComponent<AudioSource>().clip.length-0.9f);
		}
	}

	void Kill(){
		Destroy(gameObject);
	}

	void activateBomb(){
		if(isServer){
			activated = true;
		}
	}

	void Update () {
		if(isServer && activated){
			pos = gameObject.transform.localPosition;
			rotation = gameObject.transform.rotation;
			time -= Time.deltaTime;
			if(time < 0.0f || time == 0.0f){
				explode = true;
				activated = false;
				GameObject.Find("manager").SendMessage("BombExploded",gameObject.transform.position);
			}
		}
	}




}
