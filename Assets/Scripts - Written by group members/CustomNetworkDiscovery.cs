using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class CustomNetworkDiscovery : NetworkDiscovery {

	List<string> uniqueIPs = new List<string>(); 

	public override void OnReceivedBroadcast(string fromAddress,string data){

		if(!uniqueIPs.Contains(fromAddress)){
			DiscoverDispatcher.getInstance().setSource(fromAddress);
			uniqueIPs.Add(fromAddress);
		}
	}

	public void remove(){
		Debug.LogError("clear");
		uniqueIPs.Clear();
	}

}
