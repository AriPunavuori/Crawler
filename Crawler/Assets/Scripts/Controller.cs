using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
    Vector3 movement;
    float rotation;
    float rotateSpeed = 100f;
    float moveSpeed = 10f;
    void Start() {
        Cursor.visible = false;
    }

    void Update() {
        movement.x = Input.GetAxis("Horizontal");
        movement.z = Input.GetAxis("Vertical");
        transform.Translate(movement* Time.deltaTime * moveSpeed);
        rotation = Input.GetAxis("Mouse X");
        transform.Rotate(0, rotation * rotateSpeed * Time.deltaTime, 0);
    }
}
