using UnityEngine;
using System.Collections;

public class RaycastCursor : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        Transform cam = Camera.main.transform;
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, 500))
        {
            transform.position = hit.point;
        }
    }
}
