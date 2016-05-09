using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;

public class Pacman : Player {
	 
	private int points = 0;
	private int num_of_bombs = 0;
	GameObject printer;

	[SyncVar(hook="becomeMonster")]
	private bool isMonster;
	public bool monster = false;


	int oneGun = 0;

	protected override void Start(){
		base.Start();
		printer = GameObject.Find("printer");
	}

	void OnTriggerEnter(Collider x){
		if(x.gameObject.tag == "bean"){
			points+=10;
			printer.SendMessage("PrintScore",points.ToString());
		}

		if(x.gameObject.tag == "bomb"){
			num_of_bombs++;
			printer.SendMessage("addBombImage");	
		}

		if(x.gameObject.tag == "flash"){
			isMonster = true;
		}

		if(isMonster && x.gameObject.tag == "enemy"){
			CmdCollideGhost(x.gameObject);
		}

		// spawn gun
		if(x.gameObject.tag == "gun" && oneGun == 0){
			if(isLocalPlayer){
				foreach(KeyValuePair<NetworkHash128,GameObject> item in ClientScene.prefabs){
					if(item.Value.name == "gun"){
						GameObject gun = (GameObject)Instantiate(item.Value);
						gun.SendMessage("setParentId",gameObject.GetComponent<NetworkIdentity>().netId);
						NetworkServer.SpawnWithClientAuthority(gun,gameObject);
						gun.GetComponent<Collider>().enabled = false;
						gun.transform.SetParent(gameObject.transform.GetChild(0));
						gun.transform.localPosition = new Vector3(0.6f,-0.439f,0.511f);
						gun.transform.localRotation = Quaternion.Euler(0, 90.0f, 0);
						oneGun++;
						break;
					}
				}
			}
		}
	}

	// change to Hulk
	void becomeMonster(bool x){
		monster = x;
		GameObject model = Utility.getChildWithTag(gameObject.transform,"model");
		Material mat = (Material)Resources.Load("powerup",typeof(Material));
		model.GetComponent<Renderer>().material = mat;
	}

	[Command]
	void CmdCollideGhost(GameObject ghost){
		GameObject.Find("manager").SendMessage("GhostCollided",ghost);
	}

	// for debugging purposes
	void OnDrawGizmosSelected(){
		Gizmos.color = Color.red;
		Debug.Log(gameObject.transform.position);

		Vector3 playerPos = gameObject.transform.position;
 		Vector3 playerDirection = gameObject.transform.forward;
 		Quaternion playerRotation = gameObject.transform.rotation;
 		float spawnDistance = 10.0f;
 		Vector3 spawnPos = playerPos + playerDirection*spawnDistance;
		Gizmos.DrawWireSphere (gameObject.transform.position, 17.0f);
	}

	void Update(){

		// play music when near the ghost
		if(isServer){
			Vector3 playerPos1 = gameObject.transform.position;
 			Vector3 playerDirection1= gameObject.transform.forward;
 			Quaternion playerRotation1 = gameObject.transform.rotation;
 			float spawnDistance1 = 2.0f;
 			Vector3 spawnPos1 = playerPos1+ playerDirection1*spawnDistance1;


			Collider[] nearby = Physics.OverlapSphere(gameObject.transform.position, 17.0f);
			Collider[] x = nearby.Where(c => c.gameObject.tag == "enemy").ToArray();
			if(x.Length > 0){
				GameObject m = Utility.getChildWithTag(gameObject.transform,"sound");
				m.GetComponent<AudioSource>().enabled = true;
				if(!m.GetComponent<AudioSource>().isPlaying){
					m.GetComponent<AudioSource>().Play();
				}
			
			}else{
				GameObject m = Utility.getChildWithTag(gameObject.transform,"sound");
				if(m.GetComponent<AudioSource>().isPlaying){
					m.GetComponent<AudioSource>().Pause();
				}
			}
  		}
    	
  		// spawn Bomb
		if(Input.GetKeyDown(KeyCode.X) && num_of_bombs > 0){
			Vector3 playerPos = gameObject.transform.position;
 			Vector3 playerDirection = gameObject.transform.forward;
 			Quaternion playerRotation = gameObject.transform.rotation;
 			float spawnDistance = 2.0f;
 			Vector3 spawnPos = playerPos + playerDirection*spawnDistance;

			if(isLocalPlayer){
				printer.SendMessage("removeBombImage");
				num_of_bombs--;
				foreach(KeyValuePair<NetworkHash128,GameObject> item in ClientScene.prefabs){
					if(item.Value.name == "bomb"){
						GameObject bomb = (GameObject)Instantiate(item.Value,spawnPos,new Quaternion(270f,0f,0f,0f));
						bomb.AddComponent<Rigidbody>();
						SphereCollider collider = bomb.GetComponent<SphereCollider>();
						collider.isTrigger = false;
						collider.material = (PhysicMaterial)Resources.Load("bouncy",typeof(PhysicMaterial));
						bomb.transform.position = spawnPos;
						bomb.transform.eulerAngles = new Vector3(270f,0,0);
						NetworkServer.Spawn(bomb);
						bomb.SendMessage("activateBomb");
						break;
					}
				}
			}
	}
}

}
