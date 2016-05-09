using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Net.Sockets;

public class nickname : MonoBehaviour {

    public InputField nickname1;
	private string playerName;
	private EmptyMessage msg1 = new EmptyMessage();


	void Awake(){
		nickname1.contentType = InputField.ContentType.Alphanumeric;
		nickname1.characterLimit = 10;
		nickname1.onEndEdit.AddListener (delegate {
			playerName = nickname1.textComponent.text;
		});
		nickname1.onValueChanged.AddListener(delegate{
			GameObject.Find("audio").GetComponent<AudioSource>().Play();
		});
	}
		
	public void next()
	{
		
		if (GlobalData.option == 0) {
			LobbyManager lobby = LobbyManager.getInstance ();
			string ip = lobby.startServer ();
			// Text obj = GameObject.Find ("testIP").GetComponent<Text> ();
			// obj.text = ip;
			GlobalData.client = ClientScene.ConnectLocalServer ();
			SceneManager.LoadScene ("lobby");
		} else {

			GlobalData.client = new NetworkClient();
			GlobalData.client.Connect (GlobalData.connect_Ip, 4444);
			GlobalData.client.RegisterHandler(MsgType.Connect, OnConnected);
			GlobalData.client.RegisterHandler(MsgType.Error, OnError);
			GlobalData.client.RegisterHandler (Message.GET_HAS_ROOM, OnGetRoom);
		}
		GlobalData.nickname = playerName;
	}

	void OnConnected(NetworkMessage msg)
	{

		nicknameMessage msg2 = new nicknameMessage ();
		msg2.nickname = playerName;
		GlobalData.client.Send (Message.GET_HAS_ROOM, msg1);
	}

	void OnGetRoom(NetworkMessage msg)
	{
		bool hasRoom = msg.ReadMessage<hasRoomMessage> ().hasRoom;
		if (hasRoom) {
			SceneManager.LoadScene("lobby");
		} else {
			showErrorDialog(true);
		}
	}

	void OnError(NetworkMessage msg)
	{
		Debug.Log (string.Format ("error occured"));
		showErrorDialog (false);		
	}

	void showErrorDialog(bool hasNoRoom)
	{
		if (hasNoRoom) {

		}
	}

}
