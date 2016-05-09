using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ghost : Player {

	float time = 5;


	bool isFrozen = false;

	void OnTriggerEnter(Collider x){
		if(x.gameObject.tag != gameObject.tag){
			Debug.Log(x.gameObject.tag);
		}
		
		if(x.gameObject.tag == "pacman"){
			if(!x.gameObject.GetComponent<Pacman>().monster){
				CmdCollidePacman(x.gameObject);
			}
		}

		if(x.gameObject.tag == "bullet1"){
			Debug.LogError("hit by bullet");
			changeMovement(true);
		}
	}

// 	void OnCollisionEnter(Collision c){
// 		if (c.collider.tag == "enemy") {
// 			Debug.LogError ("hit enemy2");
// 		}
// //
// 		if (c.collider.tag == "Untagged") {
// 			Debug.LogError ("hit Untagged");
// 		}
// 	}

	void Update(){
		if(isFrozen){
			time -= Time.deltaTime;
			if(time < 0.0f || time == 0.0f){
			 	changeMovement(false);
		 	}	
		}
	}

	protected override void OnDisable(){
		base.OnDisable();
		CmdKillGhost(gameObject);
	}

	void changeMovement(bool disable){
		if(disable){
			GetComponent<CharacterController>().enabled = false;
			GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled=false;
			isFrozen = true;
		}else{
			GetComponent<CharacterController>().enabled = true;
			GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled=true;
			isFrozen = false;
		}
	}
	
	[Command]  
	void CmdCollidePacman(GameObject pacman){
		GameObject.Find("manager").SendMessage("PacmanCollided",pacman); 
	}

	[Command]
	void CmdKillGhost(GameObject obj){
		obj.SetActive(false);
	}

}
