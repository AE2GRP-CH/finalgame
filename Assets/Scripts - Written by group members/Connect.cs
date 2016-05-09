using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Connect : MonoBehaviour {

	void Start () {
		GameObject obj = (GameObject)Resources.Load("test_fruit",typeof(GameObject));
		GameObject bomb = (GameObject)Resources.Load("flash",typeof(GameObject));
		ClientScene.RegisterPrefab(obj);
		ClientScene.RegisterPrefab(bomb);
		if(GlobalData.option == 0){
			NetworkManager.singleton.networkPort = 7777;
			NetworkManager.singleton.StartHost();
		}else{
			NetworkManager.singleton.networkAddress = GlobalData.connect_Ip;
			NetworkManager.singleton.networkPort = 7777;
			NetworkManager.singleton.StartClient();
		}
	}
	
	void Update () {
	
	}
}
