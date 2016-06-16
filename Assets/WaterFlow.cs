using UnityEngine;
using System.Collections;

public class WaterFlow : MonoBehaviour {
    public WaterDrop[] drops;
    // Use this for initialization

    public Material m_material;
    
    private GameObject m_mesh;
	private float[,,] voxels;
	private Color[,,] colors;
    public float target = 1 / 5f;
    private float currentTarget;
    private float time;
    private bool isDirty;

    public int x, y, z;

    void Start()
    {
        currentTarget = target;
        //Target is the value that represents the surface of mesh
        //For example the perlin noise has a range of -1 to 1 so the mid point is were we want the surface to cut through
        //The target value does not have to be the mid point it can be any value with in the range
        MarchingCubes.SetTarget(target);

        //Winding order of triangles use 2,1,0 or 0,1,2
        MarchingCubes.SetWindingOrder(0, 1, 2);

        //Set the mode used to create the mesh
        //Cubes is faster and creates less verts, tetrahedrons is slower and creates more verts but better represents the mesh surface
        //MarchingCubes.SetModeToCubes();
        MarchingCubes.SetModeToCubes();

        //The size of voxel array. Be carefull not to make it to large as a mesh in unity can only be made up of 65000 verts
        int width = 32;
        int height = 32;
        int length = 32;


		voxels = new float[width, height, length];
		colors = new Color[width, height, length];

        //Fill voxels with values. Im using perlin noise but any method to create voxels will work
        CalcVoxels(width, height, length);

        Mesh mesh = MarchingCubes.CreateMesh(voxels, colors);

        //The diffuse shader wants uvs so just fill with a empty array, there not actually used
        mesh.uv = new Vector2[mesh.vertices.Length];
        mesh.RecalculateNormals();

        m_mesh = new GameObject("Mesh");
        m_mesh.AddComponent<MeshFilter>();
        m_mesh.AddComponent<MeshRenderer>();
		m_mesh.GetComponent<Renderer>().material = m_material;
        m_mesh.GetComponent<MeshFilter>().mesh = mesh;
        //Center mesh
        m_mesh.transform.localPosition = transform.position + new Vector3(-32 / 2, -32 / 2, -32 / 2);
    }

    void Update()
    {
        time += Time.deltaTime;

        /*if (time > 1 / 10f)
        {
            time -= 1 / 10f;
            currentTarget = target;
            MarchingCubes.SetTarget(target);
            CalcVoxels(32, 32, 32);
            
        }*/

    }


    public void CalcVoxels(int width, int height, int length)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    voxels[x, y, z] = 0;
                    Vector3 xyz = new Vector3(x, y, z) + transform.position;
                    Vector3 center2 = new Vector3(width / 2, height / 2, length / 2);
                }
            }
        }
    }

	public void SetVoxel(int x, int y, int z, float value, Color color)
    {
        if (x >= 0 && y >= 0 && z >= 0 && x < 32 && y < 32 && z < 32)
        {
            if (voxels[x, y, z] != value)
                isDirty = true;
            voxels[x, y, z] = value;
        }
    }

    public void SetColor(int x, int y, int z, Color color)
    {
        if (x >= 0 && y >= 0 && z >= 0 && x < 32 && y < 32 && z < 32)
        {
            if (colors[x, y, z] != color)
                isDirty = true;
            colors[x, y, z] = new Color(color.r, color.g, color.b, color.a);
        }
    }

    public float GetVoxel(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0 || x >= 32 || y >= 32 || z >= 32)
            return 0;
        return voxels[x, y, z];
    }

    public Vector3 GetGradientAt(int x, int y, int z)
    {
        float gx = ((GetVoxel(x, y, z) - GetVoxel(x + 1, y, z)) - (GetVoxel(x, y, z) - GetVoxel(x - 1, y, z))) / 2;
        float gy = ((GetVoxel(x, y, z) - GetVoxel(x, y + 1, z)) - (GetVoxel(x, y, z) - GetVoxel(x, y - 1, z))) / 2;
        float gz = ((GetVoxel(x, y, z) - GetVoxel(x, y, z + 1)) - (GetVoxel(x, y, z) - GetVoxel(x, y, z - 1))) / 2;
        return new Vector3(gx, gy, gz);
    }

    public void UpdateMesh()
    {
        if (isDirty)
        {
			Mesh mesh = MarchingCubes.CreateMesh(voxels, colors);

            MarchingCubes.SetTarget(target);
			int lg = mesh.vertices.Length; 
            //The diffuse shader wants uvs so just fill with a empty array, there not actually used
			/*Color[] mColors = new Color[lg];
			Color tmp;
			for(int i = 0; i < lg; i++)
			{	
				mColors [i] = new Color32(255, 0, 0, 1);//Color.Lerp(Color.red, Color.green,mesh.vertices[i].y);

			}*/

			//mesh.SetColors (mColors);
			//mesh.colors = colors;
			//mesh.uv = new Vector2[lg];
            mesh.RecalculateNormals();
            DestroyImmediate(m_mesh.GetComponent<MeshFilter>().sharedMesh, true);
            m_mesh.GetComponent<MeshFilter>().mesh = mesh;
            m_mesh.transform.localPosition = transform.position + new Vector3(-32 / 2, -32 / 2, -32 / 2);
            isDirty = false;
        }
    }
}
