using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour {

	[HideInInspector]public BoxCollider2D boxCollider;
	RaycastOrigins rayOrigins;
	public CollisionInfo collisions;

	public LayerMask climbable;
	public LayerMask collisionMask;

	[Range(2, 256)] public int horizontalRayCount = 4;
	[Range(2, 256)] public int verticalRayCount = 4;
	float horizontalRaySpacing;
	float verticalRaySpacing;
	const float skinWidth = 0.015f;

	[HideInInspector]public Vector2 playerInput;

	void Awake () {
		boxCollider = GetComponent<BoxCollider2D> ();
	}

	void Start(){
		CalculateRaySpacing ();
	}


	public void Move(Vector3 velocity, Vector2 input){
		UpdateRayOrigins ();
		collisions.Reset ();
		playerInput = input;

		CheckForRope (velocity);

		if (velocity.x != 0) {
			HorizontalCollisions (ref velocity);
		}

		if (velocity.y != 0) {
			VerticalCollisions (ref velocity);
		}

		if (collisions.climbing) {
			velocity.x = 0;
		}

		transform.Translate (velocity);
	}

	void CheckForRope(Vector3 velocity){
		Bounds bounds = boxCollider.bounds;
		float yOffset = (playerInput.y == -1 && !collisions.climbing) ? -bounds.size.y / 2 : 0f;
		Vector2 rayOrigin = new Vector2 (bounds.center.x, bounds.center.y + yOffset);
		Debug.DrawRay (rayOrigin, Vector2.down * bounds.size.y / 2f, Color.blue);
		Collider2D climbableHit = Physics2D.OverlapCircle(rayOrigin, bounds.size.y / 2f, climbable);

		if (climbableHit) {
			if (!collisions.climbing && playerInput.y != 0) {
				transform.position = new Vector3 (climbableHit.transform.position.x, transform.position.y, 0);
				collisions.climbing = true;
			}
		} else {
			collisions.climbing = false;
		}
	}

	void HorizontalCollisions(ref Vector3 velocity){
		float directionX = Mathf.Sign (velocity.x);
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;

		for (int i = 0; i < horizontalRayCount; i++) {
			Vector3 rayOrigin = (directionX == -1)?rayOrigins.bottomLeft:rayOrigins.bottomRight;
			rayOrigin += Vector3.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector3.right * directionX, rayLength, collisionMask);

			Debug.DrawRay (rayOrigin, Vector2.right * directionX * rayLength, Color.red);

			if (hit) {
				if (hit.collider.tag == "Jumpable") {
					if (hit.distance == 0) {
						continue;
					}
				}

				velocity.x = (hit.distance - skinWidth) * directionX;
				rayLength = hit.distance;

				collisions.left = directionX == -1f;
				collisions.right = directionX == 1f;
			}
		}
	}

	void VerticalCollisions(ref Vector3 velocity){
		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++) {
			Vector3 rayOrigin = (directionY == -1)?rayOrigins.bottomLeft:rayOrigins.topLeft;
			rayOrigin += Vector3.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector3.up * directionY, rayLength, collisionMask);

			Debug.DrawRay (rayOrigin, Vector2.up * directionY * rayLength, Color.red);

			if (hit) {
				if (collisions.climbing) {
					continue;
				}
				if (hit.collider.tag == "Jumpable") {
					if (directionY == 1 || hit.distance == 0 || (playerInput.y == -1 && Input.GetKeyDown(KeyCode.Space))) {
						continue;
					}
				}

				velocity.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;
			
				collisions.below = directionY == -1f;
				collisions.above = directionY == 1f;

			}
		}
	}

	void UpdateRayOrigins(){
		Bounds bounds = boxCollider.bounds;
		bounds.Expand (skinWidth * -2f);

		rayOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		rayOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
		rayOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		rayOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
	}

	void CalculateRaySpacing(){
		Bounds bounds = boxCollider.bounds;
		bounds.Expand (skinWidth * -2f);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}


	public struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	public struct CollisionInfo{
		public bool above, below;
		public bool left, right;
		public bool rope;
		public bool climbing;

		public void Reset(){
			above = below = false;
			left = right = false;
		}
	}
}
