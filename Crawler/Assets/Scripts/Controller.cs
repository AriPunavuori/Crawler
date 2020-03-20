using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
    PlayerCharacter pc;
    Vector3 movement;

    void Start() {
        pc = GetComponent<PlayerCharacter>();
        Cursor.visible = false;
    }

    void Update() {
        movement.x = Input.GetAxis("Horizontal");
        movement.z = Input.GetAxis("Vertical");
        transform.Translate(movement* Time.deltaTime * pc.speed);
    }
}
