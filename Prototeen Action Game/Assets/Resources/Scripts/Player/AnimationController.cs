using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {
	// Player references
	private Animator _animator;

	// Player animation parameters
	public float CurrentMovementSpeed = 0;

	// Use this for initialization
	public void Initialize () {
		// Get player references
		_animator = GetComponentInChildren<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		// Update animator variables
		_animator.SetFloat ("WalkSpeed", CurrentMovementSpeed);
	}
}
