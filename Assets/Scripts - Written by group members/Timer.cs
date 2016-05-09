using UnityEngine;
using UnityEngine.Networking;
using System;

public struct struct_Time
{
   public int sec;
   public int minute;

   public void initTime(int min, int seconds){
   		sec = seconds;
   		minute = min;
   }
};

public class Timer:NetworkBehaviour{

	DateTime prevTime = new DateTime();
	DateTime startTime = DateTime.MinValue;
	private static int second = 0;
	private static int minute;
	private static bool end = false;

	void Awake(){
		minute = PlayerPrefs.GetInt("Timer");
	}

	[ClientRpc]
	void RpcUpdateTime(struct_Time tm){
		GameObject printer = GameObject.Find("printer");
		printer.SendMessage("PrintTime",tm);		
	}

	void stopTimer(){
		end = true;
	}

	void Update(){	
	  if(isServer && !end){	
	  	DateTime now = DateTime.Now;
	  	bool future = now.Subtract(prevTime).Seconds > 0;
		//When future == false; no time change
	  	if(future){
	  		// if(now.Subtract(prevTime).Seconds < 2){
	  			if(second == 0){ second = 60;minute--; }
	  			second--;
	  			struct_Time tm = new struct_Time();
				tm.initTime(minute,second);
				RpcUpdateTime(tm);
				if(second == 0 && minute == 0){ 
	  				stopTimer(); 
	  				GameObject.Find("manager").SendMessage("TimeFinished");
	  			}
	  		// }
	  		prevTime = now;
	  	 }
	   }
	 }
		

}