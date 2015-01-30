using UnityEngine;
using System.Collections;

public class PlayerStatus : MonoBehaviour {
	// Player script references
	private MovementController _movement;
	private InputController _input;
	private AnimationController _animation;

	// Use this for initialization (Temporary).
	void Start () {
		// Get Player References
		_movement = GetComponent<MovementController> ();
		_input = GetComponent<InputController> ();
		_animation = GetComponent<AnimationController> ();

		// Initialize player scripts
		_animation.Initialize ();
		_movement.Initialize (_animation);
		_input.Initialize (_movement);
	}
}
