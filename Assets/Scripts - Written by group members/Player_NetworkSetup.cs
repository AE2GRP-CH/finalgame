using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_NetworkSetup : NetworkBehaviour {

	[SerializeField]Camera FPSCharacterCam;
	[SerializeField]AudioListener audioListener;
	// Use this for initialization
	void Start () {
		if(isLocalPlayer)
		{
			GameObject.Find("Camera").SetActive(false);
			Debug.Log("enabled");
			CharacterController controller = GetComponent<CharacterController>();
			controller.enabled = true;
			GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled=true;
			FPSCharacterCam.enabled=true;
			//audioListener.enabled=true;
		}
	}
}
