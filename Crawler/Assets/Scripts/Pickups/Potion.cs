using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour {
	void OnTriggerStay2D(Collider2D other) {
		if (other.gameObject.CompareTag("Player")) {
			var pc = other.GetComponent<PlayerCharacter>();
			if (!pc.potion) {
				pc.GetPotion();
				Destroy(gameObject);
			}
		}
	}
}
