using UnityEngine;
using System.Collections;

public class CameraSmoothFollow : MonoBehaviour {
	public GameObject target;                           // Target to follow
	public float targetHeight = 1.7f;                         // Vertical offset adjustment
	public float distance = 12.0f;                            // Default Distance
	public float offsetFromWall = 0.1f;                       // Bring camera away from any colliding objects
	public float maxDistance = 20f;                       // Maximum zoom Distance
	public float minDistance = 0.6f;                      // Minimum zoom Distance
	public float xSpeed = 200.0f;                             // Orbit speed (Left/Right)
	public float ySpeed = 200.0f;                             // Orbit speed (Up/Down)
	public float yMinLimit = -80f;                            // Looking up limit
	public float yMaxLimit = 80f;                             // Looking down limit
	public bool enableZoom = false;
	public float zoomRate = 40f;                          // Zoom Speed
	public float rotationDampening = 3.0f;                // Auto Rotation speed (higher = faster)
	public float zoomDampening = 5.0f;                    // Auto Zoom speed (Higher = faster)
	public float zoomCollisionDampening = 20.0f;		   // Collision Auto Zoom Speed
	public LayerMask collisionLayers = -1;     // What the camera will collide with
	public bool lockToRearOfTarget = false;             // Lock camera to rear of target
	public float VerticalAngle = 35.0f;
	
	private float xDeg = 0.0f;
	private float yDeg = 0.0f;
	private float currentDistance;
	private float desiredDistance;
	private float correctedDistance;
	private bool rotateBehind = false;
	private bool mouseSideButton = false;  
	private float pbuffer = 0.0f;       //Cooldownpuffer for SideButtons
	private float coolDown = 0.5f;      //Cooldowntime for SideButtons 
	
	// Shake parameters
	public float shakeDuration = 1.5f;
	public float shakeStrenght = 1.0f;
	public int shakeVibrato = 10;
	public float shakeRandomness = 90.0f;
	private bool collisionShaken = false;
	
	void Start ()
	{      
		if (transform.parent != null)
			transform.parent.position = Vector3.zero;
		
		Vector3 angles = transform.eulerAngles;
		xDeg = angles.x;
		yDeg = angles.y;
		currentDistance = distance;
		desiredDistance = distance;
		correctedDistance = distance;
		
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
		
		if (lockToRearOfTarget)
			rotateBehind = true;
	}
	
	//Only Move camera after everything else has been updated
	void LateUpdate ()
	{
		// Don't do anything if target is not defined
		if (target == null)
			return;
		//pushbuffer
		if(pbuffer>0)
			pbuffer -=Time.deltaTime;
		if(pbuffer<0)pbuffer=0;
		
		Vector3 vTargetOffset;
		yDeg = ClampAngle (yDeg, yMinLimit, yMaxLimit);
		
		// Set camera rotation
		//Quaternion rotation = Quaternion.Euler (VerticalAngle, xDeg, 0); // Dynamic Rotation
		Quaternion rotation = Quaternion.Euler (VerticalAngle, 0, 0); // Dynamic Rotation

		// Calculate the desired distance
		if(enableZoom)
			desiredDistance -= Input.GetAxis ("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs (desiredDistance);
		desiredDistance = Mathf.Clamp (desiredDistance, minDistance, maxDistance);
		correctedDistance = desiredDistance;
		
		// Calculate desired camera position
		vTargetOffset = new Vector3 (0, -targetHeight, 0);
		Vector3 position = target.transform.position - (rotation * Vector3.forward * desiredDistance + vTargetOffset);
		
		// Check for collision using the true target's desired registration point as set by user using height
		RaycastHit collisionHit;
		Vector3 trueTargetPosition = new Vector3 (target.transform.position.x, target.transform.position.y + targetHeight, target.transform.position.z);
		
		// If there was a collision, correct the camera position and calculate the corrected distance
		var isCorrected = false;
		if (Physics.Linecast (trueTargetPosition, position, out collisionHit, collisionLayers))
		{
			// Calculate the distance from the original estimated position to the collision location,
			// subtracting out a safety "offset" distance from the object we hit.  The offset will help
			// keep the camera from being right on top of the surface we hit, which usually shows up as
			// the surface geometry getting partially clipped by the camera's front clipping plane.
			correctedDistance = Vector3.Distance (trueTargetPosition, collisionHit.point) - offsetFromWall;
			isCorrected = true;
		}
		else
		{
			// Set collision flag
			if(transform.parent != null && collisionShaken)
				collisionShaken = false;
		}
		
		// For smoothing, lerp distance only if either distance wasn't corrected, or correctedDistance is more than currentDistance
		currentDistance = !isCorrected || correctedDistance > currentDistance ? Mathf.Lerp (currentDistance, correctedDistance, Time.deltaTime * zoomDampening) : 
			Mathf.Lerp (currentDistance, correctedDistance, Time.deltaTime * zoomCollisionDampening);
		
		// Keep within limits
		currentDistance = Mathf.Clamp (currentDistance, minDistance, maxDistance);
		
		// Recalculate position based on the new currentDistance
		position = target.transform.position - (rotation * Vector3.forward * currentDistance + vTargetOffset);
		
		//Finally Set rotation and position of camera
		transform.rotation = rotation;
		transform.localPosition = position;
	}
	
	private void RotateBehindTarget()
	{
		float targetRotationAngle = target.transform.eulerAngles.y;
		float currentRotationAngle = transform.eulerAngles.y;
		xDeg = Mathf.LerpAngle (currentRotationAngle, targetRotationAngle, rotationDampening * Time.deltaTime);
		
		// Stop rotating behind if not completed
		if (targetRotationAngle == currentRotationAngle)
		{
			if (!lockToRearOfTarget)
				rotateBehind = false;
		}
		else
			rotateBehind = true;
		
	}

	private float ClampAngle (float angle, float min, float max)
	{
		if (angle < -360f)
			angle += 360f;
		if (angle > 360f)
			angle -= 360f;
		return Mathf.Clamp (angle, min, max);
	}
}
