
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character {

	private Rigidbody2D rigidBody;
	LayerMask layerMask;
	public GameObject player;
	public GameObject aimer;
	Vector3 target;
	float targetDistance = 1.25f;
	float detectionDistance = 7f;
	public Collider2D[] players;
	bool following;

	void Start() {
		layerMask = LayerMask.GetMask("Player", "Obstacles");
		rigidBody = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate() {

		if (player == null) { // Jos ei jahdattavaa
			players = Physics2D.OverlapCircleAll(transform.position, detectionDistance, LayerMask.GetMask("Player")); //Etsi 2Dcollidereita detectionDistance-kokoiselta, ympyrän muotoiselta alueelta
			if (players.Length > 0) { // Jos löytyi pelaaja/pelaajia
				player = FindClosest(); // Aseta lähin löytynyt pelaaja jahdattavaksi
				following = true;
			}
		} else { // Jos on jahdattava
			Vector2 dirVector = player.transform.position - transform.position; // Pelaajan suuntaan vihollisesta
			RaycastHit2D hit = Physics2D.Raycast(transform.position, dirVector, detectionDistance, layerMask); // Castataan ray pelaajaan päin
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
				if (Vector3.Distance(transform.position, player.transform.position) < attackRange) { // Jos on jahdattava on tarpeeksi lähellä hyökkäystä varten
					if (attackTimer >= attackInterval) { // Odota attackInterval -pituinen aika
						Shoot();
						photonView.RPC("Shoot", PhotonTargets.Others);
						Debug.Log("En Garde!");
						attackTimer = 0;
					} else {
						attackTimer += Time.deltaTime;
					}
				}
			}
			following = true;

			// Ampuminen myös paikaltaan
			if (player != null) {
				if (Vector3.Distance(transform.position, player.transform.position) < attackRange) {
					if (attackTimer >= attackInterval) {
						Shoot();
						photonView.RPC("Shoot", PhotonTargets.Others);
						Debug.Log("En Garde!");
						attackTimer = 0;
					} else {
						attackTimer += Time.deltaTime;
					}
				}
			}

			// Tähtää pelaajaa päin
			aimer.transform.right = target - transform.position;

			if (health <= 0) {
				Destroy(gameObject); // Kuole
			}
		}
	}

	GameObject FindClosest() {
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionDistance, LayerMask.GetMask("Player"));
		if (colliders.Length > 0) {
			GameObject closest = colliders[0].gameObject;
			float shortestDist = Vector3.Distance(transform.position, closest.transform.position);
			for (int i = 0; i < colliders.Length; i++) {
				float dist = Vector3.Distance(transform.position, colliders[i].gameObject.transform.position);
				if (dist < shortestDist) {
					closest = colliders[i].gameObject;
					shortestDist = dist;
				}
			}
			return closest;
		}
		return player;
	}
}
