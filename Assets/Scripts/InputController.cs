using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player))]
public class InputController : MonoBehaviour {
	Player player;

	void Start (){
		player = GetComponent<Player> ();
	}

	void Update () {
		player.input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

		if (player.mayMove) {

			if (Input.GetKeyDown (KeyCode.X)) {
				player.OnAttackKey ();
			}

			if (Input.GetKeyDown (KeyCode.Z)) {
				player.OnEvadeKey ();
			}

			if (Input.GetKeyDown (KeyCode.Space)) {
				player.OnSpaceDown ();
			}

			if (Input.GetKeyUp (KeyCode.Space)) {
				player.OnSpaceUp ();
			}
		}
	}
}
