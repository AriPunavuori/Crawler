using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType { Hero0, Hero1, Hero2, Hero3, enemy0, enemy1, enemy2, enemy3, env }
public class Character : Photon.MonoBehaviour {

    public EntityType characterType;

    public bool npc;

    public bool ranged;

    public float projectileSpeed;
    public float attackAngle;
    public int attackRange;
    public int damage;

    public float attackInterval;
    public float attackTimer;
    public float speed;
    public int health;

    public GameObject projectileHeading;
    public GameObject boltSpawn;
    public GameObject projectile;

    public Vector2 movement;

    public void takeDamage(int dmg) {
        if(health - dmg < 0) {
            // Grim reaper calling
            health = 0;
        } else {
            // Still alive
            health -= dmg;
        }
    }
    [PunRPC]
    public void Shoot() {
        GameObject projectileClone = Instantiate(this.projectile, boltSpawn.transform.position, Quaternion.identity);
        Projectile projectile = projectileClone.GetComponent<Projectile>();
        // Set damage of the projectile
        projectile.damage = damage;
        // Set speed of the projectile
        projectile.speed = projectileSpeed;

        projectile.transform.SetParent(projectileHeading.transform);
    }
    [PunRPC]
    public void Melee() {
        if(attackTimer < 0) {
            attackTimer = attackInterval;
        }
    }
}
