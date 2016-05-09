using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiscoverDispatcher{

	public string srcIp;
	public List<Observer> observers = new List<Observer>();
	private static DiscoverDispatcher instance;

	public static DiscoverDispatcher getInstance(){
		if(instance == null){
			instance = new DiscoverDispatcher();
		}
		return instance;
	}

	public string getState(){
		return srcIp;
	}

	public void setSource(string ip){
		srcIp = ip;
		update();
	}

	public void attach(Observer observer){
		observers.Add(observer);
	}

	public void update(){
		foreach(Observer obs in observers){
			obs.update();
		}
	}

}