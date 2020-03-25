using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCharacter : Character {

    Rigidbody2D rb2D;
    bool potion;

    void Start() {
        rb2D = GetComponent<Rigidbody2D>();
        SetCharacterAttributes();
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
                Attack();
            }
            // Movement input
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
        }
    }

    private void FixedUpdate() {
        // Move the PlayerCharacter of the correct player
        if(photonView.isMine) {
            if(rb2D != null)
                rb2D.velocity = new Vector2(movement.x * speed, movement.y * speed).normalized * speed;
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
