﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_SyncPosition : NetworkBehaviour {

	[SyncVar] 
	private Vector3 syncPos;
	
	[SerializeField] Transform myTransform;

	[SerializeField] float lerpRate=15;

	// Update is called once per frame
	void FixedUpdate () {

			TransmitPosition();
			LerpPosition();
	}

	void LerpPosition()
	{
		if(!isLocalPlayer)
		{
			// myTransform.position=Vector3.Lerp(myTransform.position,syncPos,Time.deltaTime*lerpRate);
		}
	}

	[Command]
	void CmdSendServerPosition(Vector3 _position)
	{
		syncPos=_position;
	}

	[ClientCallback]
	void TransmitPosition()
	{
		if(isLocalPlayer)
		{
			CmdSendServerPosition(myTransform.position);
		}
	}
}
