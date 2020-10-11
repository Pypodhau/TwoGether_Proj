using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace WasaaMP {
public class Interactive : MonoBehaviourPun {
    // Start is called before the first frame update

    MonoBehaviourPun support = null ;

    void Start () {
        
    }

    // Update is called once per frame
    void Update () {
        
    }
    void OnTriggerEnter (Collider other) {
        print (name + " : OnCollisionEnter") ;
		var hit = other.gameObject ;
		var cursor = hit.GetComponent<CursorTool> () ;
		if (cursor != null) {
			Renderer renderer = GetComponentInChildren <Renderer> () ;
		    renderer.material.color = Color.blue ;
		}
	}
    
    void OnTriggerExit (Collider other) {
        print (name + " : OnCollisionExit") ;
		var hit = other.gameObject ;
		var cursor = hit.GetComponent<CursorTool> () ;
		if (cursor != null) {
			Renderer renderer = GetComponentInChildren <Renderer> () ;
		    renderer.material.color = Color.white ;
		}
	}

    public void SetSupport (MonoBehaviourPun support) {
        this.support = support ;
        if (support != null) {
            transform.SetParent (support.transform) ;
        } else {
            transform.SetParent (null) ;
        }
    }

    public void RemoveSupport () {
        transform.SetParent (null) ;
        support = null ;
    }

    public MonoBehaviourPun GetSupport () {
        return support ;
    }
}

}
