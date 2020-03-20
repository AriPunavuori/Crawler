using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmilEnemyCharacter : Character {

    private Rigidbody2D rigidBody;

    public GameObject player;
    void Start()
    {

        rigidBody = GetComponent<Rigidbody2D>();
    }
    void Update() {

		if (player == null) {
			player = GameObject.Find("MagicalGirl(Clone)");
		}

		if (player != null) {
			float dist = Vector3.Distance(transform.position, player.transform.position);
			Debug.DrawLine(transform.position, player.transform.position, Color.red);


			if (!Physics.Linecast(transform.position, player.transform.position) && dist < 7) { //Välissä ei esteitä ja etäisyys on < x
				Vector3 moveDir = (player.transform.position - transform.position).normalized;
                rigidBody.velocity = moveDir * speed;

                //transform.position += moveDir * speed * Time.deltaTime;
				//Seurataan pelaajaa (liikutaan pelaajan suuntaan)
			} else {
				//On este, ei voi seurata.
			}
		}
	}
}

