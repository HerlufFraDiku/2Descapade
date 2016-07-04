using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {

	Controller2D controller;

	public float maxJumpHeight = 4f;
	public float minJumpHeight = 2f;
	public float timeToJumpApex = .3f;
	public float moveSpeed = 6f;
	public float accelerationGrounded = .1f;
	public float accelerationAirborne = .2f;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	Vector3 velocity;
	float xSmoothing;

	void Start () {
		controller = GetComponent<Controller2D> ();
		gravity = -(2f * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs (gravity) * timeToJumpApex; 
		minJumpVelocity = Mathf.Sqrt (2f * Mathf.Abs (gravity) * minJumpHeight);
	}
	

	void Update () {

		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

		if (controller.collisions.below || controller.collisions.above || controller.collisions.onRope) {
			velocity.y = 0;
		}

		if (input.y != -1 && Input.GetKeyDown (KeyCode.Space) && controller.collisions.below) {
			velocity.y = maxJumpVelocity;
		}

		if (Input.GetKeyUp (KeyCode.Space)) {
			if (velocity.y > minJumpVelocity) {
				velocity.y = minJumpVelocity;
			}	
		}
			

		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref xSmoothing, (controller.collisions.below) ? accelerationGrounded : accelerationAirborne);

		if (!controller.collisions.onRope) {
			velocity.y += gravity * Time.deltaTime;
		}

		controller.Move (velocity * Time.deltaTime, input);
	}
}
