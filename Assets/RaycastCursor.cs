using UnityEngine;
using System.Collections;

public class RaycastCursor : MonoBehaviour {
    public float distToCam = 5, scrollFactor = 1;
    public WaterFlow instance;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        Transform cam = Camera.main.transform;
        
        transform.position = cam.position + cam.forward * distToCam;


        if(Input.mouseScrollDelta.y != 0)
        {
            distToCam += Input.mouseScrollDelta.y * scrollFactor;
        }

        int x = (int)(transform.position.x - instance.transform.position.x) + 16;
        int y = (int)(transform.position.y - instance.transform.position.y) + 16;
        int z = (int)(transform.position.z - instance.transform.position.z) + 16;
        if (Input.GetMouseButton(0))
        {
            print(x + " " + y + " " + z);
            instance.SetVoxel(x, y, z, 1);
            instance.UpdateMesh();
        }
    }
}
