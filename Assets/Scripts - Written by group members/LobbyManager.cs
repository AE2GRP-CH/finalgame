using UnityEngine;
using System.Collections;
using System.Net;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Net.Sockets;

using UnityEngine.Networking.NetworkSystem;


public class ClientListMessage:MessageBase{

//	public Dictionary<int,string> clients;
	public string[] names;

}

public class messageListMessage:MessageBase{

	public string[] message;
}


/*
 * Class: EmptyMessage
 * 
 * A MessageBase subclass 

*/

public class EmptyMessage : MessageBase
{
	
}

public class nicknameMessage : MessageBase{ public string nickname; }

/*
  Class: connectionCountMessage

  A MessageBase subclass to tell the number of players
  
*/

public class connectionCount : MessageBase
{
	private int playerCount;
}

/*

  Class: hasRoomMessage

  A MessageBase subclass to tell the availability of a room.

*/

class hasRoomMessage : MessageBase
{
	public bool hasRoom;
}

public class connectionIdMessage : MessageBase
{
	public int connectionId;
}

/*
  	Class: mapMessage
  	
  	A MessageBase subclass to toll players a changed in map choosen by host
*/

class gameDurationMessage : MessageBase
{
	public int dropdownVal;

}

/*
	Class: LobbyManager
	A class that acts as the LobbyManager
*/

public class LobbyManager{

	private bool hasRoomAvailable;
	private int currentHost;
	private int gameDurationValue = 0;
	private string serverIp;
	private Dictionary<int,string> nickname = new Dictionary<int,string> ();
	private Dictionary<string,int> status = new Dictionary<string,int> ();
	private static LobbyManager singleton;
	private List<string> message = new List<string> ();

	public static LobbyManager getInstance(){

		if (LobbyManager.singleton == null) {
			LobbyManager.singleton = new LobbyManager ();
		}
		return LobbyManager.singleton;
	}

	public string getServerIp(){
		return serverIp;
	}

	public Dictionary<int,string> getNickNameCollection(){
		return nickname;
	}


	/*
	  Function: startServer

	  Create and instance of a NetworkServer and register handler for events

      Parameters:

          None

      Returns:

          void
	*/


	public string startServer()
	{

		IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
		foreach (IPAddress item in localIPs) {
			Debug.Log (string.Format (item.ToString ()));
			if (item.AddressFamily == AddressFamily.InterNetwork) {
				serverIp = item.ToString ();	

			}
		}
		//Debug.Log (string.Format (localIPs [0].ToString ()));
		if(!NetworkServer.active){
			NetworkServer.Shutdown();
			NetworkServer.Listen (serverIp,4444);
		}
		

		NetworkServer.RegisterHandler (Message.CHANGE_DURATION, OnChangeDuration);
		NetworkServer.RegisterHandler(Message.SET_HAS_ROOM, OnSetHasRoom);
		NetworkServer.RegisterHandler (Message.GET_HAS_ROOM, OnGetRoom);
		NetworkServer.RegisterHandler (Message.GET_DURATION, OnGetDuration);
		NetworkServer.RegisterHandler (Message.SET_READY, OnSetReady);
		NetworkServer.RegisterHandler (Message.SEND_NICKNAME, OnSaveNickName);
		NetworkServer.RegisterHandler (Message.GET_CLIENTS, OnSendClients);
		NetworkServer.RegisterHandler (Message.START_GAME,OnStart);
		NetworkServer.RegisterHandler (Message.QUIT_GAME, OnQuitGame);

		NetworkServer.RegisterHandler (Message.SHUTDOWN, OnShutDown);
//		NetworkServer.RegisterHandler (Message.NUM_OF_PLAYERS, OnRequestPlayerCount);


		return serverIp;
	}

	void updateDictionary(string name,int id){

		Dictionary<string,int> temp = new Dictionary<string,int>();
		foreach(KeyValuePair<string,int> item in status){
			if(item.Key != name){
				temp.Add(item.Key,item.Value);
			}
		}

		status = temp;

		Dictionary<int,string> temp1 = new Dictionary<int,string>();
		foreach(KeyValuePair<int,string> item in nickname){
			if(item.Key != id){
				temp1.Add(item.Key,item.Value);
			}
		}

		nickname = temp1;
	}

	void OnQuitGame(NetworkMessage msg){
		
		Debug.Log(string.Format("get quit message"));
		string name = "";
		if (nickname.TryGetValue (msg.conn.connectionId, out name)) {
			message.Add (name + " has quit the game");

			msg.conn.Disconnect();
			updateDictionary(name,msg.conn.connectionId);
			
			string[] lastMessage = new string[1];
			lastMessage [0] = message [message.Count - 1];

			messageListMessage msg1 = new messageListMessage ();
			msg1.message = lastMessage;
			NetworkServer.SendToAll (Message.MESSAGE_LIST, msg1);

			nicknameMessage msg2 = new nicknameMessage();
			msg2.nickname = name;
			NetworkServer.SendToAll (Message.REMOVE_NICKNAME,msg2);
		}
	}

