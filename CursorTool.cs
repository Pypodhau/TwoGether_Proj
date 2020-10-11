using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace WasaaMP {
public class CursorTool : MonoBehaviourPun {
	float x, previousX = 0 ;
	float y, previousY = 0 ;
	float z, lastZ ;
	public bool active ;

	private bool caught ;

	public Interactive interactiveObjectToInstanciate ;
	private GameObject target ;
	MonoBehaviourPun targetParent ;

	MonoBehaviourPun player ; 

	void Start () {
		active = false ;
		caught = false ;
		player = (MonoBehaviourPun)this.GetComponentInParent(typeof (Navigation)) ;
		name = player.name + "_" + name ;
	}
	
	void Update () {
			// control of the 3D cursor
			if (player.photonView.IsMine  || ! PhotonNetwork.IsConnected) {
				if (Input.GetButtonDown ("Fire1")) {
					Fire1Pressed (Input.mousePosition.x, Input.mousePosition.y) ;
				}
				if (Input.GetButtonUp ("Fire1")) {
					Fire1Released (Input.mousePosition.x, Input.mousePosition.y) ;
				}
				if (active) {
					Fire1Moved (Input.mousePosition.x, Input.mousePosition.y, Input.mouseScrollDelta.y) ;
				}
				if (Input.GetKeyDown (KeyCode.C)) {
					CreateInteractiveCube () ;
				}
				if (Input.GetKeyDown (KeyCode.B)) {
					Catch () ;
				}
				if (Input.GetKeyDown (KeyCode.N)) {
					Release () ;
					target = null ;
				}
			}

	}

	public void Fire1Pressed (float mouseX, float mouseY) {
		active = true ;
		x = mouseX ;
		previousX = x ;
		y = mouseY ;
		previousY = y ;
	}

	public void Fire1Released (float mouseX, float mouseY) {
		active = false ;
	}

	public void Fire1Moved (float mouseX, float mouseY, float mouseZ) {
		x = mouseX ;
		float deltaX = (x - previousX) / 100.0f ;
		previousX = x ;
		y = mouseY ;
		float deltaY = (y - previousY) / 100.0f ;
		previousY = y ;
		float deltaZ = mouseZ / 10.0f ;
		transform.Translate (deltaX, deltaY, deltaZ) ;
	}

	public void Catch () {
		print ("B ?") ;
		if (target != null) {
			print ("B :") ;
			var tb = target.GetComponent<Interactive> () ;
			if (tb != null) {
				if ((! caught) && (this != tb.GetSupport ())) { // pour ne pas prendre 2 fois l'objet et lui faire perdre son parent
					targetParent = tb.GetSupport () ;
				}
				print ("ChangeSupport of object " +  tb.photonView.ViewID + " to " + photonView.ViewID) ;
				photonView.RPC ("ChangeSupport", RpcTarget.All, tb.photonView.ViewID, photonView.ViewID) ;
				tb.photonView.TransferOwnership (PhotonNetwork.LocalPlayer) ;
				PhotonNetwork.SendAllOutgoingCommands () ;
				caught = true ;
				print ("B !") ;
			}
		} else {
			print ("pas B") ;
		}
	}

	public void Release () {
		if (target != null) {
			print ("N :") ;
			var tb = target.GetComponent<Interactive> () ;
			if (tb != null) {
				if (targetParent != null) {
					photonView.RPC ("ChangeSupport", RpcTarget.All, tb.photonView.ViewID, targetParent.photonView.ViewID) ;
					targetParent = null ;
				} else {
					photonView.RPC ("RemoveSupport", RpcTarget.All, tb.photonView.ViewID) ;
				}
				PhotonNetwork.SendAllOutgoingCommands () ;
				print ("N !") ;
				caught = false ;
			}
		} else {
			print ("pas N") ;
		}
	}

	public void CreateInteractiveCube () {
		var objectToInstanciate = PhotonNetwork.Instantiate (interactiveObjectToInstanciate.name, transform.position, transform.rotation, 0) ;
	}

	void OnTriggerEnter (Collider other) {
        print (name + " : OnTriggerEnter") ;
		target = other.gameObject ;
	}

	void OnTriggerExit (Collider other) {
        print (name + " : OnTriggerExit") ;
		target = null ;
	}

    [PunRPC] public void ChangeSupport (int interactiveID, int newSupportID) {
        Interactive go = PhotonView.Find (interactiveID).gameObject.GetComponent<Interactive> () ;
        MonoBehaviourPun s = PhotonView.Find (newSupportID).gameObject.GetComponent<MonoBehaviourPun> () ;
        print ("ChangeSupport of object " +  go.name + " to " + s.name) ;
        go.SetSupport (s) ;
    }

    [PunRPC] public void RemoveSupport (int interactiveID) {
        Interactive go = PhotonView.Find (interactiveID).gameObject.GetComponent<Interactive> () ;
        print ("RemoveSupport of object " +  go.name) ;
        go.RemoveSupport () ;
    }

}
}