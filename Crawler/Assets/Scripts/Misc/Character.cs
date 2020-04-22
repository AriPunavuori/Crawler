using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType { Hero0, Hero1, Hero2, Hero3, Enemy0, Enemy1, Enemy2, Enemy3 }

public class Character : Photon.MonoBehaviour {

    public EntityType characterType;

    // Four first of the values are for heroes the rest for enemies
    bool[] rangeds = new bool[] { true, false, true, false, true, true, false, false }; // Ranger or melee
    float[] projectileSpeeds = new float[] { 15f, 7.5f, 7.5f, 6.5f, 6.5f, 6.5f, 10f, 10f }; // Speed of projectile
    float[] attackRanges = new float[] { 7.5f, 2.5f, 10f, 2.5f, 6.5f, 8f, 1.5f, 1.5f }; // Range of attack
    int[] damages = new int[] { 10, 20, 15, 25, 5, 5, 10, 10 };  // Amount of damage
    float[] attackIntervals = new float[] { 0.2f, 0.3f, 0.3f, 0.35f, .5f, .5f, .5f, .5f }; // Attack interval
    float[] speeds = new float[] { 4.5f, 4.5f, 4.5f, 4.5f, 3.5f, 3.5f, 3.5f, 3.5f }; // Movement speed
    int[] healths = new int[] { 200, 250, 200, 250, 20 , 30, 50, 60 }; // Health
    float[] specialCooldowns = new float[] { 3.0f, 10.0f, 7.0f, 10.0f, 3.5f, 3.5f, 3.5f, 3.5f }; // Special cooldowns

    public bool ranged;
    public float projectileSpeed;
    public float attackRange;
    public int damage;
    public float attackInterval;
    public float speed;
    public int health;
    public float specialCooldown;
   
    public float attackTimer;
    public int weaponLevel;

    public GameObject projectileSpawn;
    public GameObject projectilePrefab;

    public Vector2 movement;

    public int CheckCharacterHealt(EntityType et) {
        return healths[(int)et];
    }
    public void SetCharacterAttributes() {
        ranged = rangeds[(int)characterType];
        projectileSpeed = projectileSpeeds[(int)characterType];
        attackRange = attackRanges[(int)characterType];
        damage = damages[(int)characterType];
        attackInterval = attackIntervals[(int)characterType];
        speed = speeds[(int)characterType];
        health = healths[(int)characterType];
        specialCooldown = specialCooldowns[(int)characterType];
    }
}