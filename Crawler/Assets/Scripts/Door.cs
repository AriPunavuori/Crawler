using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    GameManager gm;
    BoxCollider bc;
    Vector3 startPosition;
    Vector3 endPosition;
    float speed = .5f;
    public bool opened;

    float openingSpeed;
    void Start() {
        gm = FindObjectOfType<GameManager>();
        bc = GetComponent<BoxCollider>();
        startPosition = transform.position;
        endPosition = transform.position + Vector3.down;
        print(endPosition);
    }
    void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.CompareTag("Player") && !opened) {
            if(gm.UseKey()) {
                bc.enabled = false;
                opened = true;
            }
        }
    }
    void Update() {
        if(opened) {
            transform.Translate(Vector3.down * Time.deltaTime * speed, Space.World);
            if(transform.position.y < endPosition.y)
                Destroy(gameObject);
        }
    }
}
