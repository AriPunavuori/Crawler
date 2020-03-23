using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCharacter : Character {



    bool potion;

    void Start() {
        if(characterType == EntityType.Hero0) {
            ranged = true;
            damage = 50;
            attackRange = 20;
            attackInterval = 0.5f;
            speed = 10;
            health = 100;
        }
        if (characterType == EntityType.Hero1)
        {
            ranged = false;
            damage = 50;
            attackRange = 20;
            attackInterval = 0.5f;
            speed = 10;
            health = 70;
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


    void Update() 
    {
        

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If colliding gameobject contains projectile component?
        if (collision.gameObject.GetComponent<Projectile>())
        {
            if(collision.gameObject.GetComponent<Projectile>().shooter != characterType)
            {
                takeDamage(collision.gameObject.GetComponent<Projectile>().damage);
            }
        }
    }


    public void GetSpeed() {
        speed += 10;
    }

}
