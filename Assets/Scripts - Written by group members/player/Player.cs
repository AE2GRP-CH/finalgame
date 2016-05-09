using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

	[SerializeField]
	Camera FPSCharacterCam;

	[SerializeField]
	AudioListener audioListener;

	GameObject camera = null;

	protected virtual void Start () {
		if (isLocalPlayer) {
			camera = GameObject.Find("Camera");
			camera.SetActive(false);
			GetComponent<CharacterController>().enabled=true;
			GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled=true;
			FPSCharacterCam.enabled=true;
			audioListener.enabled=true;
		}
	}
	
	protected virtual void OnDisable(){
		camera.SetActive(true);
		Invoke("EnableThirdCamera",3.0f);
	}

	void EnableThirdCamera(){
		if(GameObject.Find("CameraSwitcher") != null){
			GameObject.Find("CameraSwitcher").SendMessage("activate");
		}else{
			GameObject cameraSwitch = new GameObject("CameraSwitcher");
			cameraSwitch.AddComponent<CameraSwitcher>();
			cameraSwitch.SendMessage("activate");
		}
	}
}
