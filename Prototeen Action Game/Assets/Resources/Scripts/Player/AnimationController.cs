using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {
	// Player references
	private Animator _animator;

	// Player animation parameters
	[System.NonSerialized]
	public float CurrentMovementSpeed = 0;
	public bool inCyberSprint = false;

	// Use this for initialization
	public void Initialize () {
		// Get player references
		_animator = GetComponentInChildren<Animator> ();
	}

	/// <summary>
	/// Sets the cyber sprint.
	/// </summary>
	/// <param name="status">If set to <c>true</c> status.</param>
	public void SetCyberSprint(bool status)
	{
		// Start Cyber Sprint
		if (!inCyberSprint && status) {
			_animator.SetTrigger("CyberSprint_Trigger");
			Debug.Log("TRIGGER");
		}

		// Set Cyber Sprint Status
		_animator.SetBool ("CyberSprint", status);
		inCyberSprint = status;
	}
	
	// Update is called once per frame
	void Update () {
		// Update animator variables
		_animator.SetFloat ("WalkSpeed", CurrentMovementSpeed);
	}
}
