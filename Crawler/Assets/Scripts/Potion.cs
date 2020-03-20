using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")) {
            var pc = other.GetComponent<PlayerCharacter>();
            pc.GetPotion();
            Destroy(gameObject);
        }
    }
}
