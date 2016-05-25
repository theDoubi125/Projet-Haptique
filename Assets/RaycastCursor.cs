using UnityEngine;
using System.Collections;

public class RaycastCursor : MonoBehaviour {
    public float distToCam = 5, scrollFactor = 1;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        Transform cam = Camera.main.transform;
        
        transform.position = cam.position + cam.forward * distToCam;

        print(Input.mouseScrollDelta);
        if(Input.mouseScrollDelta.y != 0)
        {
            distToCam += Input.mouseScrollDelta.y * scrollFactor;
        }
        
    }
}
