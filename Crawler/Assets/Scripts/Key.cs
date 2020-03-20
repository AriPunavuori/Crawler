using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour {
    GameManager gm;

    private void Start() {
        gm = FindObjectOfType<GameManager>();
    }
    private void OnTriggerEnter(Collider other) {
        print("Hit key!");
        if(other.gameObject.CompareTag("Player")) {
            print("Hitting object was Player!");
            gm.FoundKey();
            Destroy(gameObject);
        }
    }
}
