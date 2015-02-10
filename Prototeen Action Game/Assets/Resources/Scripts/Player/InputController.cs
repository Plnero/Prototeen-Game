using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {
	// Player script references
	private MovementController _movement;

	// Connected Gamepads
	public string[] GamePads;

	// Keyboard default movement keys (later move this to a specialized class)
	public KeyCode Forward = KeyCode.W;
	public KeyCode Backward = KeyCode.S;
	public KeyCode Left = KeyCode.A;
	public KeyCode Right = KeyCode.D;
	public KeyCode Walking = KeyCode.LeftAlt;
	public KeyCode CyberSprint = KeyCode.LeftShift;

	// Player input
	private bool _forward;
	private bool _backward;
	private bool _left;
	private bool _right;
	private bool _walking;
	private bool _cyberSprint;

	// Movement input
	private Vector2 _movementInput;
	private Vector2 _cameraInput;

	// Use this for initialization
	public void Initialize (MovementController movement) {
		// Set initial References
		_movement = movement;

		// Detect Gamepad presence
		DetectGamepad ();
		Invoke("DetectGamepad", 1);
	}

	/// <summary>
	/// Detects gamepad presence.
	/// </summary>
	void DetectGamepad()
	{
		GamePads = Input.GetJoystickNames ();
	}

	// Update is called once per frame
	void Update () {
		// Get Gameplay Input
		CatchGameplayInput ();

		// Execute gameplay input
		ExecuteMovementInput ();
	}

	/// <summary>
	/// Executes the movement input.
	/// </summary>
	void ExecuteMovementInput()
	{
		// Send movement vector
		if(_movementInput.magnitude > 0)
			_movement.TryMovement(_movementInput,_walking,_cyberSprint);

		// Send camera vector
		CameraSmoothFollow.Instance.SetLookAxis (_cameraInput);
	}

	/// <summary>
	/// Catchs the gameplay input.
	/// </summary>
	void CatchGameplayInput()
	{
		// Catch Keyboard movement
		if(GamePads.Length == 0)
			CatchKeyboardMovementInput();
		// Catch Gamepad movement
		else
			CatchGamepadMovementInput();

		// Get Movement Type Input
		_walking	 = Input.GetKey (Walking);
		_cyberSprint = Input.GetKey (CyberSprint) || Input.GetAxisRaw ("CyberSprint") > 0;
	}

	
	/// <summary>
	/// Catchs the keyboard movement input.
	/// </summary>
	void CatchKeyboardMovementInput()
	{
		// Get movement input
		_forward	= Input.GetKey (Forward);
		_backward	= Input.GetKey (Backward);
		_right		= Input.GetKey (Right);
		_left 		= Input.GetKey (Left);
		
		// Set input movement vector
		_movementInput = Vector2.zero;
		_movementInput.x += _right?     1 : 0;
		_movementInput.x += _left? 	   -1 : 0;
		_movementInput.y += _forward? 	1 : 0;
		_movementInput.y += _backward? -1 : 0;

		// Set camera input
		_cameraInput = new Vector2 (Input.GetAxis ("Mouse X"), Input.GetAxis ("Mouse Y"));
	}
	
	/// <summary>
	/// Catchs the gamepad movement input.
	/// </summary>
	void CatchGamepadMovementInput()
	{
		// Catch axis input
		_movementInput = new Vector2(Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));

		// First try to get the mouse input, if there is, it will be the listened one
		_cameraInput = new Vector2 (Input.GetAxis ("Mouse X"), Input.GetAxis ("Mouse Y"));
		// Otherwise, get the gamepad input
		if(_cameraInput.magnitude == 0)
			// Set camera input
			_cameraInput = new Vector2 (Input.GetAxis ("Horizontal R"),0);
	}
}
