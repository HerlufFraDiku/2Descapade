using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	public Controller2D target;
	public Vector2 focusAreaSize;
	public float verticalOffset;
	public float lookAheadDistX = 3f;
	public float lookAheadSmoothTimeX = 0.3f;
	public float verticalSmoothTime = 0.2f;

	FocusBox focusBox;
	float currentLookAheadX;
	float targetLookAheadX;
	float lookAheadDir;
	float smoothLookVelocityX;
	float verticalSmoothVelocity;
	bool lookAheadStopped;

	void Start () {
		focusBox = new FocusBox (target.boxCollider.bounds, focusAreaSize);
	}

	void LateUpdate () {
		focusBox.Update (target.boxCollider.bounds);

		Vector2 focusPosition = focusBox.center + Vector2.up * verticalOffset;

		if (focusBox.velocity.x != 0) {
			lookAheadDir = Mathf.Sign (focusBox.velocity.x);
			if (Mathf.Sign (target.playerInput.x) == Mathf.Sign (focusBox.velocity.x) && target.playerInput.x != 0) {
				lookAheadStopped = false;
				targetLookAheadX = lookAheadDistX * lookAheadDir;
			} else {
				if (!lookAheadStopped) {
					lookAheadStopped = true;
					targetLookAheadX = currentLookAheadX + (lookAheadDir * lookAheadDistX - currentLookAheadX)/4f;
				}
			}
		}

		currentLookAheadX = Mathf.SmoothDamp (currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookAheadSmoothTimeX);

		focusPosition.y = Mathf.SmoothDamp (transform.position.y, focusPosition.y, ref verticalSmoothVelocity, verticalSmoothTime);
		focusPosition += Vector2.right * currentLookAheadX;

		transform.position = (Vector3)focusPosition + Vector3.forward * -10;
	}

	void OnDrawGizmos(){
		Gizmos.color = new Color (1, 0, 0, 0.5f);
		Gizmos.DrawCube (focusBox.center, focusAreaSize);
	}

	struct FocusBox {
		public Vector2 center;
		public Vector2 velocity;
		float left, right;
		float top, bottom;

		public FocusBox(Bounds targetBounds, Vector2 size){
			left = targetBounds.center.x - size.x / 2;
			right = targetBounds.center.x + size.x / 2;
			top = targetBounds.min.y + size.y;
			bottom = targetBounds.min.y;
			velocity = Vector2.zero;
			center = new Vector2((left+right)/2, (top+bottom)/2);
		}

		public void Update(Bounds targetBounds){
			float shiftX = 0;
			if (targetBounds.min.x < left) {
				shiftX = targetBounds.min.x - left;
			} else if (targetBounds.max.x > right) {
				shiftX = targetBounds.max.x - right;
			}

			left += shiftX;
			right += shiftX;

			float shiftY = 0;
			if (targetBounds.min.y < bottom) {
				shiftY = targetBounds.min.y - bottom;
			} else if (targetBounds.max.y > top) {
				shiftY = targetBounds.max.y - top;
			}
			bottom += shiftY;
			top += shiftY;

			center = new Vector2((left+right)/2, (top+bottom)/2);
			velocity = new Vector2 (shiftX, shiftY);
		}
	}
}
