using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUpgrade : MonoBehaviour {
    void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player")) {
            var pc = other.GetComponent<PlayerCharacter>();
            if(pc.weaponLevel < 3) {
                pc.GetWeaponUpgrade();
                Destroy(gameObject);
            }
        }
    }
}
