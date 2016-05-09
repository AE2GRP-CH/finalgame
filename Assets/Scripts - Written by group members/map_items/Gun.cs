using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Gun : MapItem {

	[SyncVar(hook="setParent")]
	public NetworkInstanceId parentNetId;

	[SyncVar(hook="updateRot")]
	private Vector3 rot;

	[SyncVar]
	private bool hasOwnership = false;

	[SyncVar(hook = "KillObject")]
	private bool present;

	private int numOfBullets = 3;


	void setParent(NetworkInstanceId id){
		if(!isServer){
			GameObject parent = ClientScene.FindLocalObject(id);
			transform.SetParent(parent.transform.GetChild(0));
			transform.localRotation = Quaternion.Euler(0,90.0f,0);
			transform.localPosition = new Vector3(0.6f,-0.439f,0.511f);
		}
	}

	void updateRot(Vector3 rot){
		gameObject.transform.localRotation = Quaternion.Euler (rot);
	}

	void Update(){
		if(Input.GetMouseButtonDown(0) && hasOwnership){
			GameObject printer = GameObject.Find("printer");
			printer.SendMessage("PrintBullet",numOfBullets.ToString());
			GetComponent<AudioSource>().clip = (AudioClip)Resources.Load("sounds/laser",typeof(AudioClip));
			GetComponent<AudioSource>().volume = 0.55f;
			GetComponent<AudioSource>().Play();
    		if(--numOfBullets != -1){
    			GameObject gunTip = Utility.getChildWithName(gameObject.transform,"gunTip");
    			CmdSpawnBullet(new Vector3(gameObject.transform.parent.eulerAngles.x-270.0f,gameObject.transform.root.eulerAngles.y,0));
    		}else{
    			Destroy(gameObject);
    		}
    	}
	}

	[Command]
	void CmdSpawnBullet(Vector3 rot){
		GameObject gunTip = Utility.getChildWithName(gameObject.transform,"gunTip");
		GameObject bullet = Instantiate((GameObject)Resources.Load("bullet2",typeof(GameObject)));
		bullet.SendMessage("setParentId",gameObject.GetComponent<NetworkIdentity>().netId);
		bullet.SendMessage("setRot",rot);
		
		NetworkServer.Spawn(bullet);
		StartCoroutine(moveBullet(bullet));
	}

	IEnumerator moveBullet(GameObject bullet){
		yield return new WaitForSeconds(0.25f);
		//bullet.SetActive(true);
		bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.up * 400.0f);
	}

	void setParentId(NetworkInstanceId id){
		parentNetId = id;
		hasOwnership = true;
	}
	
	protected override void Start(){
		if(!isServer && gameObject.transform.parent == null){
			gameObject.transform.eulerAngles = new Vector3(-90f,0,0);
		}
	}

	private void KillObject(bool status){
		if(!status){
			Destroy(gameObject);
		}
	}

	protected override void OnTriggerEnter(Collider c){
		if(c.gameObject.tag == "pacman"){
			GetComponent<AudioSource>().Play();
			Invoke("kill",GetComponent<AudioSource>().clip.length+0.4f);
		}
	}

	void kill(){
		present = false;
	}


}
