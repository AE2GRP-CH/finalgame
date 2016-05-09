using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class snowstate : NetworkBehaviour {

	private bool hasAut = false;

	public override void OnStartAuthority(){
		// if(isServer){
		// 	gameObject.SetActive(true);
		// }else{
		// 	hasAut = true;
		// }
	}

	void Start(){
	//	gameObject.SetActive(false);	
		if(!isServer){
			gameObject.transform.eulerAngles = new Vector3(270.0f,0,0);
		}

		// if(isClient && !hasAut){
		// 	gameObject.SetActive(false);
		// }else{
		// 	gameObject.SetActive(true);
		// }

	}




}
 