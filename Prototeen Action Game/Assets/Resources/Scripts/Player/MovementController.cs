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
		_moveDirection.y += Physics.gravity.y;

		// Move the controller
		_character.Move(_moveDirection * Time.deltaTime);

		// Reset movement variable
		_moveDirection = Vector3.zero;
	}

	/// <summary>
	/// Tries the movement. (Keyboard version)
	/// </summary>
	/// <param name="movementInput">Movement input.</param>
	public void TryMovement(Vector2 movementInput,bool walking)
	{
		// Forward vector relative to the camera along the x-z plane	
		Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
		forward.y = 0;
		forward = forward.normalized;

		// Right vector relative to the camera
		// Always orthogonal to the forward vector
		Vector3 right = new Vector3(forward.z, 0, -forward.x);

		// Target direction relative to the camera
		_moveDirection = movementInput.x * right + movementInput.y * forward;

		// Set movement direction based on input
		//_moveDirection = new Vector3 (movementInput.x, 0, movementInput.y);

		// Get Current movement speed
		_currentMovementSpeed = walking ? walkSpeed : runSpeed;

		// Add movement speed
		_moveDirection *= _currentMovementSpeed ;

		// Rotate towards movement direction
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(_moveDirection),rotateSpeed * Time.deltaTime);
	}

}
