using UnityEngine;
using System.Collections;

public class WaterDrop : MonoBehaviour {
    public Vector3 speed;
    public float weight;

	void Start () {
	
	}
	

	void Update () {
        transform.position += speed * Time.deltaTime;
	}
}
