using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character {

	Rigidbody2D rigidBody;
	LayerMask layerMask;
	public GameObject player;
    Vector2 target;
	float targetDistance = 1.25f;
	float detectionDistance = 5f;
	public Collider2D[] players;
	bool following;

	void Start() {
		layerMask = LayerMask.GetMask("Player", "Obstacles");
		rigidBody = GetComponent<Rigidbody2D>();
		SetCharacterAttributes();
        photonView.TransferOwnership(1);
    }

	private void FixedUpdate() {

		if (player == null) { // Jos ei jahdattavaa
			players = Physics2D.OverlapCircleAll(transform.position, detectionDistance, LayerMask.GetMask("Player")); //Etsi 2Dcollidereita detectionDistance-kokoiselta, ympyrän muotoiselta alueelta
			if (players.Length > 0) { // Jos löytyi pelaaja/pelaajia
				player = FindClosest(); // Aseta lähin löytynyt pelaaja jahdattavaksi
                Debug.Log(player);


                int playerID = player.GetComponent<PhotonView>().ownerId;
                photonView.TransferOwnership(playerID);
                following = true;


			}
		} else { // Jos on jahdattava
			Vector2 dirVector = player.transform.position - transform.position; // Pelaajan suuntaan vihollisesta
			RaycastHit2D hit = Physics2D.Raycast(transform.position, dirVector, detectionDistance, layerMask); // Castataan ray pelaajaan päin
			rigidBody.velocity = Vector2.zero; // Pysähdytään

			if (hit.collider != null) { // Jos ray osui pelaajaan/seinään
				if (Vector2.Distance(transform.position, player.transform.position) < detectionDistance) { // Jos etäisyys jahdattavaan < 5
					Debug.DrawLine(transform.position, hit.point, Color.red); // Piirrä punanen debug viiva
					target = hit.point; // Jahdattavan objektin olinpaikka
				} else { // Jos meni liian kauas
					player = null; // Ei jahdattavaa
					following = false;
				}
			}
			if (Vector2.Distance(transform.position, target) > targetDistance && following) { // Jos etäisyys jahdattavan olinpaikkaan > targetDistance
				//Vector3 moveDir = (target - transform.position).normalized; // Suunta jahdattavaa päin
                float MoveDirX = target.x - transform.position.x;
                float MoveDirY = target.y - transform.position.y;

                rigidBody.velocity = new Vector2(MoveDirX , MoveDirY).normalized * speed;

                //rigidBody.velocity = moveDir * speed; // liiku jahdattavan suuntaan
			} else if (player != null && Vector2.Distance(transform.position, player.transform.position) < attackRange) { // Jos on jahdattava, joka tarpeeksi lähellä hyökkäystä varten
				if (attackTimer >= attackInterval) { // Odota attackInterval -pituinen aika
					Melee();
					attackTimer = 0;
				} else {
					attackTimer += Time.deltaTime;
				}
			}
			following = true;
		}
	}

	GameObject FindClosest() {
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionDistance, LayerMask.GetMask("Player"));
		if (colliders.Length > 0) {
			GameObject closest = colliders[0].gameObject;
			float shortestDist = Vector2.Distance(transform.position, closest.transform.position);
			for (int i = 0; i < colliders.Length; i++) {
				float dist = Vector2.Distance(transform.position, colliders[i].gameObject.transform.position);
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