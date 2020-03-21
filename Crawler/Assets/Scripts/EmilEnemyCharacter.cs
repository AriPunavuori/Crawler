using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmilEnemyCharacter : Character {

	private Rigidbody2D rigidBody;
	LayerMask layerMask;
	public GameObject player;
	Vector3 target;
	float targetDistance = 1.25f;
	float detectionDistance = 5f;
	public Collider2D[] players;
	bool following;

	void Start() {
		layerMask = LayerMask.GetMask("Player", "Obstacles");
		rigidBody = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate() {

		if (player == null) { // Jos ei jahdattavaa
			players = Physics2D.OverlapCircleAll(transform.position, detectionDistance, LayerMask.GetMask("Player")); //Etsi 2Dcollidereita detectionDistance-kokoiselta, ympyrän muotoiselta alueelta
			if (players.Length > 0) { // Jos löytyi
				player = players[0].gameObject; // Aseta ensimmäinen löytynyt jahdattavaksi
				following = true;
			}
		} else { // Jos on jahdattava
			Vector2 dirVector = player.transform.position - transform.position; // Pelaajan suuntaan vihollisesta
			RaycastHit2D hit = Physics2D.Raycast(transform.position, dirVector, detectionDistance, layerMask); // Castataan ray pelaajaan
			rigidBody.velocity = Vector3.zero; // Pysähdytään

			if (hit.collider != null) { // Jos ray osui pelaajaan/seinään
				if (Vector3.Distance(transform.position, player.transform.position) < detectionDistance) { // Jos etäisyys jahdattavaan < 5
					Debug.DrawLine(transform.position, hit.point, Color.red); // Piirrä punanen debug viiva
					target = hit.point; // Jahdattavan objektin olinpaikka
				} else { // Jos meni liian kauas
					player = null; // Ei jahdattavaa
					following = false;
				}
			}

			if (Vector3.Distance(transform.position, target) > targetDistance && following) { // Jos etäisyys jahdattavan olinpaikkaan > targetDistance
				Vector3 moveDir = (target - transform.position).normalized; // Suunta jahdattavaa päin
				rigidBody.velocity = moveDir * speed; // liiku jahdattavan suuntaan
			}
		}
	}
}