	void OnSetReady(NetworkMessage msg){

		string name = "";
		if (nickname.TryGetValue (msg.conn.connectionId, out name)) {
			message.Add (name + " is ready");
			status[name] = 1;

			string[] lastMessage = new string[1];
			lastMessage [0] = message [message.Count - 1];

			messageListMessage msg1 = new messageListMessage ();
			msg1.message = lastMessage;

			NetworkServer.SendToAll (Message.MESSAGE_LIST, msg1);
		}
	}

	public bool isActive(){
		return NetworkServer.active;
	}

	void OnStart(NetworkMessage msg){

		messageListMessage msg1 = new messageListMessage();
		string[] message1 = new string[1];
		message1[0] = "Starting Game..";

		msg1.message = message1; 
		NetworkServer.SendToAll (Message.MESSAGE_LIST, msg1);

		bool playerIsNotReady = false;

		foreach(KeyValuePair<string,int>pl in status){
			if(pl.Value == 0){
				message1[0] = "Aborted. " + pl.Key + " is not ready";
				msg1.message = message1; 
				NetworkServer.SendToAll (Message.MESSAGE_LIST, msg1);
				playerIsNotReady = true;
			}
		}

		if(!playerIsNotReady){
			EmptyMessage msg2 = new EmptyMessage();
			NetworkServer.SendToAll (Message.START_GAME, msg2);
		}

	}
//
	public void shutdownServer (){
		singleton = null;
		NetworkServer.Shutdown ();
	}

	void OnShutDown(NetworkMessage msg){
		shutdownServer();
	}
//
	void OnSendClients(NetworkMessage msg){

		Dictionary<int, string>.ValueCollection values = nickname.Values;
		string[] qwe = new string[values.Count];							

		int count = 0;
		foreach (string name in values)
		{
			qwe [count] = name;
			count++;
		}

		ClientListMessage msg1 = new ClientListMessage ();
		msg1.names = qwe;

		NetworkServer.SendToClient(msg.conn.connectionId,Message.GET_CLIENTS, msg1);
	}

	void OnSaveNickName(NetworkMessage msg){
		Debug.Log ("save nickname");
		string nickname1 = msg.ReadMessage<nicknameMessage> ().nickname;
		//NetworkConnection connection = NetworkServer.connections [NetworkServer.connections.Count - 1];
		nickname.Add (msg.conn.connectionId, nickname1); 
		status.Add(nickname1,0);

		if (msg.conn.connectionId == 0) {
			message.Add (nickname1 + " will host the game");
		} else {
			message.Add (nickname1 + " has joined the game");
		}

		nicknameMessage msg1 = new nicknameMessage ();
		msg1.nickname = nickname1;
		NetworkServer.SendToAll (Message.SEND_NICKNAME, msg1);

		messageListMessage msg2 = new messageListMessage ();
		msg2.message = message.ToArray ();
		NetworkServer.SendToAll (Message.MESSAGE_LIST, msg2);
	}


	void OnGetDuration(NetworkMessage msg){

		gameDurationMessage msg1 = new gameDurationMessage ();
		msg1.dropdownVal = gameDurationValue;
		NetworkServer.SendToClient (msg.conn.connectionId, Message.CHANGE_DURATION, msg1);

	}

//	/*
//		Function: OnChangeMap
//
//		Receives map change info from host and send to all clients
//
//		Parameters:
//
//			msg1 - the message received from the caller
//			
//		Returns:
//
//			void
//	*/

	void OnChangeDuration(NetworkMessage msg1)
	{
		int val = msg1.ReadMessage<gameDurationMessage> ().dropdownVal;
		gameDurationValue = val;
		gameDurationMessage msg = new gameDurationMessage ();
		msg.dropdownVal = gameDurationValue;
		NetworkServer.SendToAll(Message.CHANGE_DURATION,msg);
	}
//
//	/*
//        Function: OnRequestPlayerCount
//
//		Send number of players connected to the host
//
//        Parameters: 
//
//           msg1
//
//        Returns:
//
//           void
//     */
//
//	void OnRequestPlayerCount(NetworkMessage msg1)
//	{
//		int playerCount1 = NetworkServer.connections.Count;
//		connectionCount msg = new connectionCount ();
////		msg.newMessage (playerCount1);
//		NetworkConnection localConn = NetworkServer.localConnections [0];
//		NetworkServer.SendToClient (localConn.connectionId, Message.NUM_OF_PLAYERS, msg);
//	}
//
//	/*
//        Function: OnSetHasRoom
//
//		Receive availability status of room from host and saved it for client to check
//
//        Parameters: 
//
//           msg
//
//        Returns:
//
//           void
//     */
//
	void OnSetHasRoom(NetworkMessage msg)
	{
		bool hasRoom1 = msg.ReadMessage<hasRoomMessage> ().hasRoom;
		hasRoomAvailable = hasRoom1;
	}
//
	void OnGetRoom(NetworkMessage msg)
	{
		hasRoomMessage msg1 = new hasRoomMessage ();
		msg1.hasRoom = hasRoomAvailable;
		NetworkConnection connection = NetworkServer.connections [NetworkServer.connections.Count - 1];
		NetworkServer.SendToClient (connection.connectionId, Message.GET_HAS_ROOM, msg1);

	}








}
