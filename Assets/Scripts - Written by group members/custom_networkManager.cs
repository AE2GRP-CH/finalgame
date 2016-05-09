using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class custom_networkManager : NetworkManager {

//	public string networkAddess = "192.168.1.101";
	private Dictionary<int,string> nicknames;
	private Dictionary<Vector3,bool> spawn_position = new Dictionary<Vector3,bool>();

	public override void OnServerAddPlayer(NetworkConnection conn, short id){

		Debug.Log(string.Format("asdasd"));
//
		GameObject obj1 = null;
		if(conn.connectionId < 1){
			obj1 = Instantiate(this.playerPrefab)as GameObject;
			obj1.transform.position = this.GetStartPosition ().position;
			spawnObjects("bomb",getBombPositions());
			spawnObjects("fruit",getFruitPositions());
			spawnObjects("flash",getFlashPositions());
			spawnObjects("snow",getSnowPositions());
			spawnObjects("gun",getGunPositions());
		}else{
			obj1 = (GameObject)Instantiate(Resources.Load("ghost",typeof(GameObject)));
			// GameObject obj11 = GameObject.FindGameObjectWithTag("startpositions");
			foreach(KeyValuePair<Vector3,bool> pos in spawn_position){
				if(!pos.Value){
					obj1.transform.position = pos.Key;
					spawn_position[pos.Key] = true;
					break;
				}
			}

		}

		NetworkServer.AddPlayerForConnection(conn,obj1, id);
	}

	public override void OnClientConnect(NetworkConnection conn){
		Debug.Log("player connected");
		ClientScene.AddPlayer(conn,0);
	}

	private void addNickname(int id,Transform parent){
		string name="";
		GameObject text_nickname = new GameObject("nickname");
		//text_nickname.AddComponent<Transform>();
		text_nickname.AddComponent<MeshRenderer>();
		TextMesh text1 = text_nickname.AddComponent<TextMesh>();
		if(nicknames.TryGetValue(id,out name)){
			text1.text = name;
		}
		text1.fontSize = 500;
		text1.characterSize = 0.00015F;
		text1.color = new Color(0.2F, 0.3F, 0.4F, 0.5F);
		text1.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
		text1.alignment = TextAlignment.Center;
		text1.anchor = TextAnchor.MiddleCenter;
		text_nickname.transform.SetParent(parent);
		text_nickname.transform.localPosition = new Vector3((float)0.06,(float)1.37,(float)0.04);
	}

	public override void OnStartClient(NetworkClient client){
		GameObject bomb2 = (GameObject)Resources.Load("bomb",typeof(GameObject));
		ClientScene.RegisterPrefab(bomb2);

		GameObject ghost = (GameObject)Resources.Load("ghost",typeof(GameObject));
		ClientScene.RegisterPrefab(ghost);

		GameObject gun = (GameObject)Resources.Load("gun",typeof(GameObject));
		ClientScene.RegisterPrefab(gun);

		GameObject bullet = (GameObject)Resources.Load("bullet2",typeof(GameObject));
		ClientScene.RegisterPrefab(bullet);
	}



	public override void OnClientSceneChanged(NetworkConnection conn) {
		base.OnClientSceneChanged(conn);
		//Debug.Log(conn.connectionId);
		if(conn.connectionId < 0){
			GameObject[] startpos = GameObject.FindGameObjectsWithTag("startpositions");
			//Debug.Log(startpos.Length);
			 foreach(GameObject pos in startpos){
				//Debug.Log(string.Format(pos.transform.position.ToString()));
				spawn_position.Add(pos.transform.position,false);
		    }
		}
    }

    private void spawnObjects(string item, GameObject[] positions){

    	string itemname = null;

    	if(item == "fruit"){
    		itemname = "test_fruit";
    	}else if(item == "flash"){
    		itemname = "flash";
    	}else if(item == "snow"){
    		itemname = "snow";
    	}else if(item == "bomb"){
    		itemname = "bomb";
    	}else if(item == "gun"){
    		itemname = "gun";
    	}


    	int i = 0;
    	foreach(KeyValuePair<NetworkHash128,GameObject> item1 in ClientScene.prefabs){
    		if(item1.Value.name == itemname){
    			while(i < positions.Length){
    				Transform trans1 = positions[i].transform;
					GameObject obj = Instantiate(item1.Value,trans1.position,item1.Value.transform.rotation)as GameObject;
					obj.transform.eulerAngles = trans1.eulerAngles;
					NetworkServer.Spawn(obj);
					i++;
    			}
				
    		}

    	}
    }

    GameObject[] getPlayerStartPositions(){
    	GameObject[] positions = GameObject.FindGameObjectsWithTag("startpositions");
    	return positions;
    }

    GameObject[] getFlashPositions(){
    	GameObject[] positions = GameObject.FindGameObjectsWithTag("flashpositions");
    	return positions;
    }

     GameObject[] getBombPositions(){
    	GameObject[] positions = GameObject.FindGameObjectsWithTag("bombposition");
    	return positions;
    }

    GameObject[] getFruitPositions(){
    	GameObject[] position = GameObject.FindGameObjectsWithTag("fruitpositions");
    	return position;
    }

    GameObject[] getSnowPositions(){
    	GameObject[] position = GameObject.FindGameObjectsWithTag("snowpositions");
    	return position;
    }

    GameObject[] getGunPositions(){
    	GameObject[] position = GameObject.FindGameObjectsWithTag("gunpositions");
    	return position;
    }

    public override void OnStartHost(){

    	nicknames = LobbyManager.getInstance().getNickNameCollection();
    	Debug.Log("asd123123");
    }

}
