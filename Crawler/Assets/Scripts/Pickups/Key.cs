using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Key : MonoBehaviour {
	GameManager gm;

	private void Start() {
		gm = FindObjectOfType<GameManager>();
	}
	private void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag("Player")) {
			gm.FoundKey(gameObject.name, other.name);
			Destroy(gameObject);
		}
	}
}
