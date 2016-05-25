using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaycastCursor : MonoBehaviour {
    public float distToCam = 5, scrollFactor = 1;
    public List<WaterFlow> instances = new List<WaterFlow>();
	public int brushSize = 10;
	public int brushType = 1;
	private Vector3 currentPosition;
	// Use this for initialization
	void Start () {
		currentPosition = new Vector3 (0, 0, 0);
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

		    currentPosition.Set (x, y, z);

            if (Input.GetMouseButton(0))
            {
                print(x + " " + y + " " + z);

                int offset = (int)(brushSize * 0.5);

                /*for (int teta = 0; teta < 360; teta++) {
				    for (int teta2 = 0; teta2 < 180; teta2++) {
					    instance.SetVoxel((int)(x+offset*Mathf.Cos(teta)*Mathf.Sin(teta2)), (int)(y+offset*Mathf.Sin(teta)*Mathf.Cos(teta2)), (int)(z+offset*Mathf.Cos(teta2)), 1);
				    }
			    }*/

                if (brushType == 1)
                {
                    // Brush boule
                    for (int posX = x - offset; posX <= x + offset; posX++)
                    { // Profondeur
                        for (int posZ = z - offset; posZ <= z + offset; posZ++)
                        {
                            for (int posY = y - offset; posY <= y + offset; posY++)
                            {
                                if (Vector3.Distance(currentPosition, new Vector3(posX, posY, posZ)) < offset)
                                    instance.SetVoxel(posX, posY, posZ, 1);
                            }
                        }
                    }
                }
                else if (brushType == 2)
                {
                    // Brush boule
                    for (int posX = x - offset; posX <= x + offset; posX++)
                    { // Profondeur
                        for (int posZ = z - offset; posZ <= z + offset; posZ++)
                        {
                            for (int posY = y - offset; posY <= y + offset; posY++)
                            {
                                if (Vector3.Distance(currentPosition, new Vector3(posX, posY, posZ)) < offset)
                                    instance.SetVoxel(posX, posY, posZ, 1);
                            }
                        }
                    }
                }
                else
                {
                    // brush cube
                    for (int posX = x - offset; posX <= x + offset; posX++)
                    { // Profondeur
                        for (int posZ = z - offset; posZ <= z + offset; posZ++)
                        {
                            for (int posY = y - offset; posY <= y + offset; posY++)
                            {
                                instance.SetVoxel(posX, posY, posZ, 1);
                            }
                        }
                    }
                }
                instance.UpdateMesh();
            }
        }
    }
}
