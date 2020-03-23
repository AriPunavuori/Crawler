using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType { Hero0, Hero1, Hero2, Hero3, enemy0, enemy1, enemy2, enemy3, env }
public class Character : MonoBehaviour {

    public EntityType characterType;

    public bool ranged;
    public int damage;
    public int attackRange;
    public float attackInterval;
    public float speed;
    public int health;


    public void takeDamage(int dmg)
    {
        if(health - dmg < 0)
        {
            health = 0;
        }
        else
        {
            health -= dmg;
        }
    }

    

}
