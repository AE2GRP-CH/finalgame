using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class networktransform : NetworkBehaviour {

	[SyncVar]private Vector3 syncPos;
	[SyncVar]private Quaternion syncPlayerRotation;
	[SyncVar]private Quaternion syncCamRotation;
	[SerializeField] Transform playerTransform;
	[SerializeField] Transform camTransform;
	[SerializeField] Transform mytransform;
	[SerializeField] float lerpRate=15;

	void FixedUpdate(){
		TransmitPosition ();
		LerpPosition ();
		TransmitRotations ();
		LerpRotations ();

	}
	// Update is called once per frame
	void LerpPosition(){
	if (!isLocalPlayer) {
			mytransform.position=Vector3.Lerp(mytransform.position,syncPos,Time.deltaTime*lerpRate);
		} 
	}

	[Command]
	void CmdProvidePostionToServer(Vector3 pos){
		syncPos = pos;
	}

	[ClientCallback]
	void TransmitPosition(){
		if (isLocalPlayer) {
			CmdProvidePostionToServer (mytransform.position);
		}
	}

	
	void LerpRotations(){
	if (!isLocalPlayer) {
			playerTransform.rotation=Quaternion.Lerp(playerTransform.rotation,syncPlayerRotation,Time.deltaTime*lerpRate);
			camTransform.rotation=Quaternion.Lerp(camTransform.rotation,syncCamRotation,Time.deltaTime*lerpRate);
		}
	}
	[Command]
	void CmdProvideRotationsToServer(Quaternion playerRot, Quaternion camRot){
		syncPlayerRotation = playerRot;
		syncCamRotation = camRot;
	}

	[Client]
	void TransmitRotations(){
	if (isLocalPlayer) {
			CmdProvideRotationsToServer(playerTransform.rotation,camTransform.rotation);
		}
	}
}
