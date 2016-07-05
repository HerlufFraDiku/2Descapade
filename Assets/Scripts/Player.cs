using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(InputController))]
public class Player : MonoBehaviour {

	Controller2D controller;
	Vector3 velocity;
	[HideInInspector]public Vector2 input;

	[Header("Movement")]
	public float moveSpeed = 6f;
	public float climbSpeed = 4f;
	public float accelerationGrounded = .1f;
	public float accelerationAirborne = .2f;

	[HideInInspector]public bool mayMove;
	float facingDir;
	float xSmoothing;

	[Header("Jumping")]
	public float maxJumpHeight = 4f;
	public float minJumpHeight = 2f;
	public float timeToJumpApex = .3f;
	public float ropeJumpForce = 10f;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;

	[Header("Evading")]
	public float evadeSpeed = 10f;
	public float evadeDuration = 0.5f;
	public float evadeCooldown = 1f;

	float nextEvadeTime;
	float evadeEndtime;

	void Start () {
		controller = GetComponent<Controller2D> ();
		gravity = -(2f * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs (gravity) * timeToJumpApex; 
		minJumpVelocity = Mathf.Sqrt (2f * Mathf.Abs (gravity) * minJumpHeight);
		mayMove = true;
		facingDir = 1f;
	}

	void Update () {
		if (input.x != 0) {
			facingDir = input.x;
		}

		if (controller.collisions.below || controller.collisions.above || controller.collisions.climbing) {
			velocity.y = 0;
		}

		if (mayMove) {
			CalculateXVelocity ();
		}

		CalculateYVelocity ();

		controller.Move (velocity * Time.deltaTime, input);
	}
		
	public void OnSpaceDown(){
		print ("Space Down Called");
		if (controller.collisions.climbing) {
			controller.collisions.climbing = false;
			velocity = new Vector2 (ropeJumpForce * input.x, 8);
		} else if (controller.collisions.below && input.y != -1) {
			print ("Jump Force Should be applied");
			velocity.y = maxJumpVelocity;
		}
	}

	public void OnSpaceUp(){
		if (velocity.y > minJumpVelocity) {
			velocity.y = minJumpVelocity;
		}
	}

	public void OnEvadeKey(){
		if(controller.collisions.below && !controller.collisions.climbing && nextEvadeTime < Time.time){
			evadeEndtime = Time.time + evadeDuration;
			mayMove = false;
			StartCoroutine ("Evade");
		}
	}

	void CalculateXVelocity (){
		if (!controller.collisions.climbing) {
			float targetVelocityX = input.x * moveSpeed;
			velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref xSmoothing, (controller.collisions.below) ? accelerationGrounded : accelerationAirborne);
		} else {
			velocity.x = 0f;
		}
	}

	void CalculateYVelocity (){
		if (!controller.collisions.climbing) {
			velocity.y += gravity * Time.deltaTime;
		} else {
			velocity.y = input.y * climbSpeed;
		}
	}

	IEnumerator Evade(){
		while (Time.time < evadeEndtime) {
			velocity.x = evadeSpeed * facingDir;
			yield return null;
		}

		nextEvadeTime = Time.time + evadeCooldown;
		mayMove = true;
	}
}
