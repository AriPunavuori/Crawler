using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character {
    public enum PlayerType { Hero0, Hero1, Hero2, Hero3 };

    public PlayerType pt;

    bool potion;

    void Start() {
        if(pt == PlayerType.Hero0) {
            ranged = true;
            damage = 50;
            attackRange = 20;
            attackInterval = 0.5f;
            speed = 10;
            health = 100;
        }
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.H)) {
            UsePotion();
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
