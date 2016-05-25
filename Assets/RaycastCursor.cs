using UnityEngine;
using System.Collections;

public class RaycastCursor : MonoBehaviour {
    public float distToCam = 5, scrollFactor = 1;
    public WaterFlow[] instances;
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

        foreach(WaterFlow instance in instances)
        {
            int x = (int)(transform.position.x - instance.transform.position.x) + 16;
            int y = (int)(transform.position.y - instance.transform.position.y) + 16;
            int z = (int)(transform.position.z - instance.transform.position.z) + 16;
            if (x >= 0 && y >= 0 && z >= 0 && x < 32 && y < 32 && z < 32 && Input.GetMouseButton(0))
            {
                instance.SetVoxel(x, y, z, 1);
                instance.UpdateMesh();
            }
        }
    }
}
