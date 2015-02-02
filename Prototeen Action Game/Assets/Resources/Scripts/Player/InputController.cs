﻿using UnityEngine;
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

	// Player input
	private bool _forward;
	private bool _backward;
	private bool _left;
	private bool _right;
	private bool _walking;

	// Movement input
	private Vector2 _movementInput;

	// Use this for initialization
	public void Initialize (MovementController movement) {
		// Set initial References
		_movement = movement;

		// Detect Gamepad presence
		//Invoke("DetectGamepad", 1);
		DetectGamepad ();
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
			_movement.TryMovement(_movementInput,_walking);
	}

	/// <summary>
	/// Catchs the gameplay input.
	/// </summary>
	void CatchGameplayInput()
	{
		// Get movement input
		_forward	= Input.GetKey (Forward);
		_backward	= Input.GetKey (Backward);
		_right		= Input.GetKey (Right);
		_left 		= Input.GetKey (Left);
		_walking	= Input.GetKey (Walking);

		// Set input movement vector
		_movementInput = Vector2.zero;
		_movementInput.x += _right?     1 : 0;
		_movementInput.x += _left? 	   -1 : 0;
		_movementInput.y += _forward? 	1 : 0;
		_movementInput.y += _backward? -1 : 0;
	}
}