using UnityEngine;
using System.Collections;

public class ChunkGenerator : MonoBehaviour {
    public int layersX, layersY, layersZ;
    public Transform chunkPrefab;
	public RaycastCursor cursor;
	public Material material;

	void Start ()
    {
	    for(int i=0; i < layersX; i++)
        {
            for(int j=0; j < layersY; j++)
            {
                for(int k=0; k < layersZ; k++)
                {
                    Transform instance = Instantiate<Transform>(chunkPrefab);
                    instance.transform.position = transform.position + new Vector3(30 * i, 30 * j, 30 * k);
                    WaterFlow waterflow = instance.GetComponent<WaterFlow>();
                    waterflow.m_material = material;
                    waterflow.x = i;
                    waterflow.y = j;
                    waterflow.z = k;

                    cursor.instances.Add(waterflow);
                }
            }
        }
	}
}
