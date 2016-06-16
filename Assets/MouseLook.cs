using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour {

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX;
	public float sensitivityY;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	float rotationY = 0;
    float rotationX = 0;

    Vector2 lastMousePos;

    public HapticArmController controller;


    void Update ()
	{
        if (Input.GetMouseButtonDown(1))
            lastMousePos = Input.mousePosition;
        if(Input.GetMouseButton(1))
        {
            Vector2 mouseMovement = ((Vector2)Input.mousePosition - lastMousePos);
            rotationX += mouseMovement.x * sensitivityX;
            rotationY += mouseMovement.y * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, -90, 90);
            lastMousePos = Input.mousePosition;
        }
        Vector2 armPos = new Vector2(controller.armPos.y, controller.armPos.z);
        if (controller.switches[1])
        {
            Vector2 mouseMovement = (armPos - lastArmPos);
            rotationX += mouseMovement.x * sensitivityX;
            rotationY += mouseMovement.y * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, -90, 90);
        }
        lastArmPos = armPos;
        transform.rotation = Quaternion.Euler(rotationY, -rotationX, 0);
    }
	
	void Start ()
	{
		// Make the rigid body not change rotation
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
	}

    private Vector2 lastArmPos;
    private bool isButtonPressed = false;
}