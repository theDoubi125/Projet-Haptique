﻿using UnityEngine;
using System.Collections;

public class RaycastCursor : MonoBehaviour {
    public float distToCam = 5, scrollFactor = 1;
    public WaterFlow instance;
	public int brushSize = 10;
	public int brushIndex = 0;
	public Brush[] brushes;

	private Vector3 currentPosition;
	// Use this for initialization
	void Start () {
		currentPosition = new Vector3 (0, 0, 0);
		brushes = new []{new CubicBrush(),new SphericBrush(),new CrossBrush()};
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

		currentPosition.Set (x, y, z);

        if (Input.GetMouseButton(0))
        {
            print(x + " " + y + " " + z);

			instance.UpdateMesh();
        }
    }
}


abstract class Brush
{
	public abstract void SetVoxel(WaterFlow instance, int brushSize, int x, int y, int z, float value);
}
	
class CubicBrush : Brush
{
	public override void SetVoxel(WaterFlow instance, int brushSize, int x, int y, int z, float value)
	{
		int offset = (int)(brushSize * 0.5);
		
		for (int posX = x - offset; posX <= x + offset; posX++) { // Profondeur
			for (int posZ = z - offset; posZ <= z + offset; posZ++) {
				for (int posY = y - offset; posY <= y + offset; posY++) {
					instance.SetVoxel (posX, posY, posZ, 1);
				}
			}
		}
	}
}

class SphericBrush : Brush
{
	public override void SetVoxel(WaterFlow instance, int brushSize, int x, int y, int z, float value)
	{
		int offset = (int)(brushSize * 0.5);
		
		// Brush boule
		for (int posX = x - offset; posX <= x + offset; posX++) { // Profondeur
			for (int posZ = z - offset; posZ <= z + offset; posZ++) {
				for (int posY = y - offset; posY <= y + offset; posY++) {
					if (Vector3.Distance (currentPosition, new Vector3 (posX, posY, posZ)) < offset)
						instance.SetVoxel (posX, posY, posZ, 1);
				}
			}
		}
	}
}

class CrossBrush : Brush
{
	public override void SetVoxel(WaterFlow instance, int brushSize, int x, int y, int z, float value)
	{
		int offset = (int)(brushSize * 0.5);

		// Brush croix
		for (int posX = x - offset; posX <= x + offset; posX++) { // Profondeur
			instance.SetVoxel (posX, y, z, 1);
		}
		for (int posZ = z - offset; posZ <= z + offset; posZ++) {
			instance.SetVoxel (x, y, posZ, 1);
		}
		for (int posY = y - offset; posY <= y + offset; posY++) {
			instance.SetVoxel (x, posY, z, 1);
		}
	}
}