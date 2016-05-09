using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

public class CameraSwitcher : NetworkBehaviour{

	List<GameObject> players = null;
	int prevKey = -1;
	bool active = false;
	GameObject activePlayer = null;
	public GameObject camera;

	public GameObject printer;

	void Start(){
		Debug.Log (KeyCode.Space);
	}

	void Update(){
		if (active) {
			if(Input.GetKeyDown(KeyCode.Alpha1)){
				if(players.Count >= 1){
					activateCamera(0);
				}
			}

			if(Input.GetKeyDown(KeyCode.Alpha2)){
				if(players.Count >= 2){
					activateCamera(1);
				}
			}

			if(Input.GetKeyDown(KeyCode.Alpha3)){
				if(players.Count >= 3){
					activateCamera(2);
				}
			}
		}
	}

	void activateCamera(int key){

		GameObject thirdcam = Utility.getChildWithTag(players[key].transform,"thirdpersonCam");

		if (thirdcam == null) {
			Debug.LogError ("Thirdcam error");
		} else {
			Debug.LogError ("Thirdcam success");
		}

		if(key != prevKey){
			if(prevKey != -1){
				GameObject prevCam = Utility.getChildWithTag(players[prevKey].transform,"thirdpersonCam");
				if (prevCam == null) {
					Debug.LogError ("prev null");
				}
				prevCam.SetActive(false);
			}

			activePlayer = players [key];
			thirdcam.SetActive(true);
			prevKey = key;
		}
	}
		

	void activate(){

		GameObject[] p = GameObject.FindGameObjectsWithTag ("pacman");
		GameObject[] enemy = GameObject.FindGameObjectsWithTag ("enemy");

		players = new List<GameObject> (p);
		players.AddRange (new List<GameObject> (enemy));

		Debug.LogError ("Number is " + players.Count.ToString ());

		if (players.Count > 1) {
			printer.SendMessage ("clearCanvasElement");
			Debug.LogError("clear canvas");
			active = true;
			Debug.LogError("clear canvas1");
			GameObject thirdcam = Utility.getChildWithTag (players [0].gameObject.transform, "thirdpersonCam");
			if(thirdcam == null){
				Debug.LogError("null thirdcam");
			}else{
				thirdcam.SetActive (true);
			}
			
		} else {
			gameObject.SetActive (false);
		}
	}
}