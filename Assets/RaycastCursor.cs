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
    private Vector3 force, displayForce;
    private HapticArmController armController;

	public bool isMouseLocked = true;


	// Use this for initialization
	void Start () {
		brushes = new Brush[]{new CubicBrush(),new SphericBrush(),new CrossBrush()};
        armController = GameObject.FindGameObjectWithTag("Haptic").GetComponent<HapticArmController>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        Transform cam = Camera.main.transform;

        Vector3 armPos = armController.GetArmPos();
        transform.position = cam.position + cam.forward * (distToCam - armPos.x) + cam.right * armPos.y + cam.up * armPos.z;


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
				(brushes [brushIndex]).SetVoxel (instance, brushSize, x, y, z, currentVoxelValue);
				instance.UpdateMesh();
            }
        }
        force = Vector3.zero;
        displayForce = Vector3.zero;
        for (int i=0; i< brushSize; i++)
        {
            for(int j=0; j< brushSize; j++)
            {
                for(int k=0; k< brushSize; k++)
                {
                    foreach (WaterFlow instance in instances)
                    {
                        Vector3 pos = new Vector3(i - brushSize/2, j - brushSize/2, k - brushSize/2);
                        if(pos.magnitude < brushSize)
                        {
                            int x = (int)(transform.position.x - instance.transform.position.x + i - 0.5f) + 16;
                            int y = (int)(transform.position.y - instance.transform.position.y + j - 0.5f) + 16;
                            int z = (int)(transform.position.z - instance.transform.position.z + k - 0.5f) + 16;
                            Vector3 centerPos = instance.transform.position + new Vector3(x - 16.5f, y - 16.5f, z - 16.5f);
                            float distance = (centerPos - transform.position).magnitude;
                            Vector3 f = -instance.GetVoxel(x, y, z) * (centerPos - transform.position).normalized * (1 - distance / brushSize) * 5;
                            displayForce += f;
                            float fx = Vector3.Dot(f, cam.forward);
                            float fy = Vector3.Dot(f, cam.right);
                            float fz = Vector3.Dot(f, cam.up);
                            force += new Vector3(fx, fy, fz);
                        }
                    }
                }
            }
        }
        GameObject.FindGameObjectWithTag("Haptic").GetComponent<HapticArmController>().setForce(force/2);
        if (force.magnitude != 0)
            print(force.magnitude);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + displayForce);
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
					instance.SetVoxel (posX, posY, posZ, value);
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

		Vector3 currentPosition = new Vector3 (x, y, z);
		
		// Brush boule
		for (int posX = x - offset; posX <= x + offset; posX++) { // Profondeur
			for (int posZ = z - offset; posZ <= z + offset; posZ++) {
				for (int posY = y - offset; posY <= y + offset; posY++) {
					if (Vector3.Distance (currentPosition, new Vector3 (posX, posY, posZ)) < offset)
						instance.SetVoxel (posX, posY, posZ, value);
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
			instance.SetVoxel (posX, y, z, value);
		}
		for (int posZ = z - offset; posZ <= z + offset; posZ++) {
			instance.SetVoxel (x, y, posZ, value);
		}
		for (int posY = y - offset; posY <= y + offset; posY++) {
			instance.SetVoxel (x, posY, z, value);
		}
	}
}
