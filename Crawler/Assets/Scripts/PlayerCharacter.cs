using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCharacter : Character {

    Rigidbody2D rb2D;
    bool potion;

    void Start() {
        rb2D = GetComponent<Rigidbody2D>();

        if(characterType == EntityType.Hero0) {
            ranged = true;
            damage = 50;
            attackRange = 20;
            projectileSpeed = 20f;
            attackAngle = 90;
            attackInterval = .2f;
            speed = 10;
            health = 100;

        }
        if(characterType == EntityType.Hero1) {
            ranged = false;
            damage = 50;
            attackRange = 3;
            projectileSpeed = 0f;
            attackAngle = 90;
            attackInterval = 0.5f;
            speed = 10;
            health = 70;
        }
    }

    void Update() {
        if(photonView.isMine) {
            attackTimer -= Time.deltaTime;
            // Health potion input
            if(Input.GetKeyDown(KeyCode.H)) {
                UsePotion();
            }
            // Attack input
            if(attackTimer < 0 && Input.GetKeyDown(KeyCode.Mouse0)) {
                if(ranged) {
                    Shoot();
                    photonView.RPC("Shoot", PhotonTargets.Others);
                } else {
                    Melee();
                    photonView.RPC("Melee", PhotonTargets.Others);
                }
                attackTimer = attackInterval;
            }
            // Movement input
            movement.x = Input.GetAxis("Horizontal");
            movement.y = Input.GetAxis("Vertical");
        }
        if (health <= 0)
        {
            Destroy(gameObject); // Kuole
        }
    }

    private void FixedUpdate() {
        // Move the PlayerCharacter of the correct player
        if(photonView.isMine) {
            if(rb2D != null)
                rb2D.MovePosition(rb2D.position + movement.normalized * speed * Time.fixedDeltaTime);
        }
    }

    public void UsePotion() {
        if(potion) {
            health += 100;
            potion = false;
        }
    }

    public void GetPotion() {
        potion = true;
    }

    public void GetSpeed() {
        speed += 10;
    }
}
