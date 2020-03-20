using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmilEnemyCharacter : Character {

	public GameObject player;

	void Update() {

		if (GameObject.Find("MagicalGirl(Clone)") != null && player == null) {
			player = GameObject.Find("MagicalGirl(Clone)");
		}

		if (player != null) {
			float dist = Vector2.Distance(transform.position, player.transform.position);
			Debug.DrawLine(transform.position, player.transform.position, Color.red);

			if (!Physics.Linecast(transform.position, player.transform.position) && dist < 7) { //Välissä ei esteitä ja etäisyys on < x
				Vector3 moveDir = (player.transform.position - transform.position).normalized;
				transform.position += moveDir * speed * Time.deltaTime;
				//Seurataan pelaajaa (liikutaan pelaajan suuntaan)
			} else {
				//On este, ei voi seurata.
			}
		}
	}
}
