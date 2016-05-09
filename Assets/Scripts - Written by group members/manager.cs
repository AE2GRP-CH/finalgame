using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

public class manager : NetworkBehaviour {

	private int numOfFruits = 0;
	private int numOfEnemy = 0;
	public GameObject printer;

	void TimeFinished(){
		int number = GameObject.FindGameObjectsWithTag("bean").Length;
		RpcTimeFinished(number == 0);
	}

	void deactivatePlayer(){
		List<PlayerController> x = ClientScene.localPlayers;
		bool m = x[0].gameObject.GetComponent<NetworkIdentity>().isLocalPlayer;
		Debug.LogError(x.Count);
		(x[0].gameObject).SetActive(false);
		Debug.LogError(m.ToString());
	}

	[ClientRpc]
	void RpcTimeFinished(bool pacmanWins){
		GameObject printer = GameObject.Find("printer");
		if(pacmanWins){
			printer.SendMessage("PrintStateToGui",isServer?"Pacman Wins":"Ghost Lose");				
		}else{
			printer.SendMessage("PrintStateToGui",isServer?"Pacman Lose":"Ghost Wins");
		}
	}

	void PacmanCollided(GameObject pacman){
		RpcPacmanCaptured(pacman);
	}


	[ClientRpc]
	void RpcPacmanCaptured(GameObject pac){
		GameObject printer = GameObject.Find("printer");
		if(pac.GetComponent<NetworkIdentity>().isLocalPlayer){
			printer.SendMessage("PrintStateToGui","You were eaten");
			GameObject.Find("Timer").SendMessage("stopTimer");
		}else{
			//Debug.LogError("anjing");
			printer.SendMessage("PrintStateToGui","Ghost Wins!!");
		}

		pac.SetActive(false);
		deactivatePlayer();
		
	}

	void GhostCollided(GameObject collidedGhost){
		GameObject[] numOfEnemy = GameObject.FindGameObjectsWithTag("enemy");
		if(numOfEnemy.Length-1 == 0){
			RpcGameOver();
		}else{
			RpcGhostCaptured(collidedGhost);
		}
	}

	[ClientRpc]
	void RpcGhostCaptured(GameObject ghost){
		if(ghost.GetComponent<NetworkIdentity>().isLocalPlayer){
			GameObject printer = GameObject.Find("printer");
			printer.SendMessage("PrintStateToGui","You were Eatenn");
		}
		ghost.SetActive(false);
	}

	[ClientRpc]

	void RpcGameOver(){
		GameObject printer = GameObject.Find("printer");
		if(isServer){
			printer.SendMessage("PrintStateToGui","Pacman Wins!!");
			GameObject.Find("Timer").SendMessage("stopTimer");
		}else{
			printer.SendMessage("PrintStateToGui","Ghost Lose");
		}
		deactivatePlayer();
	}

	void BombExploded(Vector3 pos){

		Collider[] nearby = Physics.OverlapSphere(pos,13.0f);

		Collider[] enemy = nearby.Where(p => p.gameObject.tag == "enemy").ToArray();
		Collider[] pacman = nearby.Where(p => p.gameObject.tag == "pacman").ToArray();

		List<GameObject> uniqueEnemy = Utility.removeDuplicate(enemy);
		List<GameObject> uniquePacman = Utility.removeDuplicate(pacman);

		uniqueEnemy.AddRange(uniquePacman);

		GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
		if(uniqueEnemy.Count > 0){
			if(enemies.Length - uniqueEnemy.Count != 0 && uniquePacman.Count == 0){
			// Killed one enemy
			RpcdeactivatePlayer(uniqueEnemy.ToArray());
			}else{
				if(uniquePacman.Count == 1 && enemy.Length == 0 && enemies.Length == 0){
					//Killed the only Pacman
					RpcdeactivatePlayer(uniqueEnemy.ToArray());
				}else{
					RpcPrintOutput(generateOutput(uniqueEnemy), uniqueEnemy.ToArray());
				}
			}
		}
	}

	[ClientRpc]
	void RpcPrintOutput(string[] pl,GameObject[] players){
		GameObject printer = GameObject.Find("printer");
		Debug.LogError("Length of message is " + pl.Length.ToString());
		printer.SendMessage("PrintStateToGui",isServer?pl[0]:pl[1]);
		GameObject[] localPlayer = players.Where(c => c.GetComponent<NetworkIdentity>().isLocalPlayer == true).ToArray();

		if(localPlayer.Length > 0 && (localPlayer[0].tag == "enemy" || localPlayer[0].tag == "pacman")){
			// localPlayer[0].SetActive(false);
		}

		foreach(GameObject obj in localPlayer){
			obj.SetActive(false);
		}
	    deactivatePlayer();
		if(isServer){ GameObject.Find("Timer").SendMessage("stopTimer");}
	}

	[ClientRpc]
	void RpcdeactivatePlayer(GameObject[] players){
		GameObject printer = GameObject.Find("printer");
		GameObject[] localPlayer = players.Where(c => c.GetComponent<NetworkIdentity>().isLocalPlayer == true).ToArray();
		if(localPlayer.Length > 0 && (localPlayer[0].tag == "enemy" || localPlayer[0].tag == "pacman")){
			// if(players.Length == 1){
				printer.SendMessage("PrintStateToGui","You were killed!");
				if(isServer){ GameObject.Find("Timer").SendMessage("stopTimer");}	
			// }
		}

		foreach(GameObject obj in localPlayer){
			obj.SetActive(false);
		}

		Debug.LogError(localPlayer.Length.ToString());
		// localPlayer[0].SetActive(false);
	}

	string[] generateOutput(List<GameObject> players){
		GameObject[] numOfEnemy = GameObject.FindGameObjectsWithTag("enemy");
		GameObject[] enemy = players.Where(c => c.tag == "enemy").ToArray();
		GameObject[] pacman = players.Where(c => c.tag == "pacman").ToArray();

		string[] outputs = new string[2];
		if(numOfEnemy.Length > 0){
			if(pacman.Length == 1){
				if(enemy.Length  == numOfEnemy.Length){
					outputs[0] = "It's A Tiee!!";
					outputs[1] = "It's A Tiee!!";
				}else{
					outputs[0] = "Pacman Lose";
					outputs[1] = "Ghost Wins!!";
				}
			}else{
				if(enemy.Length == numOfEnemy.Length){
					outputs[0] = "Pacman Wins!!";
					outputs[1] = "Ghost Lose";
				}
			}
		}
		return outputs;
	}

	
}
