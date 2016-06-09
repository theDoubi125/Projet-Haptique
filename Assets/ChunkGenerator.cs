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
                    instance.transform.position = transform.position + new Vector3(31 * i, 31 * j, 31 * k);
					instance.GetComponent<WaterFlow> ().m_material = material;
                    cursor.instances.Add(instance.GetComponent<WaterFlow>());
                }
            }
        }
	}
}
