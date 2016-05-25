using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {
    public WaterFlow instance;
    public int x, y, z;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = instance.transform.position + new Vector3(-16+x, -16+y, -16+z);
        //x = instance.transform.position.x - 16 + X => 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            instance.SetVoxel(x, y, z, 1);
            instance.UpdateMesh();
        }
    }
}
