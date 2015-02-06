using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class MovementController : MonoBehaviour {
	// Main character controller
	private CharacterController _character;
	private AnimationController _animation;

	// Player movement parameters
	[Range(0.1f,10.0f)]
	public float runSpeed = 2.0f; // max speed
	[Range(0.1f,10.0f)]
	public float walkSpeed = 1.0f;// min speed
	[Range(0.1f,1000.0f)]
	public float rotateSpeed = 500.0f;
	private float _currentMovementSpeed = 0.0f;

	// Player movement control variables
	private Vector3 _moveDirection = Vector3.zero;
	private Vector3 _surfaceNormal = Vector3.up;

	// CyberSprint Variables
	public float CyberSprintOffset = 0.6f;
	[Range(0,360)]
	public float AngleThreshold = 150.0f;

	// Raycast variables
	private RaycastHit _hit;
	[Range(0.1f,5.0f)]
	public float CyberSprintCheckDistance = 0.5f;

	// Use this for initialization
	public void Initialize (AnimationController animation) {
		// Set player references
		_animation = animation;

		// Get player references
		_character = GetComponent<CharacterController> ();
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void LateUpdate()
	{
		// Update the player animator
		_animation.CurrentMovementSpeed = _moveDirection.magnitude > 0 ? _currentMovementSpeed : 0;

		// Execute the calculated movement
		ExecuteMovement ();
	}

	/// <summary>
	/// Executes the movement.
	/// </summary>
	public void ExecuteMovement()
	{
		// Apply gravity
		_moveDirection += _surfaceNormal * Physics.gravity.y;

		// Move the controller
		_character.Move(_moveDirection * Time.deltaTime);

		// Reset movement variable
		_moveDirection = Vector3.zero;
	}

	private bool key = false;

	/// <summary>
	/// Checks the cyber sprint.
	/// </summary>
	/// <param name="Direction">Direction.</param>
	private void CheckCyberSprint(Vector3 moveDirection)
	{
		// Align yourself to hit.normal as up
		if(Physics.Raycast(transform.position, moveDirection.normalized, out _hit,CyberSprintCheckDistance)) 
		{
			// Check if the surface is a cybersprint surface
			if(_hit.collider.gameObject.GetComponent<CyberSprintSurface>() && 
			   Vector3.Angle(_hit.normal,transform.forward) > AngleThreshold)
			{
				Debug.Log("CYBERSPRINT SURFACE, COLLSION ANGLE: " + Vector3.Angle(_hit.normal,transform.forward));
				key = true;
				
				// Set new surface normal
				_surfaceNormal = _hit.normal.normalized;
				
				// Find forward direction with new myNormal:
				Vector3 myForward = Vector3.Cross(transform.right, _surfaceNormal);
				
				// Align character to the new myNormal while keeping the forward direction:
				Quaternion targetRot = Quaternion.LookRotation(myForward, _surfaceNormal);
				transform.rotation = targetRot;
				transform.position += transform.forward * CyberSprintOffset;
			}
		}
	}

	/// <summary>
	/// Tries the movement. (Keyboard version)
	/// </summary>
	/// <param name="movementInput">Movement input.</param>
	public void TryMovement(Vector2 movementInput,bool walking,bool CyberSprint = false)
	{
		// DEBUG
		if(key)return;

		// Forward vector relative to the camera along the x-z plane	
		Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
		forward.y = 0;
		forward = forward.normalized;

		// Right vector relative to the camera
		// Always orthogonal to the forward vector
		Vector3 right = new Vector3(forward.z, 0, -forward.x);

		// Target direction relative to the camera
		_moveDirection = movementInput.x * right + movementInput.y * forward;

		// Get Current movement speed
		_currentMovementSpeed = walking ? walkSpeed : runSpeed;

		// Add movement speed
		_moveDirection *= _currentMovementSpeed ;

		// Rotate towards movement direction
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(_moveDirection),rotateSpeed * Time.deltaTime);
	
		// Check for cyber sprint
		CheckCyberSprint (_moveDirection);
	}

}
