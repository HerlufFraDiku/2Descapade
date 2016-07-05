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
				player.OnEvadeKey ();
			}

			if (Input.GetKeyDown (KeyCode.Space)) {
				print ("Space Pressed");
				player.OnSpaceDown ();
			}

			if (Input.GetKeyUp (KeyCode.Space)) {
				player.OnSpaceUp ();
			}
		}
	}
}
