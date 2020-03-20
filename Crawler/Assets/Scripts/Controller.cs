using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
    PlayerCharacter pc;
    Rigidbody2D rb2D;
    Vector2 movement;

    void Start() {
        pc = GetComponent<PlayerCharacter>();
        rb2D = GetComponent<Rigidbody2D>();
        Cursor.visible = false;
    }

    void Update() {
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
    }
    private void FixedUpdate() {
        rb2D.MovePosition(rb2D.position + movement * pc.speed *  Time.fixedDeltaTime);
    }
}
