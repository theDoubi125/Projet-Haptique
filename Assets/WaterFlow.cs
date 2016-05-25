using UnityEngine;
using System.Collections;

public class WaterFlow : MonoBehaviour {
    public WaterDrop[] drops;
    // Use this for initialization

    public Material m_material;
    
    private GameObject m_mesh;
    private float[,,] voxels;
    public float target = 1 / 5f;
    private float currentTarget;
    private float time;

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

        //Fill voxels with values. Im using perlin noise but any method to create voxels will work
        CalcVoxels(width, height, length);

        Mesh mesh = MarchingCubes.CreateMesh(voxels);

        //The diffuse shader wants uvs so just fill with a empty array, there not actually used
        mesh.uv = new Vector2[mesh.vertices.Length];
        mesh.RecalculateNormals();

        m_mesh = new GameObject("Mesh");
        m_mesh.AddComponent<MeshFilter>();
        m_mesh.AddComponent<MeshRenderer>();
        m_mesh.GetComponent<Renderer>().material = m_material;
        m_mesh.GetComponent<MeshFilter>().mesh = mesh;
        //Center mesh
        m_mesh.transform.localPosition = new Vector3(-32 / 2, -32 / 2, -32 / 2);
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
            Mesh mesh = MarchingCubes.CreateMesh(voxels);

            //The diffuse shader wants uvs so just fill with a empty array, there not actually used
            mesh.uv = new Vector2[mesh.vertices.Length];
            mesh.RecalculateNormals();
            DestroyImmediate(m_mesh.GetComponent<MeshFilter>().sharedMesh, true);
            m_mesh.GetComponent<MeshFilter>().mesh = mesh;
            m_mesh.transform.position = transform.position;
        }*/
    }


    private void CalcVoxels(int width, int height, int length)
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
                    if (x > width-5 && y > 5 && y < height - 5 && z > 5 && z < length - 5)
                        voxels[x, y, z] = 10;
                }
            }
        }
    }
}
