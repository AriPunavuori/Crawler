using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCharacter : Character {



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


}
