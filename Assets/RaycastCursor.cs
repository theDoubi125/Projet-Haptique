using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaycastCursor : MonoBehaviour {
    public float distToCam = 5, scrollFactor = 1;
    public List<WaterFlow> instances = new List<WaterFlow>();
	public int brushSize = 10;
	public int brushIndex = 0;
	public int currentVoxelValue = 1;
	private Brush[] brushes;

	public bool isMouseLocked = true;

	public Color currentColor;


	// Use this for initialization
	void Start () {
		brushes = new Brush[]{new CubicBrush(),new SphericBrush(),new CrossBrush()};
		currentColor = new Color (Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), 1);
	}


	public void OnSetColor(Color color)
	{
		currentColor = color;
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
            int x = (int)(transform.position.x - instance.transform.position.x - 0.5f) + 16;
            int y = (int)(transform.position.y - instance.transform.position.y - 0.5f) + 16;
            int z = (int)(transform.position.z - instance.transform.position.z - 0.5f) + 16;

            if (x >= 0 && y >= 0 && z >= 0 && x < 32 && y < 32 && z < 32 && Input.GetMouseButton(0))
            {
				(brushes [brushIndex]).SetVoxel (instance, brushSize, x, y, z, currentVoxelValue, currentColor);
				instance.UpdateMesh();
            }
        }
    }
}


abstract class Brush
{
	public abstract void SetVoxel(WaterFlow instance, int brushSize, int x, int y, int z, float value, Color currentColor);
}
	
class CubicBrush : Brush
{
	public override void SetVoxel(WaterFlow instance, int brushSize, int x, int y, int z, float value, Color currentColor)
	{
		int offset = (int)(brushSize * 0.5);

        for (int posX = x - offset; posX <= x + offset; posX++)
        { // Profondeur
            for (int posZ = z - offset; posZ <= z + offset; posZ++)
            {
                for (int posY = y - offset; posY <= y + offset; posY++)
                {
                    instance.SetVoxel(posX, posY, posZ, value, currentColor);
                }
            }
        }
        for (int posX = x - offset - 1; posX <= x + offset; posX++)
        { // Profondeur
            for (int posZ = z - offset - 1; posZ <= z + offset; posZ++)
            {
                for (int posY = y - offset - 1; posY <= y + offset; posY++)
                {
                    instance.SetColor(posX, posY, posZ, currentColor);
                }
            }
        }
    }
}

class SphericBrush : Brush
{
	public override void SetVoxel(WaterFlow instance, int brushSize, int x, int y, int z, float value, Color currentColor)
	{
		int offset = (int)(brushSize * 0.5);

		Vector3 currentPosition = new Vector3 (x, y, z);
		
		// Brush boule
		for (int posX = x - offset; posX <= x + offset; posX++) { // Profondeur
			for (int posZ = z - offset; posZ <= z + offset; posZ++) {
				for (int posY = y - offset; posY <= y + offset; posY++) {
					if (Vector3.Distance (currentPosition, new Vector3 (posX, posY, posZ)) < offset)
						instance.SetVoxel (posX, posY, posZ, value, currentColor);
				}
			}
		}
	}
}

class CrossBrush : Brush
{
	public override void SetVoxel(WaterFlow instance, int brushSize, int x, int y, int z, float value, Color currentColor)
	{
		int offset = (int)(brushSize * 0.5);

		// Brush croix
		for (int posX = x - offset; posX <= x + offset; posX++) { // Profondeur
			instance.SetVoxel (posX, y, z, value, currentColor);
		}
		for (int posZ = z - offset; posZ <= z + offset; posZ++) {
			instance.SetVoxel (x, y, posZ, value, currentColor);
		}
		for (int posY = y - offset; posY <= y + offset; posY++) {
			instance.SetVoxel (x, posY, z, value, currentColor);
		}
	}
}
