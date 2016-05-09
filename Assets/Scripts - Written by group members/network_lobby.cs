using UnityEngine;
using System.Collections;
using System.Net;
using System;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class network_lobby : MonoBehaviour {


	public Dropdown dropdown;
	public Image map_image;
	public Button cancel_button;
	public Button ok_button;
	private GameObject discoverObject;

	void Awake () {

		EmptyMessage msg = new EmptyMessage ();

		GlobalData.client.RegisterHandler (Message.GET_CLIENTS, OnGetClients);
		GlobalData.client.RegisterHandler (Message.SEND_NICKNAME, AddOpponents);
		GlobalData.client.Send (Message.GET_CLIENTS, msg);
		GlobalData.client.RegisterHandler (Message.MESSAGE_LIST, OnGetMessageList);
		GlobalData.client.RegisterHandler (Message.REMOVE_NICKNAME, OnRemovePlayer);
		GlobalData.client.RegisterHandler (Message.CHANGE_DURATION, OnChangeDuration);
		GlobalData.client.RegisterHandler(Message.START_GAME, OnStartGame);
		GlobalData.client.RegisterHandler(MsgType.Disconnect,OnDisconnected);

		if (LobbyManager.getInstance ().isActive ()) {

			hasRoomMessage msg1 = new hasRoomMessage();
			msg1.hasRoom = true;
			GlobalData.client.Send(Message.SET_HAS_ROOM, msg1);

			dropdown.onValueChanged.AddListener (delegate {
				changeDropDown (dropdown);
			});

			cancel_button.onClick.AddListener (delegate {
				LobbyManager.getInstance().shutdownServer();
				SceneManager.LoadScene("Main_menu");
			});

			ok_button.onClick.AddListener (delegate {
				GlobalData.client.Send (Message.START_GAME, msg);
				PlayerPrefs.SetInt("Timer",Int32.Parse(dropdown.captionText.text));
			});

			if(GameObject.Find("discovery") == null){
				discoverObject = new GameObject("discovery");
				NetworkDiscovery discoverComponent = discoverObject.AddComponent<NetworkDiscovery> ();
				discoverComponent.broadcastPort = 4444;
				discoverComponent.showGUI = false;
				discoverComponent.useNetworkManager = false;
				if (discoverComponent.Initialize ()) {
					discoverComponent.StartAsServer ();
				}
			}
		}
		else {
			
			dropdown.enabled = false;

			cancel_button.onClick.AddListener (delegate {
				GlobalData.client.Send(Message.QUIT_GAME,msg);
				SceneManager.LoadScene("Main_menu");
			});
			
			GlobalData.client.Send (Message.GET_DURATION, msg);
		}
	}

	void OnStartGame(NetworkMessage msg){
		
		Debug.Log(string.Format("start game"));
		SceneManager.LoadScene("test");
	}

	void OnRemovePlayer(NetworkMessage msg){

		GameObject list = GameObject.Find ("Imagelist");
		string nickname = msg.ReadMessage<nicknameMessage> ().nickname;

		int i = 0;
		while (i < list.transform.childCount) {
			Transform parent = list.transform.GetChild (i);
			if (parent.childCount != 0) {

				Text name = parent.GetChild (0).gameObject.GetComponent<Text> ();
				if(name.text == nickname){

					GameObject.Destroy (parent.GetChild (0).gameObject);
					//GameObject.Destroy (parent.GetChild (1).gameObject);
				} 
			}
			i++;
		}
	}

	void OnGetMessageList(NetworkMessage msg){

		string[] mList = msg.ReadMessage<messageListMessage> ().message;
		GameObject list = GameObject.Find ("messagelist");

		int i = 0;
		int j = 0;

		while (i < mList.Length) {
			Transform parent = list.transform.GetChild (j);
			if (parent.childCount == 0) {
				int num = j + 1;
				string entry = "[Message " + num.ToString () + "]: " + mList [mList.Length == 1?i:j];
				addToList (entry, parent, new Vector3 (-190.0f, 0, 0));

				if (j == mList.Length - 1) {
					break;
				}
				i++;
			}
			j++;
		}
	}
		

	void OnGetClients(NetworkMessage msg){

		GameObject imagelist = GameObject.Find("Imagelist");
		string[] names = msg.ReadMessage<ClientListMessage>().names;
	
		int i = 0;
		while(i < names.Length){
			Transform parent = imagelist.transform.GetChild (i);
			if (parent.childCount == 0) {
				addToList (names [i], parent,new Vector3(-190.0f,-3,0));

			} else {
				GameObject obj = parent.GetChild (0).gameObject;
				Text txt1 = obj.GetComponent<Text>();
				if (txt1.text != names [i]) {
					txt1.text = names [i];
				}
			}
			i++;
		}


		ButtonStruct bStruct = new ButtonStruct();
		bStruct.createButton(new Color(1,1,1,1),"exit");

		TextStruct tStruct = new TextStruct();
		tStruct.setupTextStruct("Ready",15,new Color(255,255,255,255),new Vector2(0.5f,0.5f),new Vector2(0.5f,0.5f),TextAnchor.MiddleCenter);

		GameObject b = Utility.createButton(bStruct,tStruct,FontStyle.Normal);
		Button bComp = b.GetComponent<Button>();
		bComp.onClick.AddListener(delegate{
			bComp.enabled = false;
			EmptyMessage msg1 = new EmptyMessage();
			GlobalData.client.Send(Message.SET_READY,msg1);
		});


		Transform parent1 = imagelist.transform.GetChild (i);
		addToList (GlobalData.nickname,parent1,new Vector3(-190.0f,-3,0));

		b.transform.SetParent(parent1);
		RectTransform trans = b.GetComponent<RectTransform>();
		trans.sizeDelta = new Vector2 (85, 40);
		trans.localPosition = new Vector3 (170, 0, 0);

		nicknameMessage msg2 = new nicknameMessage();
		msg2.nickname = GlobalData.nickname;
		GlobalData.client.Send (Message.SEND_NICKNAME, msg2);
	}

	void AddOpponents(NetworkMessage msg){

		string name = msg.ReadMessage<nicknameMessage> ().nickname;
		GameObject imagelist = GameObject.Find("Imagelist");

		for (int i = 0; i < imagelist.transform.childCount; i++) {
			Transform parent = imagelist.transform.GetChild (i);
			if (parent.childCount == 0) {
				addToList (name, parent, new Vector3(-190.0f,-3,0));
				break;
			} else {
				Text obj = parent.GetChild (0).gameObject.GetComponent<Text> ();
				if (obj.text == name) {
					break;
				}
			}
		}
	}

	void addToList(string name, Transform parent, Vector3 pos){
		
		TextStruct struct1 = new TextStruct();
		struct1.setupTextStruct(name,18,new Color(1.0f,1.0f,1.0f,1.0f),new Vector2(0.5f,0.0f),new Vector2(0.5f,0.0f),TextAnchor.MiddleLeft);

		GameObject text = Utility.initTextWithRelationship(struct1,parent);
		text.GetComponent<RectTransform>().localPosition = pos;
	}

	// ------------------------- Shutdown from Server -------------------- //

	void OnDisconnected(NetworkMessage msg1){
	// 	Debug.Log(string.Format("disconnect message"));
	// 	Debug.LogError("disconnect message");
	// 	//GameObject.Find("testDisconnect").GetComponent<Text>().text = "disconnect";
		SceneManager.LoadScene ("Main_menu");
	}

	// ------------------------- Change Map -------------------- //
		
	void OnChangeDuration(NetworkMessage msg){
		int value = msg.ReadMessage<gameDurationMessage> ().dropdownVal;
		dropdown.value = value;
	}
		
	private void changeDropDown(Dropdown target){
		gameDurationMessage msg = new gameDurationMessage ();
		msg.dropdownVal = target.value;
		GlobalData.client.Send (Message.CHANGE_DURATION, msg);
	}


	// ------------------------- Local client connect -------------------- //


	void OnApplicationQuit(){
		
//		localClient.Send (Message.SHUTDOWN, new EmptyMessage ());
	}
}
