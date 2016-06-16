using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaycastCursor : MonoBehaviour {
    private float friction;
    public float frictionFactor = 10, frictionReduc = 0.99f;
    public float distToCam = 5, scrollFactor = 1;
    public List<WaterFlow> instances = new List<WaterFlow>();
	public int brushSize = 10;
	public int brushIndex = 0;
	public int currentVoxelValue = 1;
	private Brush[] brushes;
    private Vector3 force, displayForce;
    private HapticArmController armController;
    public bool isControlled = true;
    public Vector3 decal, decalFactor = new Vector3(1, 1, 1);

	public bool isMouseLocked = true;
    public float armScrollFactor;

	public Color currentColor;
    private float currentArmDist = 0;


	// Use this for initialization
	void Start () {
        brushes = new Brush[] { new CubicBrush(), new SphericBrush(), new CrossBrush() };
		currentColor = new Color (Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), 1);
        armController = GameObject.FindGameObjectWithTag("Haptic").GetComponent<HapticArmController>();
    }


	public void OnSetColor(Color color)
	{
		currentColor = color;
	}
	// Update is called once per frame
	void Update ()
    {
        Transform cam = Camera.main.transform;

        Vector3 armPos = armController.GetArmPos();
        if(isControlled)
            transform.position = cam.position + cam.forward * (distToCam - armPos.x) + cam.right * armPos.y + cam.up * armPos.z;


        if(Input.mouseScrollDelta.y != 0)
        {
            distToCam += Input.mouseScrollDelta.y * scrollFactor;
        }
        if(armController.switches[2])
        {
            distToCam += (armController.armPos.x - currentArmDist) * armScrollFactor;
        }
        currentArmDist = armController.armPos.x;
        
        foreach(WaterFlow instance in instances)
        {
            int x = (int)(transform.position.x - instance.transform.position.x - 0.5f) + 16;
            int y = (int)(transform.position.y - instance.transform.position.y - 0.5f) + 16;
            int z = (int)(transform.position.z - instance.transform.position.z - 0.5f) + 16;

            if (x >= 0 && y >= 0 && z >= 0 && x < 32 && y < 32 && z < 32 && (armController.switches[0]))
            {
				(brushes [brushIndex]).SetVoxel (instance, brushSize, x, y, z, currentVoxelValue, currentColor);
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
                            int x = (int)(transform.position.x - instance.transform.position.x + i - decal.x + decalFactor.x * instance.x) + 16;
                            int y = (int)(transform.position.y - instance.transform.position.y + j - decal.y + decalFactor.y * instance.y) + 16;
                            int z = (int)(transform.position.z - instance.transform.position.z + k - decal.z + decalFactor.z * instance.z) + 16;
                            Vector3 centerPos = instance.transform.position + new Vector3(x - 16.5f, y - 16.5f, z - 16.5f);
                            float distance = (centerPos - transform.position).magnitude;
                            Vector3 f = instance.GetGradientAt(x, y, z) * (1 - distance / brushSize) * 5;
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
        //GameObject.FindGameObjectWithTag("Haptic").GetComponent<HapticArmController>().setForce(force/5);
        friction -= frictionFactor * frictionReduc * Time.deltaTime;
        if (friction < 0)
            friction = 0;
        if (force.magnitude > 0)
            friction = frictionFactor;
        GameObject.FindGameObjectWithTag("Haptic").GetComponent<HapticArmController>().setFriction(friction);
       GetVoxel(transform.position);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + displayForce);
    }

    float GetVoxel(Vector3 pos)
    {
        float result = 0;
        foreach(WaterFlow instance in instances)
        {
            int x = (int)(transform.position.x - instance.transform.position.x + 0.5f) + 16;
            int y = (int)(transform.position.y - instance.transform.position.y - 0.5f) + 16;
            int z = (int)(transform.position.z - instance.transform.position.z - 0.5f) + 16;
            result += instance.GetVoxel(x, y, z);
        }
        return result;
    }

    float GetVoxel(Vector3 pos, int dx, int dy, int dz)
    {
        float result = 0;
        foreach (WaterFlow instance in instances)
        {
            int x = (int)(transform.position.x - instance.transform.position.x + dx - 0.5f) + 16;
            int y = (int)(transform.position.y - instance.transform.position.y + dy - 0.5f) + 16;
            int z = (int)(transform.position.z - instance.transform.position.z + dz - 0.5f) + 16;
            result += instance.GetVoxel(x, y, z);
        }
        return result;
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

        Vector3 currentPosition = new Vector3(x, y, z);

        // Brush boule
        for (int posX = x - offset; posX <= x + offset; posX++)
        { // Profondeur
            for (int posZ = z - offset; posZ <= z + offset; posZ++)
            {
                for (int posY = y - offset; posY <= y + offset; posY++)
                {
                    if (Vector3.Distance(currentPosition, new Vector3(posX, posY, posZ)) < offset)
                    {
                        instance.SetVoxel(posX, posY, posZ, value, currentColor);
                    }


                }
            }
        }
        for (int posX = x - offset - 1; posX <= x + offset + 1; posX++)
        { // Profondeur
            for (int posZ = z - offset - 1; posZ <= z + offset + 1; posZ++)
            {
                for (int posY = y - offset - 1; posY <= y + offset + 1; posY++)
                {
                    if (Vector3.Distance(currentPosition, new Vector3(posX, posY, posZ)) < offset + 2)
                        instance.SetColor(posX, posY, posZ, currentColor);
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
        for (int posX = x - offset; posX <= x + offset; posX++)
        { // Profondeur
            instance.SetVoxel(posX, y, z, value, currentColor);
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    for (int k = -1; k <= 1; k++)
                    {
                        instance.SetColor(posX + i, y + j, z + k, currentColor);
                    }
                }
            }
            for (int posZ = z - offset; posZ <= z + offset; posZ++)
            {
                instance.SetVoxel(x, y, posZ, value, currentColor);
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        for (int k = -1; k <= 1; k++)
                        {
                            instance.SetColor(x + i, y + j, posZ + k, currentColor);
                        }
                    }
                }
            }
            for (int posY = y - offset; posY <= y + offset; posY++)
            {
                instance.SetVoxel(x, posY, z, value, currentColor);
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        for (int k = -1; k <= 1; k++)
                        {
                            instance.SetColor(x + i, posY + j, z + k, currentColor);
                        }
                    }
                }
            }
        }
	}
}
