using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Net.Sockets;

public class GameListGUI:Observer{

	GameObject mainPanel;
	bool isListening = false;
	CustomNetworkDiscovery discover;

	void Start(){

		this.subject = DiscoverDispatcher.getInstance();
		this.subject.attach(this);
		Button closeButton = gameObject.transform.Find("Panel/Image/Button").gameObject.GetComponent<Button>();

		closeButton.onClick.AddListener(delegate{
			gameObject.SetActive(false);
			discover.StopBroadcast();
			isListening = false;
		});
	}

	void Update(){
		if(!isListening){
			Debug.LogError(string.Format("start to listen"));

			if(GameObject.Find("test") == null){
				Debug.LogError("new item");
				GameObject obj = new GameObject("test");
				discover = obj.AddComponent<CustomNetworkDiscovery>();
				discover.showGUI = false;
				discover.useNetworkManager = false;
				discover.broadcastPort = 4444;
			}else{
				Debug.LogError("remove");
				discover.remove();
			}

			if(discover.Initialize()){
				discover.StartAsClient();
			}
			isListening = true;
		}
	}

	public void showGUI(){
		gameObject.SetActive(true);
	}

	public override void update(){

		GameObject list = GameObject.Find("Panel1/gameList");

		int i = 0;
		while(i < list.transform.childCount){
			Transform parent = list.transform.GetChild(i);

			if(parent.childCount == 0){

				GameListItem btnItem = parent.gameObject.AddComponent<GameListItem>();

				int count = i+1;
				TextStruct txtStruct = new TextStruct();
				txtStruct.setupTextStruct("Local Game " + count.ToString(), 16, new Color(1.0f,1.0f,1.0f,1.0f),new Vector2(0.5f,0.5f),new Vector2(0.5f,0.0f),TextAnchor.MiddleLeft);
				GameObject obj = Utility.initTextWithRelationship(txtStruct,parent.transform);
				obj.GetComponent<RectTransform>().localPosition = new Vector3(-125.0f,0.0f,0);

				ButtonStruct bStruct = new ButtonStruct();
				bStruct.createButton(new Color(1.0f,1.0f,1.0f,1.0f),"exit");

				TextStruct txtStruct1 = new TextStruct();
				txtStruct1.setupTextStruct("Join Game", 14, new Color(1.0f,1.0f,1.0f,1.0f),new Vector2(0.5f,0.5f),new Vector2(0.5f,0.0f),TextAnchor.MiddleCenter);

				GameObject buttonObject = Utility.createButton(bStruct,txtStruct1,FontStyle.Bold);
				buttonObject.transform.SetParent (parent);
				buttonObject.transform.GetChild(0).localPosition = new Vector3(0,-16,0);

				buttonObject.GetComponent<RectTransform>().sizeDelta = new Vector2 (85, 35);
				buttonObject.GetComponent<RectTransform>().localPosition = new Vector3 (110, 0, 0);


				btnItem.Button = buttonObject;
				btnItem.Text = obj;
				btnItem.IP = this.subject.getState();
				break;
			}
			i++;
		}
		Debug.Log(string.Format(this.subject.getState()));
	}
}